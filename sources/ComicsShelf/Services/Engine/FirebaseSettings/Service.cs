using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engine.FirebaseSettings
{
   public class Service
   {

      public static Task Execute(LibraryVM library) => Task.Factory.StartNew(() => ExecuteAsync(library), TaskCreationOptions.LongRunning);
      private static async Task ExecuteAsync(LibraryVM library)
      {
         var start = DateTime.Now;
         try
         {

            // INITIALIZE
            var store = DependencyService.Get<IStoreService>();
            var changedIDs = new List<string>();

            // SEARCH FOR THE FIREBASE SETTINGS
            var firebaseItems = await DependencyService
               .Get<Firebase.FirebaseService>()
               .GetItemsAsync(library); ;
            if (firebaseItems == null || firebaseItems.Length == 0) return;

            // RETRIEVE THE LIBRARY ITEMS
            var libraryItems = store.GetLibraryItems(library);

            // LOOP THROUGH LEGACY ITEMS
            foreach (var firebaseItem in firebaseItems)
            {
               var libraryItem = libraryItems.Where(x => x.ID == firebaseItem.ID).FirstOrDefault();
               if (libraryItem == null) continue;

               if (firebaseItem.Rating.HasValue)
                  libraryItem.Rating = firebaseItem.Rating;
               if (firebaseItem.Readed.HasValue)
                  libraryItem.Readed = firebaseItem.Readed.Value;
               if (firebaseItem.ReadingDate.HasValue)
                  libraryItem.ReadingDate = firebaseItem.ReadingDate;
               if (firebaseItem.ReadingPage.HasValue)
               {
                  libraryItem.ReadingPage = firebaseItem.ReadingPage;
                  libraryItem.ReadingPercent = firebaseItem.ReadingPercent;
               }
               if (libraryItem.IsDirty)
                  changedIDs.Add(libraryItem.ID);
            }

            // SAVE UPDATED ITEMS
            await store.UpdateItemAsync(libraryItems.Where(x => changedIDs.Contains(x.ID)).ToArray());

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
         finally { Helpers.Insights.TrackMetric($"Firebase Settings", DateTime.Now.Subtract(start).TotalSeconds); }
      }

   }
}
