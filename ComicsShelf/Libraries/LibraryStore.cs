using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ComicsShelf.Libraries
{
   internal class LibraryStore
   {
      private const string LibraryIDs = "ComicsShelf.LibraryIDs";


      public void AddLibrary(LibraryModel library)
      {
         try
         {

            // STORE LIBRARY MODEL
            library.ID = Guid.NewGuid().ToString();
            var libraryID = $"{LibraryIDs}.{library.ID}";
            this.SetLibrary(library, libraryID);

            // STORE IDs LIST
            var libraryIDs = this.GetLibraryIDs();
            libraryIDs.Add(libraryID);
            this.SetLibraryIDs(libraryIDs);

            // UPDATE SHELL
            var shellItem = this.AddLibraryShell(library);
            Shell.CurrentShell.CurrentItem = shellItem;
            Shell.CurrentShell.FlyoutIsPresented = false;

         }
         catch (Exception) { throw; }
      }

      private ShellItem AddLibraryShell(LibraryModel library)
      {
         try
         {

            var shellContent = new ShellContent
            {
               Title = library.Description,
               BindingContext = new LibraryVM(library),
               ContentTemplate = new DataTemplate(typeof(LibraryPage))
            };

            var shellSection = new ShellSection { Title = library.Description };
            shellSection.Items.Add(shellContent);

            var shellItem = new ShellItem { Title = library.Description, Icon = $"icon_{library.Type.ToString()}.png" };
            shellItem.Items.Add(shellSection);

            Shell.CurrentShell.Items.Add(shellItem);
            return shellItem;

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


      public void DeleteLibrary(ShellItem libraryShell)
      {
         try
         {

            // REMOVE LIBRARY MODEL
            var libraryVM = libraryShell.Items[0].Items[0].BindingContext as LibraryVM;
            var library = libraryVM.Library;
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
         catch (Exception) { throw; }
      }

      public void LoadLibraries()
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
                     this.AddLibraryShell(library);
                  }
               }
               catch { }
            }
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
