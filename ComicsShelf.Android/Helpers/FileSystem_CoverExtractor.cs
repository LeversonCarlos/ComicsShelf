using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task CoverExtract(Helpers.Settings.Settings settings, Helpers.Database.dbContext database, Helpers.Database.ComicFile comicFile)
      {
         try
         {

            var comicFilePath = $"{settings.Paths.LibraryPath}{settings.Paths.Separator}{comicFile.FullPath}";

            using (var zipArchiveStream = new System.IO.FileStream(comicFilePath, FileMode.Open, FileAccess.Read))
            {
               using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Read))
               {

                  // LOCATE FIRST JPG ENTRY
                  var zipEntry = zipArchive.Entries
                     .Where(x => x.Name.ToLower().EndsWith(".jpg"))
                     .OrderBy(x => x.Name)
                     .Take(1)
                     .FirstOrDefault();

                  // RELEASE DATE
                  comicFile.ReleaseDate = database.GetDate(zipEntry.LastWriteTime.DateTime.ToLocalTime());
                  await Task.Run(() => database.Update(comicFile));

                  // COVER THUMBNAIL
                  // await Task.Run(() => zipEntry.ExtractToFile(comicFile.CoverPath));

                  // OPEN STREAM
                  using (var zipEntryStream = zipEntry.Open())
                  {

                     // COVER THUMBNAIL
                     using (var thumbnailFile = new System.IO.FileStream(comicFile.CoverPath, FileMode.CreateNew, FileAccess.Write))
                     {
                        await zipEntryStream.CopyToAsync(thumbnailFile);
                        await thumbnailFile.FlushAsync();
                        thumbnailFile.Close();
                        thumbnailFile.Dispose();
                     }                    

                     zipEntryStream.Close();
                     zipEntryStream.Dispose();
                  }

                  zipArchive.Dispose();
               }

               zipArchiveStream.Close();
               zipArchiveStream.Dispose();
            }

         }
         catch (Exception ex) { }
         // finally { GC.Collect(); }
      }

   }
}