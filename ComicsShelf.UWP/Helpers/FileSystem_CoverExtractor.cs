using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.UWP
{
   partial class FileSystem
   {

      public async Task CoverExtract(Helpers.Settings.Settings settings, Helpers.Database.dbContext database, Helpers.Database.ComicFile comicFile)
      {
         try
         {

            var comicStorageFile = await GetStorageFile(settings, comicFile.FullPath);
            using (var zipArchiveStream = await comicStorageFile.OpenStreamForReadAsync())
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
      }

   }
}