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

      public async Task<List<ComicPageVM>> ExtractPages(LibraryModel library, ComicFile comicFile)
      {
         try
         {

            // INITIALIZE
            var pages = new List<ComicPageVM>();
            if (!File.Exists(comicFile.FilePath)) { return null; }
            if (!Directory.Exists(comicFile.CachePath)) { Directory.CreateDirectory(comicFile.CachePath); }

            // OPEN ZIP ARCHIVE
            using (var zipArchiveStream = new FileStream(comicFile.FilePath, FileMode.Open, FileAccess.Read))
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
                     }

                     // PAGE SIZE
                     var pageSize = await this.FileSystem.GetPageSize(page.Path);
                     page.PageSize = new ComicPageSize(pageSize.Width, pageSize.Height);

                  }

               }
               zipArchiveStream.Close();
            }

            return pages;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return null; }
      }

   }
}