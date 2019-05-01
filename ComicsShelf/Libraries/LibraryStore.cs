using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Libraries
{
   internal class LibraryStore
   {
      private const string LibraryIDs = "ComicsShelf.LibraryIDs";


      public async Task NewLibrary(LibraryType libraryType)
      {
         try
         {

            // CREATE NEW LIBRARY THRUGH ENGINE
            var engine = Engines.Engine.Get(libraryType);
            if (engine == null) { return; }
            var library = await engine.NewLibrary();
            if (library == null) { return; }

            // STORE LIBRARY MODEL
            library.ID = Guid.NewGuid().ToString();
            var libraryID = $"{LibraryIDs}.{library.ID}";
            this.SetLibrary(library, libraryID);

            // STORE IDs LIST
            var libraryIDs = this.GetLibraryIDs();
            libraryIDs.Add(libraryID);
            this.SetLibraryIDs(libraryIDs);

            // UPDATE SHELL
            var shellItem = LibraryService.Add(library);
            Shell.CurrentShell.CurrentItem = shellItem;
            Shell.CurrentShell.FlyoutIsPresented = false;

            // STARTUP LIBRARY
            LibraryService.StartupLibrary(library);

         }
         catch (Exception ex) { await App.ShowMessage(ex); }
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


      public async Task DeleteLibrary(ShellItem libraryShell)
      {
         try
         {

            // LOCATE LIBRARY MODEL
            var libraryVM = libraryShell.Items[0].Items[0].BindingContext as LibraryVM;
            var library = libraryVM.Library;

            // DELETE LIBRARY THROUGH ENGINE
            var engine = Engines.Engine.Get(library.Type);
            if (engine != null)
            { await engine.DeleteLibrary(library); }

            // DELETE LIBRARY FROM PREFERENCES
            var libraryID = $"{LibraryIDs}.{library.ID}";
            Xamarin.Essentials.Preferences.Remove(libraryID);

            // UPDATE IDs LIST
            var libraryIDs = this.GetLibraryIDs();
            libraryIDs.Remove(library.ID);
            this.SetLibraryIDs(libraryIDs);

            // UPDATE SHELL
            Shell.CurrentShell.CurrentItem = Shell.CurrentShell.Items[0];
            Shell.CurrentShell.Items.Remove(libraryShell);

         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      public async Task LoadLibraries()
      {
         try
         {
            var libraryIDs = this.GetLibraryIDs();
            foreach (var libraryID in libraryIDs)
            {
               try
               {
                  var libraryJSON = Xamarin.Essentials.Preferences.Get(libraryID, "");
                  if (!string.IsNullOrEmpty(libraryJSON))
                  {
                     var library = Newtonsoft.Json.JsonConvert.DeserializeObject<LibraryModel>(libraryJSON);
                     LibraryService.Add(library);
                     LibraryService.StartupLibrary(library);
                  }
               }
               catch { }
            }
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
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
