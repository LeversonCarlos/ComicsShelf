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

            // RETRIEVE DATA
            var libraries = store.GetLibraries();
            var sections = store.GetSections();

            // SUGGESTION ITEMS (sections without library)
            var suggestionItems = sections
               .Where(section => section.Library == null)
               .SelectMany(x => x.Folders)
               .Select(x => x.FirstItem)
               .Where(x => x.CoverPath == Helpers.Cover.DefaultCover)
               .ToArray();
            var suggestionData = suggestionItems
               .GroupBy(item => item.LibraryID)
               .Select(grp => new
               {
                  LibraryID = grp.Key,
                  Library = libraries.FirstOrDefault(x => x.ID == grp.Key),
                  Items = grp.ToArray()
               })
               .ToArray();
            foreach (var libraryData in suggestionData)
               if (!await ExtractCovers(store, libraryData.Library, libraryData.Items)) { return; }

            // FEATURED ITEMS (firstItem of the first 6 folders on each section)
            var featuredItems = sections
               .Where(section => section.Library != null)
               .SelectMany(x => x.Folders.Take(6))
               .Select(x => x.FirstItem)
               .Where(x => x.CoverPath == Helpers.Cover.DefaultCover)
               .ToArray();
            var featuredData = featuredItems
               .GroupBy(item => item.LibraryID)
               .Select(grp => new
               {
                  LibraryID = grp.Key,
                  Library = libraries.FirstOrDefault(x => x.ID == grp.Key),
                  Items = grp.ToArray()
               })
               .ToArray();
            foreach (var libraryData in featuredData)
               if (!await ExtractCovers(store, libraryData.Library, libraryData.Items)) { return; }

            // REMAINING ITEMS (all that is left)
            foreach (var library in libraries)
            {
               var remainingItems = store
                  .GetLibraryItems(library)
                  .Where(x => x.CoverPath == Helpers.Cover.DefaultCover)
                  .ToArray();
               if (!await ExtractCovers(store, library, remainingItems)) { return; }
            }

            //store.SetSections(sections);
            //Notifyers.Notify.SectionsUpdate(sections);

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

      private static async Task<bool> ExtractCovers(IStoreService store, LibraryVM library, ItemVM[] itemList)
      {
         try
         {

            // NOTIFY START MESSAGE
            Helpers.Notify.Message(library, "Start.Covers.Extraction");

            // LOOP THROUGH ITEMS
            int itemIndex = 0;
            foreach (var item in itemList)
            {
               var progress = (double)++itemIndex / (double)itemList.Length;
               Helpers.Notify.Message(library, item.FullText);
               Helpers.Notify.Progress(library, progress);

               await Task.Delay(100);
            }

            // NOTIFY FINISH MESSAGE
            Helpers.Notify.Progress(library, 1);
            Helpers.Notify.Message(library, "");

            return true;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return false; }
      }

   }
}
