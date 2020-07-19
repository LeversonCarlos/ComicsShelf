using ComicsShelf.Helpers;
using ComicsShelf.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class LocalDrive
   {

      public override async Task<bool> ExtractPages(LibraryVM library, ItemVM libraryItem)
      {
         try
         {

            // DESTINATION CACHE PATH
            var cachePath = libraryItem.GetCachePath();
            if (!Directory.Exists(cachePath)) { Directory.CreateDirectory(cachePath); }

            // SOURCE FILE PATH
            var filePath = $"{libraryItem.FolderPath}/{libraryItem.FileName}";
            if (!File.Exists(filePath)) { return false; }

            // EXTRACT PAGES INTO CACHE
            if (!await this.ExtractPages(filePath, cachePath)) { return false; }

            return true;
         }
         catch (Exception ex) { Insights.TrackException(ex); return false; }
      }

      private async Task<bool> ExtractPages(string filePath, string cachePath)
      {
         try
         {
            if (Directory.GetFiles(cachePath).Count() != 0) { return true; }

            var start = DateTime.Now;
            try
            {
               using (var fileStream = await this.CloudService.Download(filePath))
               {
                  using (var fileArchive = new ZipArchive(fileStream, ZipArchiveMode.Read))
                  {

                     // LOCATE PAGE ENTRIES
                     var fileEntries = fileArchive.Entries
                        .Where(entry => ImageExtentions.Any(ext => entry.Name.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase)))
                        .OrderBy(entry => entry.Name)
                        .ToList();
                     if (fileEntries == null) { return false; }

                     // RETRIEVE IMAGE PAGES CONTENT
                     short pageIndex = 0;
                     foreach (var fileEntry in fileEntries)
                        await this.ExtractPage(fileEntry, cachePath, pageIndex++);

                  }
               }
            }
            catch (Exception exI) { Helpers.Insights.TrackException(exI); return false; }
            finally { Helpers.Insights.TrackDuration("Pages Extraction", DateTime.Now.Subtract(start).TotalSeconds); }

            return true;
         }
         catch (Exception) { throw; }
      }

      private async Task ExtractPage(ZipArchiveEntry entry, string cachePath, short pageIndex)
      {
         try
         {

            // PAGE DATA
            var pagetText = pageIndex.ToString().PadLeft(3, "0".ToCharArray()[0]);
            var pagePath = $"{cachePath}/P{pagetText}.jpg";
            if (File.Exists(pagePath)) { return; }

            // EXTRACT PAGE FILE             
            using (var zipEntryStream = entry.Open())
            {
               await this.ExtractPage(zipEntryStream, pagePath);
            }

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

   }
}
