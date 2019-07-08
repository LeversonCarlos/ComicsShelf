using ComicsShelf.ComicFiles;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task ExtractCover(Libraries.LibraryModel library, ComicFile comicFile)
      {
         try
         {

            // OPEN ZIP ARCHIVE
            if (!File.Exists(comicFile.FilePath)) { return; }
            using (var zipArchiveStream = new FileStream(comicFile.FilePath, FileMode.Open, FileAccess.Read))
            {
               using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Read))
               {

                  // LOCATE FIRST JPG ENTRY
                  var zipEntry = zipArchive.Entries
                     .Where(x =>
                        x.Name.ToLower().EndsWith(".jpg") ||
                        x.Name.ToLower().EndsWith(".jpeg") ||
                        x.Name.ToLower().EndsWith(".png"))
                     .OrderBy(x => x.Name)
                     .Take(1)
                     .FirstOrDefault();
                  if (zipEntry == null) { return; }

                  // OPEN STREAM
                  using (var zipEntryStream = zipEntry.Open())
                  {
                     await this.SaveThumbnail(zipEntryStream, comicFile.CoverPath);
                     zipEntryStream.Close();
                     zipEntryStream.Dispose();
                  }

                  // RELEASE DATE
                  File.SetLastWriteTime(comicFile.CoverPath, zipEntry.LastWriteTime.DateTime.ToLocalTime());
                  comicFile.ReleaseDate = zipEntry.LastWriteTime.DateTime.ToLocalTime();

                  zipArchive.Dispose();
               }

               zipArchiveStream.Close();
               zipArchiveStream.Dispose();
            }
         }
         catch (Exception) { throw; }
      }

   }
}