using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.UWP
{
   partial class FileSystem
   {

      public async Task PagesExtract(Helpers.Settings.Settings settings, Views.File.FileData fileData)
      {
         try
         {

            // OPEN ZIP ARCHIVE
            var comicStorageFile = await GetStorageFile(settings, fileData.FullPath);
            using (var zipArchiveStream = await comicStorageFile.OpenStreamForReadAsync())
            {
               using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Read))
               {

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
                     fileData.Pages.Add(pageData);
                     pageIndex++;

                     // EXTRACT PAGE IMAGE
                     if (!System.IO.File.Exists(pagePath))
                     {
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
                     /*
                     using (var bitmap = await System.Drawing.Image(pagePath))
                     {
                        pageData.Size = new Xamarin.Forms.Size(bitmap.Width, bitmap.Height);
                     }
                     */

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