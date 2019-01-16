using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.UWP
{
   partial class FileSystem
   {

      public async Task PagesExtract(Views.File.FileData fileData)
      {
         try
         {
            var settings = ComicsShelf.App.Settings;

            // DEFINE CACHE PATHS
            var cachePath = $"{settings.Paths.FilesCachePath}{settings.Paths.Separator}{fileData.ComicFile.Key}";
            cachePath = $"{cachePath}{settings.Paths.Separator}Pages";
            if (!Directory.Exists(cachePath)) { Directory.CreateDirectory(cachePath); }

            // OPEN ZIP ARCHIVE
            var comicStorageFile = await GetStorageFile(fileData.ComicFile.LibraryPath, fileData.FullPath);
            using (var zipArchiveStream = await comicStorageFile.OpenStreamForReadAsync())
            {
               using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Read))
               {

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
                     if (!File.Exists(pagePath))
                     {
                        using (var zipEntryStream = zipEntry.Open())
                        {
                           using (var pageStream = new FileStream(pagePath, FileMode.CreateNew, FileAccess.Write))
                           {
                              await zipEntryStream.CopyToAsync(pageStream);
                              await pageStream.FlushAsync();
                              pageStream.Close();
                           }
                        }
                     }
                     await this.PageSize(pageData);

                  }

               }

               zipArchiveStream.Close();
            }

         }
         catch (Exception) { throw; }
      }

      public async Task PageSize(Views.File.PageData pageData)
      {
         try
         {
            var pageStorageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(pageData.Path);
            var pageProperties = await pageStorageFile.Properties.GetImagePropertiesAsync();
            pageData.Size = new Helpers.Controls.PageSize(pageProperties.Width, pageProperties.Height);
            pageProperties = null;
            pageStorageFile = null;
         }
         catch (Exception) { throw; }
      }

   }
}