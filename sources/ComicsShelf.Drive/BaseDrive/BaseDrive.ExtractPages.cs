using ComicsShelf.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T>
   {

      public virtual Task<PageVM[]> ExtractPages(LibraryVM library, ItemVM libraryItem) => throw new NotImplementedException();

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

      protected async Task<PageVM[]> GetExtractedPagesData(string cachePath)
      {
         try
         {

            // LOCATE PAGE FILES FROM PATH
            var fileList = await Task.FromResult(Directory.GetFiles(cachePath));

            // FILTER EXTENTION AND CONVERT TO MODEL
            var pages = fileList
               .Where(file => ImageExtentions.Any(ext => file.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase)))
               .OrderBy(pagePath => pagePath)
               .Select(pagePath => new PageVM
               {
                  Text = Path.GetFileNameWithoutExtension(pagePath),
                  Path = pagePath,
                  IsVisible = false
               })
               .ToArray();

            // TRY TO LOCATE PREVIOUS PAGES SETTINGS
            var pageSizesKey = $"{cachePath}/PagesSize.json";
            var pageSizes = new PageVM[] { };
            try
            {
               var pageSizesContent = await Xamarin.Essentials.SecureStorage.GetAsync(pageSizesKey);
               if (!string.IsNullOrEmpty(pageSizesContent))
               { pageSizes = await Helpers.Json.Deserialize<PageVM[]>(pageSizesContent); }
            }
            catch (Exception pageSizesException)
            {
               Helpers.Insights.TrackException(pageSizesException);
               pageSizes = new PageVM[] { };
            }

            var getPageSize = new Func<PageVM, Task<PageSizeVM>>(async page =>
            {
               try
               {
                  var pageSize = pageSizes.Where(p => p.Index == page.Index).Select(p => p.PageSize).FirstOrDefault();
                  if (pageSize == null)
                  {
                     var size = await this.FileSystem.GetPageSize(page.Path);
                     pageSize = new PageSizeVM { Width = size.Width, Height = size.Height };
                  }
                  return pageSize;
               }
               catch (Exception ex) { Helpers.Insights.TrackException(ex); return null; }
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
               Helpers.Insights.TrackException(pageSizesException);
               pageSizes = new PageVM[] { };
            }

            // RESULT
            return pages;
         }
         catch (Exception ex) { throw new Exception($"Error extracting pages data on directory [{cachePath}].", ex); }
      }

   }
}
