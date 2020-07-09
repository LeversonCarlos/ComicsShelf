using ComicsShelf.Services.Hub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      public Task Initialize() => Task.Factory.StartNew(() => InitializeAsync(), TaskCreationOptions.LongRunning);

      public async Task<bool> InitializeAsync()
      {
         var start = DateTime.Now;
         try
         {

            var libraryIDs = await Sync.GetLibraries();

            var dataTasks = libraryIDs
               .GroupBy(x => x)
               .Select(x => x.Key)
               .Select(libraryID => Sync.GetLibrary(libraryID))
               .ToArray();
            this.LibraryList = (await Task.WhenAll(dataTasks))
               .Where(x => x != null)
               .ToArray();

            foreach (var library in this.LibraryList)
            {

               var itemIDs = await Sync.GetItems(library.ID);

               var itemTasks = itemIDs
                  .Select(itemID => Sync.GetItem(itemID))
                  .ToArray();
               var itemList = (await Task.WhenAll(itemTasks))
                  .Where(x => x != null)
                  .ToArray();
               itemList.ForEach(item => item.CoverPath = Helpers.Cover.DefaultCover);

               this.ItemList.Add(library.ID, new SortedList<string, ViewModels.ItemVM>(itemList.ToDictionary(k => k.ID, v => v)));

               Helpers.Notify.LibraryAdd(library);
               Helpers.Notify.ItemsUpdate(itemList);
            }

            return true;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return false; }
         finally { Helpers.Insights.TrackMetric($"Store Initializing", DateTime.Now.Subtract(start).TotalSeconds); }
      }

   }
}
