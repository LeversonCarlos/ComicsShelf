using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace ComicsShelf.UWP
{
   partial class FileSystem
   {

      public async Task<ZipArchive> GetZipArchive(ComicsShelf.Helpers.Settings.Settings settings, string fullPath)
      {
         try
         {

            // INITIALIZE
            var splitedPath = fullPath
               .Split(new string[] { settings.Paths.Separator }, StringSplitOptions.RemoveEmptyEntries);

            Windows.Storage.StorageFolder storageFolder = null;
            if (settings.Paths.LibraryPath.Contains(this.PathSeparator))
            { storageFolder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(settings.Paths.LibraryPath); }
            else { storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(settings.Paths.LibraryPath); }
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

   }
}