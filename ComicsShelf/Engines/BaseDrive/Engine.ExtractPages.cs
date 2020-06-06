using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

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

            // LOCATE PAGE FILES FROM PATH
            var fileList = await Task.FromResult(Directory.GetFiles(cachePath));

            // FILTER EXTENTION AND CONVERT TO MODEL
            var extentions = new string[] { ".jpg", ".jpeg", ".png" };
            var pages = fileList
               .Where(file => extentions.Any(ext => file.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase)))
               .OrderBy(x => x)
               .Select(pagePath => new ComicPageVM
               {
                  Text = Path.GetFileNameWithoutExtension(pagePath),
                  Path = pagePath,
                  IsVisible = false
               })
               .ToList();

            // TRY TO LOCATE PREVIOUS PAGES SETTINGS
            var pageSizesKey = $"{cachePath}{this.FileSystem.PathSeparator}PagesSize.json";
            var pageSizes = new ComicPageVM[] { };
            try
            {
               var pageSizesContent = await Xamarin.Essentials.SecureStorage.GetAsync(pageSizesKey);
               if (!string.IsNullOrEmpty(pageSizesContent))
               { pageSizes = await Helpers.Json.Deserialize<ComicPageVM[]>(pageSizesContent); }
            }
            catch (Exception pageSizesException)
            {
               Helpers.AppCenter.TrackEvent(pageSizesException);
               pageSizes = new ComicPageVM[] { };
            }

            var getPageSize = new Func<ComicPageVM, Task<ComicPageSize>>(async page =>
            {
               try
               {
                  var pageSize = pageSizes.Where(p => p.Index == page.Index).Select(p => p.PageSize).FirstOrDefault();
                  if (pageSize == null)
                  {
                     var size = await this.FileSystem.GetPageSize(page.Path);
                     pageSize = new ComicPageSize(size.Width, size.Height);
                  }
                  return pageSize;
               }
               catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return null; }
            });

            // LOOP THROUGH PAGES
            short pageIndex = 0;
            foreach (var page in pages)
            {
               page.Index = pageIndex++;
               page.Text = page.Text.Substring(1);
               page.PageSize = await getPageSize(page);
            }

            // STORE PAGE SETTINGS FOR FUTURE USE
            try
            {
               var pageSizesObject = pages
                  .Select(p => new
                  {
                     p.Index,
                     p.PageSize
                  })
                  .ToArray();
               var pageSizesContent = await Helpers.Json.Serialize(pageSizesObject);
               await Xamarin.Essentials.SecureStorage.SetAsync(pageSizesKey, pageSizesContent);
            }
            catch (Exception pageSizesException)
            {
               Helpers.AppCenter.TrackEvent(pageSizesException);
               pageSizes = new ComicPageVM[] { };
            }

            // RESULT
            return pages;
         }
         catch (Exception ex) { throw new Exception($"Error extracting pages data on directory [{cachePath}].", ex); }
      }

   }
}