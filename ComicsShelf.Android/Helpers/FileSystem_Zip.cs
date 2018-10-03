using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<ZipArchive> GetZipArchive(ComicsShelf.Helpers.Settings.Settings settings, string filePath)
      {
         try
         {

            // COMPOSE FILE PATH
            var comicFilePath = $"{settings.Paths.LibraryPath}{settings.Paths.Separator}{filePath}";

            // OPEN ZIP ARCHIVE
            ZipArchive zipArchive = null;
            await Task.Run(() => zipArchive = ZipFile.Open(comicFilePath, ZipArchiveMode.Read));

            // RESULT
            return zipArchive;
         }
         catch (Exception ex) { throw; }
      }

   }
}