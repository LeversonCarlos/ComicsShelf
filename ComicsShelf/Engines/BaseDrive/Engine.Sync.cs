using ComicsShelf.Store;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;

namespace ComicsShelf.Engines.BaseDrive
{
   partial class BaseDriveEngine<T>
   {
      private const string SyncFileName = "ComicsShelf.library";

      public async Task<bool> SaveSyncData(LibraryModel library, byte[] serializedValue)
      {
         try
         {
            FileVM syncFile = null;

            // SYNC FILE ID PREVIOUSLY SAVED
            var syncFileID = library.GetKeyValue(SyncFileName);

            // IF NO FILE WAS PREVISIOLLY CREATED, WE CREATE IT NOW ON THE PARENT FOLDER 
            if (string.IsNullOrEmpty(syncFileID))
               syncFile = await this.Service.Upload(library.Key, SyncFileName, serializedValue);

            // IF THE FILE WAS PREVISIOLLY CREATED, WE WILL OVERRIDE IT NOW
            if (!string.IsNullOrEmpty(syncFileID))
               syncFile = await this.Service.Upload(syncFileID, serializedValue);

            // IF NO FILE WAS STORED THEN JUST RETURN
            if (string.IsNullOrEmpty(syncFile?.ID)) return false;

            // STORE THE LIBRARY FILE ID AND RETURN
            library.SetKeyValue(SyncFileName, syncFile.ID);
            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return true; }
      }

      public async Task<byte[]> LoadSyncData(LibraryModel library)
      {
         try
         {
            byte[] byteArray = null;

            // LIBRARY FILE ALREADY DEFINED
            var fileID = await this.LoadDataAsync_GetFileID(library);
            if (string.IsNullOrEmpty(fileID)) { return null; }

            // LOAD CONTENT
            var serializedValue = await this.Service.Download(fileID);
            if (serializedValue == null) { return null; }

            // CONVERT TO BYTE ARRAY
            using (var memoryStream = new System.IO.MemoryStream())
            {
               await serializedValue.CopyToAsync(memoryStream);
               await memoryStream.FlushAsync();
               memoryStream.Position = 0;
               byteArray = memoryStream.ToArray();
            }

            return byteArray;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return null; }
      }

      private async Task<string> LoadDataAsync_GetFileID(LibraryModel library)
      {
         try
         {

            // LIBRARY FILE ALREADY DEFINED
            var fileID = library.GetKeyValue(SyncFileName);
            if (!string.IsNullOrEmpty(fileID)) { return fileID; }

            // TRY TO SEARCH ON FOLDER
            var directory = new DirectoryVM { ID = library.Key };
            var fileList = await this.Service.SearchFiles(directory, SyncFileName, 1);

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