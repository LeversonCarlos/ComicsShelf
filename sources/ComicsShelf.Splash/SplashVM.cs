using ComicsShelf.Observables;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Splash
{
   public class SplashVM : BaseVM
   {
      public IStoreService Store { get => DependencyService.Get<IStoreService>(); }

      string _LibraryID { get; set; }
      public void SetLibraryID(string libraryID) => _LibraryID = libraryID;

      string _FileID { get; set; }
      public void SetFileID(string fileID) => _FileID = fileID;

      LibraryVM _Library { get; set; }
      void LoadLibrary()
      {
         if (_Library != null) { return; }
         if (string.IsNullOrEmpty(_LibraryID)) { return; }
         _Library = Store.GetLibrary(_LibraryID);
      }

      public ObservableList<ItemVM> ItemsList { get; set; } = new ObservableList<ItemVM>();
      void LoadItems()
      {
         if (ItemsList.Count != 0) { return; }
         if (string.IsNullOrEmpty(_LibraryID)) { return; }
         if (string.IsNullOrEmpty(_FileID)) { return; }

         var libraryItems = Store.GetLibraryItems(_Library);
         if (libraryItems?.Length == 0) { return; }

         var libraryItem = libraryItems.Where(x => x.EscapedID == _FileID).FirstOrDefault();
         if (libraryItem == null) { return; }

         var folderItems = libraryItems
            .Where(item => item.FolderPath == libraryItem.FolderPath)
            .OrderBy(item => item.FullText)
            .ToArray();
         ItemsList.AddRange(folderItems);
      }

      public override Task OnAppearing()
      {
         LoadLibrary();
         LoadItems();
         return base.OnAppearing();
      }

      public override Task OnDisappearing()
      {
         return base.OnDisappearing();
      }

   }
}
