using ComicsShelf.Observables;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Splash
{
   public partial class SplashVM : BaseVM
   {
      IStoreService Store { get => DependencyService.Get<IStoreService>(); }

      string _LibraryID { get; set; }
      public void SetLibraryID(string libraryID) => _LibraryID = libraryID;

      string _FileID { get; set; }
      public void SetFileID(string fileID) => _FileID = fileID;

      LibraryVM _Library { get; set; }
      public ObservableList<ItemVM> ItemsList { get; set; } = new ObservableList<ItemVM>();

      ItemVM _SelectedItem;
      public ItemVM SelectedItem
      {
         get => _SelectedItem;
         set => SetProperty(ref _SelectedItem, value);
      }

      public override Task OnAppearing()
      {
         LoadData();
         return base.OnAppearing();
      }

      public override Task OnDisappearing()
      {
         return base.OnDisappearing();
      }

   }
}
