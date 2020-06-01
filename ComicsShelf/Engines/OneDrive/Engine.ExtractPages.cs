using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async override Task<List<ComicPageVM>> ExtractPages(LibraryModel library, ComicFile comicFile)
      {
         try
         {

            // INITIALIZE
            if (!Directory.Exists(comicFile.CachePath)) { Directory.CreateDirectory(comicFile.CachePath); }

            // DOWNLOAD COMIC FILE FROM REMOTE SERVER IF LOCAL CACHE DOESNT EXIST YET
            try
            {
               if (Directory.GetFiles(comicFile.CachePath).Count() == 0)
               {
                  if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet) { return null; }

                  // REFRESH FILE DETAILS
                  var fileVM = await this.Service.GetDetails(comicFile.Key);
                  if (!fileVM.KeyValues.TryGetValue("downloadUrl", out string downloadUrl)) { return null; }
                  if (string.IsNullOrEmpty(downloadUrl)) { return null; }

                  // OPEN REMOTE STREAM
                  var downloadStart = DateTime.Now;
                  using (var zipStream = new System.IO.Compression.HttpZipStream(downloadUrl))
                  {

                     // STREAM SIZE
                     if (fileVM.SizeInBytes.HasValue)
                        zipStream.SetContentLength(fileVM.SizeInBytes.Value);

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

                     // RETRIEVE IMAGE PAGES CONTENT
                     var tasks = new List<Task>();
                     short pageIndex = 0;
                     foreach (var entry in entryList)
                     { tasks.Add(this.ExtractPage(zipStream, entry, comicFile.CachePath, pageIndex++)); }
                     await Task.WhenAll(tasks.ToArray());

                  }
                  var downloadFinish = DateTime.Now;
                  Helpers.AppCenter.TrackEvent($"Comic.OneDrive.DownloadingPages", $"ElapsedSeconds:{(downloadFinish - downloadStart).TotalSeconds}");

               }
            }
            catch (Exception exDownload) { Helpers.AppCenter.TrackEvent(exDownload); throw; }
            if (Directory.GetFiles(comicFile.CachePath).Count() == 0) { return null; }

            // LOCATE PAGE IMAGES FROM PATH
            var pages = await this.GetExtractedPagesData(comicFile.CachePath);
            return pages;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return null; }
         finally { GC.Collect(); }
      }

      private async Task ExtractPage(HttpZipStream zipStream, HttpZipEntry entry, string cachePath, short pageIndex)
      {
         try
         {

            // PAGE DATA
            var pagetText = pageIndex.ToString().PadLeft(3, "0".ToCharArray()[0]);
            var pagePath = $"{cachePath}{this.FileSystem.PathSeparator}P{pagetText}.jpg";
            if (File.Exists(pagePath)) { return; }

            // EXTRACT PAGE FILE 
            var pageByteArray = await zipStream.ExtractAsync(entry);
            if (pageByteArray != null && pageByteArray.Length != 0)
            {

               using (var pageMemoryStream = new MemoryStream(pageByteArray))
               {
                  await pageMemoryStream.FlushAsync();
                  pageMemoryStream.Position = 0;
                  await this.ExtractPage(pageMemoryStream, pagePath);
               }

            }

         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

   }
}