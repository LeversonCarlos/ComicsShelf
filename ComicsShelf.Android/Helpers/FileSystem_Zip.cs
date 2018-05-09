using System.Threading.Tasks;
using System.IO.Compression;
using System;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      #region GetZipArchive
      public async Task<ZipArchive> GetZipArchive(ComicsShelf.Helpers.Settings.Settings settings, File.FileData comicFile)
      {
         try
         {

            // COMPOSE FILE PATH
            var comicFilePath = $"{settings.Paths.LibraryPath}{settings.Paths.Separator}{comicFile.FullPath}";

            // OPEN ZIP ARCHIVE
            ZipArchive zipArchive = null;
            await Task.Run(() => zipArchive = ZipFile.Open(comicFilePath, ZipArchiveMode.Read));

            // RESULT
            return zipArchive;
         }
         catch (Exception ex) { throw; }
      }
      #endregion      

   }
}