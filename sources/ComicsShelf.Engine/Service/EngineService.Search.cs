using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engine
{
   partial class EngineService
   {

      public static async Task<bool> SearchFiles(LibraryVM library)
      {
         try
         {

            // NOTIFY START MESSAGE
            Notifyers.Notify.Message(library, R.Search.START_MESSAGE);

            // SEARCH FILES THROUGH THE LIBRARY's ENGINE
            var searchItems = await Drive.BaseDrive
               .GetDrive(library.Type)
               .SearchItems(library);
            Notifyers.Notify.Message(library, string.Format(R.Search.FOUND_N_FILES_MESSAGE, (searchItems?.Length ?? 0).ToString()));

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
            { Notifyers.Notify.Message(library, string.Format(R.Search.INACTIVATED_N_FILES_MESSAGE, inactiveItems.Count)); }

            // UPDATE CHANGED FILES ON THE LIBRARY
            var changedItems = searchItems
               .Where(x => !libraryItems.Select(i => i.ID).ToList().Contains(x.ID))
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
            { Notifyers.Notify.Message(library, string.Format(R.Search.ADDED_N_NEW_FILES_MESSAGE, newItems.Count)); }

            // NOTIFY EMPTY MESSAGE
            if (inactiveItems.Count == 0 && newItems.Count == 0)
            { Notifyers.Notify.Message(library, ""); }

            // NOTIFY THAT LIBRARY FILES WAS UPDATED 
            // Notify.Result(library, store.GetLibraryItems(library));

            return true;
         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); return false; }
      }

   }
}
