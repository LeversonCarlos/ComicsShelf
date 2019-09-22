using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async Task<List<ComicPageVM>> ExtractPages(LibraryModel library, ComicFile comicFile)
      {
         try
         {
            var trackProps = new Dictionary<string, string>();

            // INITIALIZE
            var pages = new List<ComicPageVM>();
            if (!Directory.Exists(comicFile.CachePath)) { Directory.CreateDirectory(comicFile.CachePath); }

            // DOWNLOAD COMIC FILE FROM REMOTE SERVER IF LOCAL CACHE DOESNT EXIST YET
            var cacheFilePath = $"{comicFile.CachePath}.zip";
            trackProps.Add("CacheFilePathExists", File.Exists(cacheFilePath).ToString());
            if (!File.Exists(cacheFilePath))
            {
               trackProps.Add("NetworkAccess", Xamarin.Essentials.Connectivity.NetworkAccess.ToString());
               if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet) { return null; }

               var downloadUrl = await this.Connector.GetDownloadUrlAsync(new FileData { id = comicFile.Key });
               if (string.IsNullOrEmpty(downloadUrl)) { return null; }

               trackProps.Add("DownloadStart", DateTime.Now.ToString("MMssfff"));
               using (var downloadClient = new System.Net.Http.HttpClient())
               {
                  using (var downloadStream = await downloadClient.GetStreamAsync(downloadUrl))
                  {
                     using (var fileStream = new FileStream(cacheFilePath, FileMode.CreateNew, FileAccess.ReadWrite))
                     {
                        await downloadStream.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                        trackProps.Add("DownloadFinish", DateTime.Now.ToString("MMssfff"));
                     }
                  }
               }
            }
            trackProps.Add("CacheFilePathExistsNow", File.Exists(cacheFilePath).ToString());
            if (!File.Exists(cacheFilePath)) { return null; }

            // OPEN ZIP ARCHIVE FROM LOCAL CACHE
            trackProps.Add("ZipStart", DateTime.Now.ToString("MMssfff"));
            using (var zipArchiveStream = new FileStream(cacheFilePath, FileMode.Open, FileAccess.Read))
            {
               using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Read))
               {

                  // LOCATE PAGE ENTRIES
                  short pageIndex = 0;
                  var zipEntries = zipArchive.Entries
                     .Where(x =>
                        x.Name.ToLower().EndsWith(".jpg") ||
                        x.Name.ToLower().EndsWith(".jpeg") ||
                        x.Name.ToLower().EndsWith(".png"))
                     .OrderBy(x => x.Name)
                     .ToList();
                  if (zipEntries == null) { return null; }
                  trackProps.Add("ZipEntriesCount", zipEntries.Count.ToString());

                  // LOOP THROUGH ZIP ENTRIES
                  foreach (var zipEntry in zipEntries)
                  {

                     // PAGE DATA
                     var page = new ComicPageVM
                     {
                        Index = pageIndex,
                        Text = pageIndex.ToString().PadLeft(3, "0".ToCharArray()[0]),
                        IsVisible = false
                     };
                     page.Path = $"{comicFile.CachePath}{this.FileSystem.PathSeparator}P{page.Text}.jpg";
                     pages.Add(page);
                     pageIndex++;

                     // EXTRACT PAGE FILE 
                     trackProps.Add($"ZipPage{pageIndex}Found", File.Exists(page.Path).ToString());
                     if (!File.Exists(page.Path))
                     {
                        using (var zipEntryStream = zipEntry.Open())
                        {
                           using (var pageStream = new FileStream(page.Path, FileMode.CreateNew, FileAccess.Write))
                           {
                              await zipEntryStream.CopyToAsync(pageStream);
                              await pageStream.FlushAsync();
                              pageStream.Close();
                           }
                           zipEntryStream.Close();
                        }
                        trackProps.Add($"ZipPage{pageIndex}FoundNow", File.Exists(page.Path).ToString());
                     }

                     // PAGE SIZE
                     var pageSize = await this.FileSystem.GetPageSize(page.Path);
                     page.PageSize = new ComicPageSize(pageSize.Width, pageSize.Height);
                     trackProps.Add($"ZipPage{pageIndex}Size", $"{page.PageSize.Height}x{page.PageSize.Width}");

                  }

               }
               zipArchiveStream.Close();
            }
            trackProps.Add("ZipFinish", DateTime.Now.ToString("MMssfff"));
            Helpers.AppCenter.TrackEvent($"Comic.OneDrive.ExtractPages", trackProps);

            return pages;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return null; }
      }

   }
}