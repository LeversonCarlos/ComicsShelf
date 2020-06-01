using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async override Task<List<ComicPageVM>> ExtractPages(LibraryModel library, ComicFile comicFile)
      {
         try
         {

            // INITIALIZE
            if (!File.Exists(comicFile.FilePath)) { return null; }
            if (!Directory.Exists(comicFile.CachePath)) { Directory.CreateDirectory(comicFile.CachePath); }

            // OPEN ZIP ARCHIVE
            try
            {
               if (Directory.GetFiles(comicFile.CachePath).Count() == 0)
               {

                  var downloadStart = DateTime.Now;
                  using (var zipArchiveStream = await this.Service.Download(comicFile.Key))
                  {
                     using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Read))
                     {

                        // LOCATE PAGE ENTRIES
                        var zipEntries = zipArchive.Entries
                           .Where(x =>
                              x.Name.ToLower().EndsWith(".jpg") ||
                              x.Name.ToLower().EndsWith(".jpeg") ||
                              x.Name.ToLower().EndsWith(".png"))
                           .OrderBy(x => x.Name)
                           .ToList();
                        if (zipEntries == null) { return null; }

                        // RETRIEVE IMAGE PAGES CONTENT
                        var tasks = new List<Task>();
                        short pageIndex = 0;
                        foreach (var zipEntry in zipEntries)
                        { tasks.Add(this.ExtractPage(zipEntry, comicFile.CachePath, pageIndex++)); }
                        await Task.WhenAll(tasks.ToArray());

                     }
                  }

                  var downloadFinish = DateTime.Now;
                  Helpers.AppCenter.TrackEvent($"Comic.LocalDrive.DownloadingPages", $"ElapsedSeconds:{(downloadFinish - downloadStart).TotalSeconds}");

               }
            }
            catch (Exception exDownload) { Helpers.AppCenter.TrackEvent(exDownload); throw; }
            if (Directory.GetFiles(comicFile.CachePath).Count() == 0) { return null; }

            // LOCATE PAGE IMAGES FROM PATH
            var pages = await this.GetExtractedPagesData(comicFile.CachePath);
            return pages;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return null; }
      }

      private async Task ExtractPage(ZipArchiveEntry entry, string cachePath, short pageIndex)
      {
         try
         {

            // PAGE DATA
            var pagetText = pageIndex.ToString().PadLeft(3, "0".ToCharArray()[0]);
            var pagePath = $"{cachePath}{this.FileSystem.PathSeparator}P{pagetText}.jpg";
            if (File.Exists(pagePath)) { return; }

            // EXTRACT PAGE FILE             
            using (var zipEntryStream = entry.Open())
            {
               await this.ExtractPage(zipEntryStream, pagePath);
            }

         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

   }
}