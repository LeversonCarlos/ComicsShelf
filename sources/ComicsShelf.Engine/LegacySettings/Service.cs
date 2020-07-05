using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engine.LegacySettings
{
   public class Service
   {

      public static Task Execute(LibraryVM library) => Task.Factory.StartNew(() => ExecuteAsync(library), TaskCreationOptions.LongRunning);
      private static async Task ExecuteAsync(LibraryVM library)
      {
         var start = DateTime.Now;
         try
         {

            // CHECK IF ALREADY IMPORTED LEGACY SETTINGS
            if (library.KeyValues?.ContainsKey("ImportedLegacy") ?? false) return;
            var store = DependencyService.Get<IStoreService>();
            var changedIDs = new List<string>();

            // SEARCH FOR THE LEGACY ITEMS
            var legacyItems = await Drive.BaseDrive
               .GetDrive(library.Type)
               .SearchLegacySettings(library);
            if (legacyItems == null || legacyItems.Length == 0) return;

            // RETRIEVE THE LIBRARY ITEMS
            var libraryItems = store.GetLibraryItems(library);

            // LOOP THROUGH LEGACY ITEMS
            foreach (var legacyItem in legacyItems)
            {
               var libraryItem = libraryItems.Where(x => x.ID == legacyItem.Key).FirstOrDefault();
               if (libraryItem == null) continue;

               libraryItem.Rating = legacyItem.Rating;
               libraryItem.Readed = legacyItem.Readed;
               libraryItem.ReadingDate = legacyItem.ReadingDate;
               libraryItem.ReadingPage = legacyItem.ReadingPage;
               libraryItem.ReadingPercent = legacyItem.ReadingPercent;
               changedIDs.Add(libraryItem.ID);
            }

            // SAVE UPDATED ITEMS
            await store.UpdateItemAsync(libraryItems.Where(x => changedIDs.Contains(x.ID)).ToArray());

            // MARK ALREADY IMPORTED LEGACY SETTINGS
            if (library.KeyValues == null) library.KeyValues = new Dictionary<string, string>();
            library.KeyValues.Add("ImportedLegacy", DateTime.UtcNow.ToString());
            await store.UpdateLibraryAsync(library);

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); Helpers.Notify.Message(library, $"Error while importing legacy settings: {ex.Message}"); }
         finally { Helpers.Insights.TrackMetric($"Legacy Settings", DateTime.Now.Subtract(start).TotalSeconds); }
      }

   }
}
