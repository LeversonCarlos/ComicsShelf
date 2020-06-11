using ComicsShelf.Store;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Drive
{
   partial class DriveService
   {

      public async Task<bool> SearchFiles(LibraryStoreVM library)
      {
         try
         {

            // NOTIFY START MESSAGE
            Notify.SearchProgress(library, "drive_search_progress_start");
            var engine = Engine.GetEngine(library);
            var store = DependencyService.Get<IStoreService>();

            // SEARCH FILES THROUGH THE LIBRARY's ENGINE
            var searchItems = await engine.SearchFiles(library);
            Notify.SearchProgress(library, "drive_search_progress_files_found", (searchItems?.Length ?? 0).ToString());

            // LOAD CURRENT LIBRARY FILES
            var libraryItems = store.GetLibraryItems(library);

            // INACTIVATE LIBRARY FILES THAT ISNT FOUND BY THE ENGINE ANYMORE
            var inactiveItems = libraryItems
               .Where(x => !searchItems.Select(i => i.ID).ToList().Contains(x.ID))
               .ToList();
            inactiveItems.ForEach(file => file.Available = false);
            await store.UpdateItemAsync(inactiveItems.ToArray());
            if (inactiveItems.Count > 0)
            { Notify.SearchProgress(library, "drive_search_progress_inactivated_files", (inactiveItems.Count).ToString()); }

            // UPDATE CHANGED FILES ON THE LIBRARY
            var newItems = searchItems
               .Where(x => !libraryItems.Select(i => i.ID).ToList().Contains(x.ID))
               .ToList();
            await store.UpdateItemAsync(searchItems.ToArray());
            if (newItems.Count > 0)
            { Notify.SearchProgress(library, "drive_search_progress_new_files", (newItems.Count).ToString()); }

            // NOTIFY FINISH MESSAGE
            Notify.SearchProgress(library, "drive_search_progress_finish");

            // NOTIFY THAT LIBRARY FILES WAS UPDATED (IF SO)


            await Task.CompletedTask;
            return true;

         }
         catch (Exception ex) { ComicsShelf.Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
