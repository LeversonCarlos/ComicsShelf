using ComicsShelf.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class OneDrive
   {

      public override async Task<bool> ExtractPages(LibraryVM library, ItemVM libraryItem)
      {
         var start = DateTime.Now;
         try
         {

            // CHECK SERVICE
            if (!await this.CloudService.CheckConnectionAsync()) return false;

            // DESTINATION CACHE PATH
            var cachePath = libraryItem.GetCachePath();
            if (!Directory.Exists(cachePath)) { Directory.CreateDirectory(cachePath); }
            if (Directory.GetFiles(cachePath).Count() != 0) return true;

            // REFRESH FILE DETAILS
            var fileVM = await this.CloudService.GetDetails(libraryItem.ID);
            if (!fileVM.KeyValues.TryGetValue("downloadUrl", out string downloadUrl)) return false;
            if (string.IsNullOrEmpty(downloadUrl)) return false;

            // OPEN REMOTE STREAM
            using (var zipStream = new System.IO.Compression.HttpZipStream(downloadUrl))
            {

               // STREAM SIZE
               if (fileVM.SizeInBytes.HasValue)
                  zipStream.SetContentLength(fileVM.SizeInBytes.Value);

               // PAGES ENTRIES
               var entryList = await zipStream.GetEntriesAsync();
               entryList.RemoveAll(entry => !ImageExtentions.Any(ext => entry.FileName.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase)));
               var entries = entryList
                  .OrderBy(entry => entry.FileName)
                  .ToList();
               if (entries == null || entries.Count == 0) { return false; }

               // RETRIEVE IMAGE PAGES CONTENT
               var tasks = new List<Task>();
               short pageIndex = 0;
               foreach (var entry in entryList)
               { tasks.Add(this.ExtractPage(zipStream, entry, cachePath, pageIndex++)); }
               await Task.WhenAll(tasks.ToArray());

            }

            if (Directory.GetFiles(cachePath).Count() == 0) return false;
            return true;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return false; }
         finally { Helpers.Insights.TrackDuration("Pages Extraction", DateTime.Now.Subtract(start).TotalSeconds); }
      }

      private async Task ExtractPage(HttpZipStream zipStream, HttpZipEntry entry, string cachePath, short pageIndex)
      {
         try
         {

            // PAGE DATA
            var pagetText = pageIndex.ToString().PadLeft(3, "0".ToCharArray()[0]);
            var pagePath = $"{cachePath}/P{pagetText}.jpg";
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
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

   }
}
