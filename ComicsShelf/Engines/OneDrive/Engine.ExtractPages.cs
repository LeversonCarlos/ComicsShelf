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

            // INITIALIZE
            short pageIndex = 0;
            if (!Directory.Exists(comicFile.CachePath)) { Directory.CreateDirectory(comicFile.CachePath); }

            // DOWNLOAD COMIC FILE FROM REMOTE SERVER IF LOCAL CACHE DOESNT EXIST YET
            try
            {
               if (Directory.GetFiles(comicFile.CachePath).Count() == 0)
               {
                  if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet) { return null; }
                  var downloadUrl = await this.Connector.GetDownloadUrlAsync(new FileData { id = comicFile.Key });
                  if (string.IsNullOrEmpty(downloadUrl)) { return null; }

                  // OPEN REMOTE STREAM
                  var downloadStart = DateTime.Now;
                  using (var zipStream = new System.IO.Compression.HttpZipStream(downloadUrl))
                  {

                     // STREAM SIZE
                     var streamSizeValue = comicFile.GetKeyValue("StreamSize");
                     if (!string.IsNullOrEmpty(streamSizeValue))
                     {
                        long streamSize;
                        if (long.TryParse(streamSizeValue, out streamSize))
                        { zipStream.SetContentLength(streamSize); }
                     }

                     // FIRST ENTRY
                     var entryList = await zipStream.GetEntriesAsync();
                     entryList.RemoveAll(x =>
                        !x.FileName.ToLower().EndsWith(".jpg") &&
                        !x.FileName.ToLower().EndsWith(".jpeg") &&
                        !x.FileName.ToLower().EndsWith(".png"));
                     var entries = entryList
                        .OrderBy(x => x.FileName)
                        .ToList();
                     if (entries == null) { return null; }

                     // RETRIEVE REMOTE IMAGE CONTENT
                     var tasks = new List<Task>();
                     pageIndex = 0;
                     foreach (var entry in entryList)
                     {
                        tasks.Add(this.ExtractPage(zipStream, entry, comicFile.CachePath, pageIndex++));
                     }
                     await Task.WhenAll(tasks.ToArray());

                  }
                  var downloadFinish = DateTime.Now;
                  Helpers.AppCenter.TrackEvent($"Comic.OneDrive.DownloadingPages", $"ElapsedSeconds:{(downloadFinish - downloadStart).TotalSeconds}");

               }
            }
            catch (Exception exDownload) { Helpers.AppCenter.TrackEvent(exDownload); throw; }
            if (Directory.GetFiles(comicFile.CachePath).Count() == 0) { return null; }

            // LOCATE PAGE IMAGES FROM PATH
            var pages = Directory.GetFiles(comicFile.CachePath)
               .Where(x =>
                  x.ToLower().EndsWith(".jpg") ||
                  x.ToLower().EndsWith(".jpeg") ||
                  x.ToLower().EndsWith(".png"))
               .OrderBy(x => x)
               .Select(pagePath => new ComicPageVM
               {
                  Text = Path.GetFileNameWithoutExtension(pagePath),
                  Path = pagePath,
                  IsVisible = false
               })
               .ToList();

            // LOOP THROUGH PAGES
            pageIndex = 0;
            foreach (var page in pages)
            {
               page.Index = pageIndex++;
               page.Text = page.Text.Substring(1);
               var pageSize = await this.FileSystem.GetPageSize(page.Path);
               page.PageSize = new ComicPageSize(pageSize.Width, pageSize.Height);
            }

            return pages;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return null; }
         finally { GC.Collect(); }
      }

      private async Task ExtractPage(HttpZipStream zipStream, HttpZipEntry entry, string cachePath, short pageIndex)
      {
         try
         {

            var pagetText = pageIndex.ToString().PadLeft(3, "0".ToCharArray()[0]);
            var pagePath = $"{cachePath}{this.FileSystem.PathSeparator}P{pagetText}.jpg";

            if (!File.Exists(pagePath))
            {

               var pageByteArray = await zipStream.ExtractAsync(entry);
               if (pageByteArray != null && pageByteArray.Length != 0)
               {

                  using (var pageMemoryStream = new MemoryStream(pageByteArray))
                  {
                     await pageMemoryStream.FlushAsync();
                     pageMemoryStream.Position = 0;
                     using (var pageStream = new FileStream(pagePath, FileMode.CreateNew, FileAccess.Write))
                     {
                        await pageMemoryStream.CopyToAsync(pageStream);
                        await pageStream.FlushAsync();
                        pageStream.Close();
                     }
                     pageMemoryStream.Close();
                  }

               }

            }

         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

   }
}