using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class Sync
   {
      static string library_sync(string libraryID) => $"{base_sync()}.Library.{libraryID}";

      public static async Task<LibraryVM> GetLibrary(string libraryID)
      {
         try
         {
            var libraryJSON = Xamarin.Essentials.Preferences.Get(library_sync(libraryID), "");
            if (string.IsNullOrEmpty(libraryJSON)) { return null; }

            var library = await Helpers.Json.Deserialize<LibraryVM>(libraryJSON);

            return library;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return null; }
      }

      public static async Task<bool> SetLibrary(LibraryVM library)
      {
         try
         {
            var libraryJson = await Helpers.Json.Serialize(library);
            Xamarin.Essentials.Preferences.Set(library_sync(library.ID), libraryJson);
            return true;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return false; }
      }

      public static async Task<bool> RemoveLibrary(LibraryVM library)
      {
         try
         {
            RemoveItems(library.ID, await GetItems(library.ID));
            Xamarin.Essentials.Preferences.Remove(library_sync(library.ID));
            await Task.CompletedTask;
            return true;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return false; }
      }

   }
}
