using System;
using System.Collections.Generic;
using System.Text;

namespace ComicsShelf.Libraries
{
   internal class LibraryStore
   {
      private const string LibraryIDs = "ComicsShelf.LibraryIDs";

      public void AddLibrary(LibraryModel library)
      {
         try
         {
            var libraryIDs = this.GetLibraryIDs();
            library.ID = Guid.NewGuid().ToString();

            var libraryID = $"{LibraryIDs}.{library.ID}";
            this.SetLibrary(library, libraryID);

            libraryIDs.Add(libraryID);
            this.SetLibraryIDs(libraryIDs);
         }
         catch (Exception) { throw; }
      }

      public void SetLibrary(LibraryModel library)
      { this.SetLibrary(library, $"{LibraryIDs}.{library.ID}"); }
      private void SetLibrary(LibraryModel library, string libraryID)
      {
         try
         {
            var libraryJSON = Newtonsoft.Json.JsonConvert.SerializeObject(library);
            Xamarin.Essentials.Preferences.Set(libraryID, libraryJSON);
         }
         catch (Exception) { throw; }
      }

      public void DeleteLibrary(LibraryModel library)
      {
         try
         {
            var libraryID = $"{LibraryIDs}.{library.ID}";
            Xamarin.Essentials.Preferences.Remove(libraryID);

            var libraryIDs = this.GetLibraryIDs();
            libraryIDs.Remove(library.ID);
            this.SetLibraryIDs(libraryIDs);
         }
         catch (Exception) { throw; }
      }

      public List<LibraryModel> GetLibraries()
      {
         try
         {
            var libraryList = new List<LibraryModel>();
            var libraryIDs = this.GetLibraryIDs();
            foreach (var libraryID in libraryIDs)
            {
               try
               {
                  var libraryJSON = Xamarin.Essentials.Preferences.Get(libraryID, "");
                  if (!string.IsNullOrEmpty(libraryJSON))
                  {
                     var library = Newtonsoft.Json.JsonConvert.DeserializeObject<LibraryModel>(libraryJSON);
                     libraryList.Add(library);
                  }
               }
               catch { }
            }
            return libraryList;
         }
         catch (Exception) { throw; }
      }

      private List<string> GetLibraryIDs()
      {
         try
         {
            var libraryIDsJSON = Xamarin.Essentials.Preferences.Get(LibraryIDs, "[]");
            var libraryIDs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(libraryIDsJSON);
            return libraryIDs;
         }
         catch (Exception) { throw; }
      }

      private void SetLibraryIDs(List<string> libraryIDs)
      {
         try
         {
            var libraryIDsJSON = Newtonsoft.Json.JsonConvert.SerializeObject(libraryIDs);
            Xamarin.Essentials.Preferences.Set(LibraryIDs, libraryIDsJSON);
         }
         catch (Exception) { throw; }
      }


   }
}
