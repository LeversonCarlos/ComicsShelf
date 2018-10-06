﻿using System;
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

            // OPEN ZIP ARCHIVE
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

                  // OPEN STREAM
                  using (var zipEntryStream = zipEntry.Open())
                  {

                     // LOAD IMAGE
                     using (var originalBitmap = await Android.Graphics.BitmapFactory.DecodeStreamAsync(zipEntryStream))
                     {

                        // DEFINE SIZE
                        double imageHeight = 450; double imageWidth = 150;
                        double scaleFactor = (double)imageHeight / (double)originalBitmap.Height;
                        imageHeight = originalBitmap.Height * scaleFactor;
                        imageWidth = originalBitmap.Width * scaleFactor;

                        // INITIALIZE THUMBNAIL STREAM
                        // using (var thumbnailStream = new MemoryStream())
                        using (var thumbnailFileStream = new System.IO.FileStream(comicFile.CoverPath, FileMode.CreateNew, FileAccess.Write))
                        {

                           // SCALE BITMAP
                           using (var thumbnailBitmap = Android.Graphics.Bitmap.CreateScaledBitmap(originalBitmap, (int)imageWidth, (int)imageHeight, false))
                           {
                              await thumbnailBitmap.CompressAsync(Android.Graphics.Bitmap.CompressFormat.Jpeg, 70, thumbnailFileStream);
                              // thumbnailStream.Position = 0;
                              await thumbnailFileStream.FlushAsync();
                           }

                           thumbnailFileStream.Close();
                        }

                     }

                     // COVER THUMBNAIL
                     /*
                     using (var thumbnailFile = new System.IO.FileStream(comicFile.CoverPath, FileMode.CreateNew, FileAccess.Write))
                     {
                        await zipEntryStream.CopyToAsync(thumbnailFile);
                        await thumbnailFile.FlushAsync();
                        thumbnailFile.Close();
                        thumbnailFile.Dispose();
                     }   
                     */

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