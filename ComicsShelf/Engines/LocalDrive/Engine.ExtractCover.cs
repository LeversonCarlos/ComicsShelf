using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async override Task<bool> ExtractCover(LibraryModel library, ComicFile comicFile)
      {
         try
         {

            // VALIDATE
            if (!File.Exists(comicFile.FilePath)) { return false; }

            // OPEN STREAM
            using (var zipArchiveStream = await this.Service.Download(comicFile.Key))
            {
               using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Read))
               {

                  // FIRST ENTRY
                  var zipEntry = zipArchive.Entries
                     .Where(x =>
                        x.Name.ToLower().EndsWith(".jpg") ||
                        x.Name.ToLower().EndsWith(".jpeg") ||
                        x.Name.ToLower().EndsWith(".png"))
                     .OrderBy(x => x.Name)
                     .Take(1)
                     .FirstOrDefault();
                  if (zipEntry == null) { return false; }

                  // RETRIEVE IMAGE CONTENT
                  using (var zipEntryStream = zipEntry.Open())
                  {
                     await this.FileSystem.SaveThumbnail(zipEntryStream, comicFile.CoverPath);
                  }

                  // RELEASE DATE
                  File.SetLastWriteTime(comicFile.CoverPath, zipEntry.LastWriteTime.DateTime.ToLocalTime());
                  comicFile.ReleaseDate = zipEntry.LastWriteTime.DateTime.ToLocalTime();

               }
               zipArchiveStream.Close();
            }

            // RETURN
            return (File.Exists(comicFile.CoverPath));
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return false; }
      }

   }
}