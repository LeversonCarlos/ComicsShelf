using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task PagesExtract(Helpers.Settings.Settings settings, Views.File.FileData fileData)
      {
         try
         {

            // OPEN ZIP ARCHIVE            
            var comicFilePath = $"{fileData.ComicFile.LibraryPath}{settings.Paths.Separator}{fileData.FullPath}";
            using (var zipArchiveStream = new System.IO.FileStream(comicFilePath, FileMode.Open, FileAccess.Read))
            {
               using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Read))
               {
                  var heightPixels = Android.Content.Res.Resources.System.DisplayMetrics.HeightPixels;

                  // DEFINE EXTRACT PATH REMOVING INVALID CHARACTERS
                  var cachePath = fileData.FullPath
                     .Replace(settings.Paths.Separator, "_")
                     .Replace("#", "")
                     .Replace(":", "")
                     .Replace(".", "_")
                     .Replace(" ", "_")
                     .Replace("___", "_")
                     .Replace("__", "_");
                  cachePath = $"{settings.Paths.FilesCachePath}{settings.Paths.Separator}{cachePath}";
                  if (!Directory.Exists(cachePath)) { Directory.CreateDirectory(cachePath); }

                  // LOOP THROUGH ZIP ENTRIES
                  short pageIndex = 0;
                  var zipEntries = zipArchive.Entries
                     .Where(x => x.Name.ToLower().EndsWith(".jpg"))
                     .OrderBy(x => x.Name)
                     .ToList();
                  foreach (var zipEntry in zipEntries)
                  {

                     // PAGE DATA
                     var pageIndexText = pageIndex.ToString().PadLeft(3, "0".ToCharArray()[0]);
                     var pagePath = $"{cachePath}{settings.Paths.Separator}P{pageIndexText}.jpg";
                     var pageData = new Views.File.PageData { Page = pageIndex, Path = pagePath, IsVisible = false };

                     // OPEN STREAM
                     /*
                     using (var zipEntryStream = zipEntry.Open())
                     {

                        // LOAD IMAGE
                        using (var originalBitmap = await Android.Graphics.BitmapFactory.DecodeStreamAsync(zipEntryStream))
                        {

                           // DEFINE SIZE
                           double imageHeight = heightPixels; double imageWidth = (heightPixels/3);
                           double scaleFactor = (double)imageHeight / (double)originalBitmap.Height;
                           imageHeight = originalBitmap.Height * scaleFactor;
                           imageWidth = originalBitmap.Width * scaleFactor;

                           // INITIALIZE THUMBNAIL STREAM
                           using (var thumbnailFileStream = new System.IO.FileStream(pagePath, FileMode.CreateNew, FileAccess.Write))
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

                        zipEntryStream.Close();
                        zipEntryStream.Dispose();
                     }
                     */

                     // SIMPLE EXACT FILE 
                     if (!System.IO.File.Exists(pagePath)) {
                        using (var zipEntryStream = zipEntry.Open())
                        {
                           using (var thumbnailFile = new System.IO.FileStream(pagePath, FileMode.CreateNew, FileAccess.Write))
                           {
                              await zipEntryStream.CopyToAsync(thumbnailFile);
                              await thumbnailFile.FlushAsync();
                              thumbnailFile.Close();
                              thumbnailFile.Dispose();
                           }
                        }
                     }

                     // IMAGE SIZE
                     using (var bitmap = await Android.Graphics.BitmapFactory.DecodeFileAsync(pagePath))
                     {
                        pageData.Size = new Helpers.Controls.PageSize(bitmap.Width, bitmap.Height);
                     }

                     fileData.Pages.Add(pageData);
                     pageIndex++;

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