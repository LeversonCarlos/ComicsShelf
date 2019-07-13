using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {
      private const string SYNC_FILE_ID = "SyncFileID";

      public async Task<bool> SaveSyncData(Libraries.LibraryModel library, byte[] serializedValue)
      {
         try
         {
            using (var serializedStream = new System.IO.MemoryStream(serializedValue))
            {
               var syncFile = new FileData { FileName = Libraries.LibraryModel.SyncFile };

               // SYNC FILE PREVIOUSLY SAVED
               syncFile.id = library.GetKeyValue(SYNC_FILE_ID);

               // SET THE MAIN FOLDER WHERE THE FILE MUST BE CREATED
               if (string.IsNullOrEmpty(syncFile.id))
               { syncFile.parentID = library.LibraryKey; }

               // UPLOAD CONTENT
               syncFile = await this.Connector.UploadAsync(syncFile, serializedStream);
               if (string.IsNullOrEmpty(syncFile.id)) { return false; }

               // STORE THE LIBRARY FILE ID
               library.SetKeyValue(SYNC_FILE_ID, syncFile.id);
               return true;
            }
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      public async Task<byte[]> LoadSyncData(Libraries.LibraryModel library)
      {
         try
         {

            // LIBRARY FILE ALREADY DEFINED
            var fileID = await this.LoadDataAsync_GetFileID(library);
            if (string.IsNullOrEmpty(fileID)) { return null; }

            // LOAD CONTENT
            var httpUrl = $"me/drive/items/{fileID}/content";
            var serializedValue = await this.Connector.GetByteArrayAsync(httpUrl);

            return serializedValue;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

      private async Task<string> LoadDataAsync_GetFileID(Libraries.LibraryModel library)
      {
         try
         {

            // LIBRARY FILE ALREADY DEFINED
            var fileID = library.GetKeyValue(SYNC_FILE_ID);
            if (!string.IsNullOrEmpty(fileID)) { return fileID; }

            // TRY TO SEARCH ON FOLDER
            var folder = new FileData { id = library.LibraryKey };
            var fileList = await this.Connector.SearchFilesAsync(folder, Libraries.LibraryModel.SyncFile, 100);
            if (fileList == null || fileList.Count == 0)
            { fileList = await this.Connector.SearchFilesAsync(folder, Libraries.LibraryModel.SyncFile_OLD, 100); }
            if (fileList == null || fileList.Count == 0) { return string.Empty; }
            var file = fileList.Where(x => x.parentID == library.LibraryKey).FirstOrDefault();
            if (file == null) { return string.Empty; }

            // RESULT
            return file.id;
         }
         catch (Exception) { throw; }
      }

   }
}