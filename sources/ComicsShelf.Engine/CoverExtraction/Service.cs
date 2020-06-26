using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engine.CoverExtraction
{
   public class Service
   {

      public static Task Execute() => Task.Factory.StartNew(() => ExecuteAsync(), TaskCreationOptions.LongRunning);

      private static async Task ExecuteAsync()
      {
         try
         {
            var store = DependencyService.Get<IStoreService>();

            // LOCATE FIRST 6 ITEMS FROM EACH SECTION AS THOSE ARE THE ONES THAT FIRST APPEARS TO USER
            var sections = store.GetSections();
            var featuredItems = sections
               .SelectMany(x => x.Folders.Take(6))
               .Select(x => x.FirstItem)
               .Where(x => x.CoverPath == DefaultCover.DefaultCover.Path)
               .ToArray();

            // GROUP BY LIBRARY SO WE COULD GET A NICE PROGRESS ON THE SETTINGS PAGE FOR THOSE
            var featuredLibraryIDs = featuredItems
               .GroupBy(x => x.LibraryID)
               .Select(x => x.Key)
               .ToArray();
            foreach (var libraryID in featuredLibraryIDs)
            {
               var libraryfeaturedItems = featuredItems.Where(x => x.LibraryID == libraryID).ToArray();
               if (!await ExtractCovers(store, libraryID, libraryfeaturedItems)) { return; }
            }

            // LOCATE ALL REMAINING ITEMS
            var remainingItems = sections
               .SelectMany(x => x.Folders)
               .SelectMany(x => x.Items)
               .Where(x => x.CoverPath == DefaultCover.DefaultCover.Path)
               .ToArray();

            // GROUP BY LIBRARY SO WE COULD GET A NICE PROGRESS ON THE SETTINGS PAGE FOR THOSE
            var remainingLibraryIDs = remainingItems
               .GroupBy(x => x.LibraryID)
               .Select(x => x.Key)
               .ToArray();
            foreach (var libraryID in remainingLibraryIDs)
            {
               var libraryfeaturedItems = remainingItems.Where(x => x.LibraryID == libraryID).ToArray();
               if (!await ExtractCovers(store, libraryID, libraryfeaturedItems)) { return; }
            }

            /*
            var libraries = store.GetLibraries();
            var libraryItems = libraries
               .Select(x => store.GetLibraryItems(x))
               .SelectMany(x => x)
               .ToList();
            */

            //store.SetSections(sections);
            //Notifyers.Notify.SectionsUpdate(sections);

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

      private static async Task<bool> ExtractCovers(IStoreService store, string libraryID, ItemVM[] itemList)
      {
         try
         {

            var library = store.GetLibrary(libraryID);

            // NOTIFY START MESSAGE
            Notifyers.Notify.Message(library, "Start.Covers.Extraction");

            // LOOP THROUGH ITEMS
            int itemIndex = 0;
            foreach (var item in itemList)
            {
               var progress = (double)++itemIndex / (double)itemList.Length;
               Notifyers.Notify.Message(library, item.FullText);
               Notifyers.Notify.Progress(library, progress);

               await Task.Delay(100);
            }

            // NOTIFY FINISH MESSAGE
            Notifyers.Notify.Progress(library, 1);
            Notifyers.Notify.Message(library, "");

            return true;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return false; }
      }

   }
}
