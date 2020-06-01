using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.BaseDrive
{
   partial class BaseDriveEngine<T>
   {

      public virtual Task<List<ComicPageVM>> ExtractPages(LibraryModel library, ComicFile comicFile) => throw new NotImplementedException();

      protected async Task ExtractPage(Stream zipEntryStream, string pagePath)
      {
         try
         {
            using (var pageStream = new FileStream(pagePath, FileMode.CreateNew, FileAccess.Write))
            {
               await zipEntryStream.CopyToAsync(pageStream);
               await pageStream.FlushAsync();
               pageStream.Close();
            }
         }
         catch (Exception) { throw; }
      }

      protected async Task<List<ComicPageVM>> GetExtractedPagesData(string cachePath)
      {
         try
         {

            // LOCATE PAGE IMAGES FROM PATH
            var pages = Directory.GetFiles(cachePath)
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
            short pageIndex = 0;
            foreach (var page in pages)
            {
               page.Index = pageIndex++;
               page.Text = page.Text.Substring(1);
               var pageSize = await this.FileSystem.GetPageSize(page.Path);
               page.PageSize = new ComicPageSize(pageSize.Width, pageSize.Height);
            }

            // RESULT
            return pages;
         }
         catch (Exception) { throw; }
      }

   }
}