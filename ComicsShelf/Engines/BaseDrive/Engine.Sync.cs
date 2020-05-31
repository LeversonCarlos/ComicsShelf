using ComicsShelf.Store;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;

namespace ComicsShelf.Engines.BaseDrive
{
   partial class BaseDriveEngine<T>
   {
      private const string SYNC_FILE_ID = "SyncFileID";

      public async Task<bool> SaveSyncData(LibraryModel library, byte[] serializedValue)
      {
         try
         {
            FileVM syncFile = null;

            // SYNC FILE ID PREVIOUSLY SAVED
            var syncFileID = library.GetKeyValue(SYNC_FILE_ID);

            // IF NO FILE WAS PREVISIOLLY CREATED, WE CREATE IT NOW ON THE PARENT FOLDER 
            if (string.IsNullOrEmpty(syncFileID))
               syncFile = await this.Service.Upload(library.Key, SYNC_FILE_ID, serializedValue);

            // IF THE FILE WAS PREVISIOLLY CREATED, WE WILL OVERRIDE IT NOW
            if (!string.IsNullOrEmpty(syncFileID))
               syncFile = await this.Service.Upload(syncFileID, serializedValue);

            // IF NO FILE WAS STORED THEN JUST RETURN
            if (string.IsNullOrEmpty(syncFile?.ID)) return false;

            // STORE THE LIBRARY FILE ID AND RETURN
            library.SetKeyValue(SYNC_FILE_ID, syncFile.ID);
            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return false; }
      }

      public async Task<byte[]> LoadSyncData(LibraryModel library)
      {
         try
         {

            // LIBRARY FILE ALREADY DEFINED
            var fileID = await this.LoadDataAsync_GetFileID(library);
            if (string.IsNullOrEmpty(fileID)) { return null; }

            // LOAD CONTENT
            var serializedValue = await this.Service.Download(fileID);

            return serializedValue;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return null; }
      }

      private async Task<string> LoadDataAsync_GetFileID(LibraryModel library)
      {
         try
         {

            // LIBRARY FILE ALREADY DEFINED
            var fileID = library.GetKeyValue(SYNC_FILE_ID);
            if (!string.IsNullOrEmpty(fileID)) { return fileID; }

            // TRY TO SEARCH ON FOLDER
            var directory = new DirectoryVM { ID = library.Key };
            var fileList = await this.Service.SearchFiles(directory, LibraryModel.SyncFile, 100);

            if (fileList?.Length == 0) { return string.Empty; }

            var file = fileList.Where(x => x.ParentID == library.Key).FirstOrDefault();
            if (file == null) { return string.Empty; }

            // RESULT
            return file.ID;
         }
         catch (Exception) { throw; }
      }

   }
}