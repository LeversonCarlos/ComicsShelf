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
         using (var log = new Helpers.InsightsLogger("Legacy Settings"))
         {
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

                  if (legacyItem.Rating != 0)
                     libraryItem.Rating = (short)(legacyItem.Rating > 3 ? 1 : -1);
                  if (legacyItem.ReadingDate != DateTime.MinValue)
                  {
                     libraryItem.Readed = legacyItem.Readed;
                     libraryItem.ReadingDate = legacyItem.ReadingDate.ToUniversalTime();
                     libraryItem.ReadingPage = legacyItem.ReadingPage;
                     libraryItem.ReadingPercent = legacyItem.ReadingPercent;
                  }
                  if (libraryItem.IsDirty)
                     changedIDs.Add(libraryItem.ID);
               }

               // SAVE UPDATED ITEMS
               await store.UpdateItemAsync(libraryItems.Where(x => changedIDs.Contains(x.ID)).ToArray());

               // MARK ALREADY IMPORTED LEGACY SETTINGS
               if (library.KeyValues == null) library.KeyValues = new Dictionary<string, string>();
               if (library.KeyValues?.ContainsKey("ImportedLegacy") ?? false) library.KeyValues.Remove("ImportedLegacy");
               library.KeyValues.Add("ImportedLegacy", DateTime.UtcNow.ToString());
               await store.UpdateLibraryAsync(library);

            }
            catch (Exception ex) { Helpers.Insights.TrackException(ex); }
         }
      }

   }
}
