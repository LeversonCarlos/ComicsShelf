using ComicsShelf.Helpers.Observables;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Main
{

   public class MenuVM : BaseVM
   {

      public MenuVM()
      {

         this.ScreenItems = new ObservableList<MenuItem> {
            new MenuItem { Type=enumMenuItemType.Home, Text=R.Strings.HOME_MAIN_TITLE, Image="icon_home.png" }
         };
         this.CommandItems = new ObservableList<MenuItem> {
            new MenuItem { Type=enumMenuItemType.Command, Text=R.Strings.NEW_LOCAL_LIBRARY_COMMAND, Image="icon_LocalDrive_New", Key="LocalDrive" },
            new MenuItem { Type=enumMenuItemType.Command, Text=R.Strings.NEW_ONEDRIVE_LIBRARY_COMMAND, Image="icon_OneDrive_New", Key="OneDrive" },
         };
         this.ItemSelectCommand = new Command(async (item) => await this.ItemSelect(item as MenuItem));

         Messaging.Subscribe<Store.LibraryModel>(Messaging.enMessages.LibraryAdded.ToString(), (library) =>
         {
            this.ScreenItems.Add(new MenuItem
            {
               Key = library.ID,
               Type = enumMenuItemType.Library,
               Image = $"icon_{library.Type.ToString()}",
               Text = library.Description
            });
         });

         Messaging.Subscribe<Store.LibraryModel>(Messaging.enMessages.LibraryRemoved.ToString(), async (library) =>
         {
            var screenItem = this.ScreenItems.Where(x => x.Type == enumMenuItemType.Library && x.Key == library.ID).FirstOrDefault();
            if (screenItem != null)
            {
               this.ScreenItems.Remove(screenItem);
               await App.Navigation().PopToRootAsync();
            }
         });

      }

      public ObservableList<MenuItem> ScreenItems { get; private set; }
      public ObservableList<MenuItem> CommandItems { get; private set; }

      public Command ItemSelectCommand { get; set; }
      private async Task ItemSelect(MenuItem item)
      {
         try
         {
            switch (item.Type)
            {
               case enumMenuItemType.Home:
                  await App.Navigation().PopToRootAsync();
                  App.HideNavigation();
                  return;
               case enumMenuItemType.Command:
                  var libraryType = (item.Key == "OneDrive" ? Store.LibraryType.OneDrive : Store.LibraryType.LocalDrive);
                  await DependencyService.Get<Store.ILibraryStore>().NewLibraryAsync(libraryType);                  
                  return;
               case enumMenuItemType.Library:
                  var library = DependencyService.Get<Store.ILibraryStore>().GetLibrary(item.Key);
                  var vm = new Library.LibraryVM(library);
                  var page = new Library.LibraryPage { BindingContext = vm };
                  await App.Navigation().PushAsync(page);
                  App.HideNavigation();
                  return;
            }
         }
         catch (Exception) { throw; }
      }


   }

   public enum enumMenuItemType : short { Home, Library, Command };
   public class MenuItem
   {
      public enumMenuItemType Type { get; set; }
      public string Key { get; set; }
      public string Image { get; set; }
      public string Text { get; set; }
   }

}
