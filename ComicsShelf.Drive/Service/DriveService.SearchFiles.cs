using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class DriveService
   {

      public async Task<bool> SearchFiles(LibraryStoreVM library)
      {
         try
         {

            // NOTIFY START MESSAGE
            Notify.SearchStart(library, "drive_search_progress_start");

            // SEARCH FILES THROUGH THE LIBRARY's ENGINE

            // LOAD CURRENT LIBRARY FILES

            // REMOVE-INACTIVE LIBRARY FILES THAT ISNT RETURNED BY THE ENGINE ANYMORE

            // UPDATE ATTRIBUTES OF ALREADY EXISTING FILES

            // ADD NEW FILES TO THE LIBRARY

            // CHECK FOR POSSIBLE DUPLICITY

            // NOTIFY FINISH MESSAGE
            Notify.SearchStart(library, "drive_search_progress_finish");

            // NOTIFY THAT LIBRARY FILES WAS UPDATED (IF SO)


            await Task.CompletedTask;
            return true;

         }
         catch (Exception ex) { ComicsShelf.Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
