using System.Threading.Tasks;
using System.IO.Compression;
using System;
using Windows.Storage.AccessCache;
using Windows.Storage;
using System.IO;

namespace ComicsShelf.UWP
{
   partial class FileSystem
   {

      #region GetZipArchive
      public async Task<ZipArchive> GetZipArchive(ComicsShelf.Helpers.Settings.Settings settings, File.FileData comicFile)
      {
         try
         {

            // INITIALIZE
            var splitedPath = comicFile.FullPath
               .Split(new string[] { settings.Paths.Separator }, StringSplitOptions.RemoveEmptyEntries);
            StorageFolder storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(settings.Paths.LibraryPath);
            StorageFile storageFile = null;

            // LOOP THROUGH SPLITED PATH PARTS
            for (int splitIndex = 0; splitIndex < splitedPath.Length; splitIndex++)
            {
               var folderPath = splitedPath[splitIndex];
               if (!folderPath.EndsWith(".cbz") && !folderPath.EndsWith(".cbr"))
               { storageFolder = await storageFolder.GetFolderAsync(folderPath); }
               else
               { storageFile = await storageFolder.GetFileAsync(folderPath); }
            }

            // OPEN ZIP ARCHIVE        
            Stream zipStream = await storageFile.OpenStreamForReadAsync();
            ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            // RESULT
            return zipArchive;
         }
         catch (Exception ex) { throw; }
      }
      #endregion      

   }
}