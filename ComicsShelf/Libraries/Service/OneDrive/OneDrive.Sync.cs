﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Libraries.Implementation
{
   partial class OneDrive
   {

      public async Task<bool> SaveDataAsync(Library library, byte[] serializedValue)
      {
         try
         {
            using (var uploadStream = new System.IO.MemoryStream(serializedValue))
            {
               var libraryFile = new FileData { FileName = Library.FileName };

               // LIBRARY FILE ALREADY DEFINED
               libraryFile.id = library.GetKeyValue(OneDrive.LibraryFileID);

               // WILL SET THE MAIN FOLDER WHERE THE FILE MUST BE CREATED
               if (string.IsNullOrEmpty(libraryFile.id))
               { libraryFile.parentID = library.LibraryID; }

               // UPLOAD CONTENT
               libraryFile = await this.Connector.UploadAsync(libraryFile, uploadStream);
               if (string.IsNullOrEmpty(libraryFile.id)) { return false; }

               // STORE THE LIBRARY FILE ID
               library.SetKeyValue(OneDrive.LibraryFileID, libraryFile.id);
               return true;
            }
         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("OneDrive.SaveDataAsync", ex); return false; }
      }

      public async Task<byte[]> LoadDataAsync(Library library)
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
         catch (Exception ex) { Engine.AppCenter.TrackEvent("OneDrive.LoadDataAsync", ex); return null; }
      }

      private async Task<string> LoadDataAsync_GetFileID(Library library)
      {

         // LIBRARY FILE ALREADY DEFINED
         var fileID = library.GetKeyValue(OneDrive.LibraryFileID);
         if (!string.IsNullOrEmpty(fileID)) { return fileID; }

         // TRY TO SEARCH ON FOLDER
         var folder = new FileData { id = library.LibraryID };
         var fileList = await this.Connector.SearchFilesAsync(folder, Library.FileName, 1);
         if (fileList == null || fileList.Count == 0) { return string.Empty; }
         var file = fileList.Where(x => x.parentID == library.LibraryID).FirstOrDefault();
         if (file == null) { return string.Empty; }

         // RESULT
         return file.id;

      }

   }
}