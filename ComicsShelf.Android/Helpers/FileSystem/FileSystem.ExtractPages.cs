using ComicsShelf.ComicFiles;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<ComicFiles.ComicPagesVM> ExtractPages(Libraries.LibraryModel library, ComicFile comicFile)
      {
         var result = new ComicFiles.ComicPagesVM();
         try
         {

            if (!File.Exists(comicFile.FilePath)) { return result; }
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
                  if (zipEntries == null) { return result; }

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
                     page.Path = $"{comicFile.CachePath}{this.PathSeparator}P{page.Text}.jpg";
                     result.Pages.Add(page);
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
                           zipEntryStream.Dispose();
                        }
                     }

                     // PAGE SIZE
                     using (var bitmap = await Android.Graphics.BitmapFactory.DecodeFileAsync(page.Path))
                     {
                        page.PageSize = new ComicPageSize(bitmap.Width, bitmap.Height);
                     }

                  }

                  zipArchive.Dispose();
               }

               zipArchiveStream.Close();
               zipArchiveStream.Dispose();
            }

            return result;
         }
         catch (Exception) { throw; }
      }

   }
}