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
            Helpers.Notify.Message(library, Strings.CONNECTING_MESSAGE);

            // SEARCH FILES THROUGH THE LIBRARY's ENGINE
            var searchItems = await Drive.BaseDrive
               .GetDrive(library.Type)
               .SearchItems(library);
            Helpers.Notify.Message(library, string.Format(Strings.FOUND_N_FILES_MESSAGE, (searchItems?.Length ?? 0).ToString()));

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
            { Helpers.Notify.Message(library, string.Format(Strings.REMOVED_N_FILES_MESSAGE, inactiveItems.Count)); }

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
            { Helpers.Notify.Message(library, string.Format(Strings.ADDED_N_NEW_FILES_MESSAGE, newItems.Count)); }

            // NOTIFY EMPTY MESSAGE
            if (inactiveItems.Count == 0 && newItems.Count == 0)
            { Helpers.Notify.Message(library, ""); }

            // NOTIFY THAT LIBRARY FILES WAS UPDATED 
            // Notify.Result(library, store.GetLibraryItems(library));

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); Helpers.Notify.Message(library, $"Error while refreshing data: {ex.Message}"); }
         finally { Helpers.Insights.TrackEvent($"Refreshing Data", $"Seconds:{Math.Round(DateTime.Now.Subtract(start).TotalSeconds, 0)}"); }
      }

   }
}
