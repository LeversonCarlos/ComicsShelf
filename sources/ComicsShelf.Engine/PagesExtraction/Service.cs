using ComicsShelf.Helpers;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engine.PagesExtraction
{
   public class Service
   {

      public static async Task<PageVM[]> ExecuteAsync(ItemVM libraryItem)
      {
         var start = DateTime.Now;
         try
         {

            var library = DependencyService
               .Get<IStoreService>()
               .GetLibrary(libraryItem.LibraryID);
            if (library == null) { return null; }

            var itemPages = await Drive.BaseDrive
               .GetDrive(library.Type)
               .ExtractPages(library, libraryItem);

            return itemPages;
         }
         catch (Exception ex) { Insights.TrackException(ex); return null; }
         finally { Insights.TrackMetric($"Pages Extraction", DateTime.Now.Subtract(start).TotalSeconds); }
      }

   }
}
