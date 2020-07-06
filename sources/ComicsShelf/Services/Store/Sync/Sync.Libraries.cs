using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class Sync
   {
      static string libraries_sync() => $"{base_sync()}.Libraries";

      public static async Task<string[]> GetLibraries()
      {
         try
         {
            var libraryIDsJSON = Xamarin.Essentials.Preferences.Get(libraries_sync(), "[]");
            var libraryIDs = await Helpers.Json.Deserialize<string[]>(libraryIDsJSON);
            return libraryIDs;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return new string[] { }; }
      }

      public static async Task<bool> SetLibraries(LibraryVM[] libraryList)
      {
         try
         {
            var libraryIDs = libraryList.Select(x => x.ID).ToArray();
            var libraryIDsJson = await Helpers.Json.Serialize(libraryIDs);
            Xamarin.Essentials.Preferences.Set(libraries_sync(), libraryIDsJson);
            return true;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return false; }
      }

   }
}
