using ComicsShelf.Helpers.Observables;
using System;
using Xamarin.Forms;

namespace ComicsShelf.Main
{

   public class MenuVM : ObservableObject
   {

      public MenuVM()
      {

         this.ScreenItems = new ObservableList<MenuItem> {
            new MenuItem { Type=enumMenuItemType.Home, Text=R.Strings.HOME_MAIN_TITLE, Image="tab_feed.png" },
            new MenuItem { Type=enumMenuItemType.Library, Text="Meus Quadrinhos", Image="icon_OneDrive", Key="LibraryOne" },
            new MenuItem { Type=enumMenuItemType.Library, Text="Meus Outros Quadrinhos", Image="icon_LocalDrive", Key="LibraryTwo" }
         };

         this.CommandItems = new ObservableList<MenuItem> {
            new MenuItem { Type=enumMenuItemType.Command, Text=R.Strings.NEW_LOCAL_LIBRARY_COMMAND, Image="icon_LocalDrive_New", Key="LocalDrive" },
            new MenuItem { Type=enumMenuItemType.Command, Text=R.Strings.NEW_ONEDRIVE_LIBRARY_COMMAND, Image="icon_OneDrive_New", Key="OneDrive" },
         };

         this.ItemSelectCommand = new Command((item) => this.ItemSelect(item as MenuItem));

      }

      public ObservableList<MenuItem> ScreenItems { get; private set; }
      public ObservableList<MenuItem> CommandItems { get; private set; }

      public Command ItemSelectCommand { get; set; }
      private void ItemSelect(MenuItem item)
      {
         try
         {
            switch (item.Type)
            {
               case enumMenuItemType.Home:
                  return;
               case enumMenuItemType.Command:
                  return;
               case enumMenuItemType.Library:
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
