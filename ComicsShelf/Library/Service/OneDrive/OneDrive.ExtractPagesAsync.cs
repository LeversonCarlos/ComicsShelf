using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Library.Implementation
{
   partial class OneDrive
   {

      public async Task ExtractPagesAsync(Helpers.Database.Library library, Views.File.FileData fileData)
      {
         try
         {

            // DEFINE CACHE PATHS
            var cachePath = $"{App.Settings.Paths.FilesCachePath}{App.Settings.Paths.Separator}{fileData.ComicFile.Key}";
            if (!Directory.Exists(cachePath)) { Directory.CreateDirectory(cachePath); }
            var cachePagesPath = $"{cachePath}{App.Settings.Paths.Separator}Pages";
            if (!Directory.Exists(cachePagesPath)) { Directory.CreateDirectory(cachePagesPath); }
            var cacheFilePath = $"{cachePath}{App.Settings.Paths.Separator}ComicFile.cbz";

            // DOWNLOAD COMIC FILE FROM REMOTE SERVER IF LOCAL CACHE DOESNT EXIST YET
            if (!File.Exists(cacheFilePath))
            {
               var downloadUrl = await this.Connector.GetDownloadUrlAsync(new FileData { id = fileData.ComicFile.Key });
               if (string.IsNullOrEmpty(downloadUrl)) { return; }
               using (var downloadClient = new System.Net.Http.HttpClient())
               {
                  using (var downloadStream = await downloadClient.GetStreamAsync(downloadUrl))
                  {
                     using (var fileStream = new FileStream(cacheFilePath, FileMode.CreateNew, FileAccess.ReadWrite))
                     {
                        await downloadStream.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                     }
                  }
               }
            }
            if (!File.Exists(cacheFilePath)) { return; }

            // OPEN ZIP ARCHIVE FROM LOCAL CACHE
            using (var zipArchiveStream = new System.IO.FileStream(cacheFilePath, FileMode.Open, FileAccess.Read))
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

                     // INITIALIZE PAGE DATA
                     var pageIndexText = pageIndex.ToString().PadLeft(3, "0".ToCharArray()[0]);
                     var pagePath = $"{cachePagesPath}{App.Settings.Paths.Separator}P{pageIndexText}.jpg";
                     var pageData = new Views.File.PageData { Page = pageIndex, Path = pagePath, IsVisible = false };
                     fileData.Pages.Add(pageData);
                     pageIndex++;

                     // EXTRACT PAGE 
                     if (!File.Exists(pagePath)) {
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
                     await this.FileSystem.PageSize(App.Settings, pageData);

                  }

               }
               zipArchiveStream.Close();
            }

         }
         catch (Exception) { throw; }
      }

   }
}