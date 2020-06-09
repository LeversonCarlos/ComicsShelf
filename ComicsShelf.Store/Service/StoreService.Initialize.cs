using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      public const string LibraryInitializeMessage = "Library.Initialize.Message";
      const string LibraryIDs = "ComicsShelf.LibraryIDs";
      LibraryStoreVM[] LibraryList { get; set; }

      public async Task<bool> InitializeAsync()
      {
         try
         {

            // LIBRARY IDs
            var libraryIDsJSON = Xamarin.Essentials.Preferences.Get(LibraryIDs, "[]");
            var libraryIDs = await Base.Helpers.Json.Deserialize<string[]>(libraryIDsJSON);

            // LIBRARY CONTENT
            var libraryList = new List<LibraryStoreVM>();
            foreach (var libraryID in libraryIDs)
            {
               var libraryJSON = Xamarin.Essentials.Preferences.Get($"{LibraryIDs}.{libraryID}", "");
               if (!string.IsNullOrEmpty(libraryJSON))
               {
                  var library = await Base.Helpers.Json.Deserialize<LibraryStoreVM>(libraryJSON);
                  libraryList.Add(library);
               }
            }
            this.LibraryList = libraryList.ToArray();

            // NOTIFY LIBRARY INITIALIZATION
            var taskList = this.LibraryList
               .Select(x => new Task(() => Base.Helpers.Messaging.Send(LibraryInitializeMessage, x.ID)))
               .ToArray();
            await Task.WhenAll(taskList);

            return true;
         }
         catch (Exception) { throw; }
      }

   }
}
