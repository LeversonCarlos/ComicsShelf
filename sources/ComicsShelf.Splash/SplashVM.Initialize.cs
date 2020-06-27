using ComicsShelf.Observables;
using ComicsShelf.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;

namespace ComicsShelf.Splash
{
   partial class SplashVM
   {

      public void InitializeData()
      {
         try
         {
            this.InitializeLibrary()
                .InitializeItems()
                .InitializeSelectedItem();
         }
         catch (Exception ex) { Helpers.Message.Show(ex); Helpers.Modal.Pop(); }
      }

      LibraryVM _Library { get; set; }
      SplashVM InitializeLibrary()
      {
         if (string.IsNullOrEmpty(_LibraryID)) { throw new Exception($"No libraryID parameter was defined for the loading of the splash screen"); }
         if (_Library != null) { return this; }
         _Library = Store.GetLibrary(_LibraryID);
         if (_Library == null) { throw new Exception($"Could not load the library for the libraryID {_LibraryID} on the splash screen"); }
         return this;
      }

      public ObservableList<ItemVM> ItemsList { get; set; } = new ObservableList<ItemVM>();
      SplashVM InitializeItems()
      {
         if (string.IsNullOrEmpty(_FileID)) { throw new Exception($"No fileID parameter was defined for the loading of the splash screen"); }
         if (ItemsList.Count != 0) { return this; }

         var libraryItems = Store.GetLibraryItems(_Library);
         if (libraryItems?.Length == 0) { throw new Exception($"Could not load items list for the libraryID {_LibraryID} on the splash screen"); }

         var libraryItem = libraryItems.Where(x => x.EscapedID == _FileID).FirstOrDefault();
         if (libraryItem == null) { throw new Exception($"Could not locate the item {_FileID} for the libraryID {_LibraryID} on the splash screen"); }

         var folderItems = libraryItems
            .Where(item => item.FolderPath == libraryItem.FolderPath)
            .OrderBy(item => item.FullText)
            .ToArray();
         if (folderItems?.Length == 0) { throw new Exception($"Could not locate items with folder path {libraryItem.FolderPath} for the libraryID {_LibraryID} on the splash screen"); }

         ItemsList.AddRange(folderItems);
         if (ItemsList.Count == 0) { throw new Exception($"No items is loaded for the libraryID {_LibraryID} on the splash screen"); }

         var editionsColumns = Device.Idiom == TargetIdiom.Phone ? 3 : Device.Idiom == TargetIdiom.Tablet ? 5 : Device.Idiom == TargetIdiom.Desktop ? 7 : 9;
         var editionsRows = (int)Math.Ceiling((double)ItemsList.Count / (double)editionsColumns);
         EditionsHeight = (Helpers.Cover.DefaultHeight + (Helpers.Cover.ItemMargin * 2)) * editionsRows;

         return this;
      }

      SplashVM InitializeSelectedItem()
      {
         if (string.IsNullOrEmpty(_FileID)) { throw new Exception($"No fileID parameter was defined for the loading of the splash screen"); }
         if (ItemsList.Count == 0) { throw new Exception($"No items is loaded for the libraryID {_LibraryID} on the splash screen"); }

         SelectedItem = ItemsList.Where(x => x.EscapedID == _FileID).FirstOrDefault();
         if (SelectedItem == null) { throw new Exception($"Could not locate the item {_FileID} for the libraryID {_LibraryID} on the splash screen"); }

         return this;
      }

   }
}
