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

            var comicFilePath = $"{comicFile.LibraryPath}{settings.Paths.Separator}{comicFile.FullPath}";
            if (!File.Exists(comicFilePath)) { return; }

            // OPEN ZIP ARCHIVE
            using (var zipArchiveStream = new System.IO.FileStream(comicFilePath, FileMode.Open, FileAccess.Read))
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

                  // RELEASE DATE
                  comicFile.ReleaseDate = database.GetDate(zipEntry.LastWriteTime.DateTime.ToLocalTime());
                  await Task.Run(() => database.Update(comicFile));

                  // OPEN STREAM
                  using (var zipEntryStream = zipEntry.Open())
                  {
                     await this.SaveThumbnail(zipEntryStream, comicFile.CoverPath);
                     zipEntryStream.Close();
                     zipEntryStream.Dispose();
                  }

                  zipArchive.Dispose();
               }

               zipArchiveStream.Close();
               zipArchiveStream.Dispose();
            }

         }
         catch { }
      }

      public async Task SaveThumbnail(Stream imageStream, string imagePath)
      {
         try
         {

            // LOAD IMAGE
            using (var originalBitmap = await Android.Graphics.BitmapFactory.DecodeStreamAsync(imageStream))
            {
               if (originalBitmap == null) { return; }

               // DEFINE SIZE
               double imageHeight = 450; double imageWidth = 150;
               double scaleFactor = (double)imageHeight / (double)originalBitmap.Height;
               imageHeight = originalBitmap.Height * scaleFactor;
               imageWidth = originalBitmap.Width * scaleFactor;

               // INITIALIZE THUMBNAIL STREAM
               using (var thumbnailFileStream = new FileStream(imagePath, FileMode.CreateNew, FileAccess.Write))
               {

                  // SCALE BITMAP
                  using (var thumbnailBitmap = Android.Graphics.Bitmap.CreateScaledBitmap(originalBitmap, (int)imageWidth, (int)imageHeight, false))
                  {
                     await thumbnailBitmap.CompressAsync(Android.Graphics.Bitmap.CompressFormat.Jpeg, 70, thumbnailFileStream);
                     await thumbnailFileStream.FlushAsync();
                  }
                  thumbnailFileStream.Close();

               }

            }

         }
         catch (Exception) { throw; }
      }

   }
}