using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engine.RefreshData
{
   public class Service
   {

      public static Task Execute(LibraryVM library) => Task.Factory.StartNew(() => ExecuteAsync(library), TaskCreationOptions.LongRunning);

      private static async Task ExecuteAsync(LibraryVM library)
      {
         var start = DateTime.Now;
         try
         {

            // NOTIFY START MESSAGE
            Helpers.Notify.Message(library, Resources.Translations.ENGINE_REFRESH_CONNECTING_DRIVE_SERVICE_MESSAGE);

            // SEARCH FILES THROUGH THE LIBRARY's ENGINE
            var searchItems = await Drive.BaseDrive
               .GetDrive(library.Type)
               .SearchItems(library);
            Helpers.Notify.Message(library, string.Format(Resources.Translations.ENGINE_REFRESH_FOUND_N_FILES_MESSAGE, (searchItems?.Length ?? 0).ToString()));

            // LOAD CURRENT LIBRARY FILES
            var store = DependencyService.Get<Store.IStoreService>();
            var libraryItems = store.GetLibraryItems(library);

            // INACTIVATE LIBRARY FILES THAT ISNT FOUND BY THE ENGINE ANYMORE
            var inactiveItems = libraryItems
               .Where(x => !searchItems.Select(i => i.ID).ToList().Contains(x.ID))
               .ToList();
            inactiveItems.ForEach(file => file.Available = false);
            await store.UpdateItemAsync(inactiveItems.ToArray());
            if (inactiveItems.Count > 0)
            { Helpers.Notify.Message(library, string.Format(Resources.Translations.ENGINE_REFRESH_REMOVED_N_FILES_MESSAGE, inactiveItems.Count)); }

            // UPDATE CHANGED FILES ON THE LIBRARY
            var changedItems = searchItems
               .Where(x => libraryItems.Select(i => i.ID).ToList().Contains(x.ID))
               .ToList();
            foreach (var changedItem in changedItems)
               changedItem.SetData(libraryItems.Where(x => x.ID == changedItem.ID).FirstOrDefault());
            await store.UpdateItemAsync(changedItems.ToArray());

            // ADD NEW FILES TO THE LIBRARY
            var newItems = searchItems
               .Where(x => !libraryItems.Select(i => i.ID).ToList().Contains(x.ID))
               .ToList();
            await store.UpdateItemAsync(newItems.ToArray());
            if (newItems.Count > 0)
            { Helpers.Notify.Message(library, string.Format(Resources.Translations.ENGINE_REFRESH_ADDED_N_NEW_FILES_MESSAGE, newItems.Count)); }

            // NOTIFY EMPTY MESSAGE
            if (inactiveItems.Count == 0 && newItems.Count == 0)
            { Helpers.Notify.Message(library, ""); }

            // RETRIEVE LIBRARY DATA
            Helpers.Notify.LibraryData(library);

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); Helpers.Notify.Message(library, $"Error while refreshing data: {ex.Message}"); }
         finally { Helpers.Insights.TrackMetric($"Refreshing Data", DateTime.Now.Subtract(start).TotalSeconds); }
      }

   }
}
