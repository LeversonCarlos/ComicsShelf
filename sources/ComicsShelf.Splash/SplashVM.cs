using ComicsShelf.Observables;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Splash
{
   public partial class SplashVM : BaseVM
   {
      IStoreService Store { get => DependencyService.Get<IStoreService>(); }

      public SplashVM()
      {
         SelectItemCommand = new Command(async item => await SelectItemAsync(item));
      }

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

      public Command SelectItemCommand { get; set; }
      Task SelectItemAsync(object item)
      {
         UpdateItemAsync();
         this.SelectedItem = (item as ItemVM);
         return Task.CompletedTask;
      }

      private async void UpdateItemAsync()
      {
         if (!this.SelectedItem.IsDirty) { return; }
         await DependencyService.Get<IStoreService>().UpdateItemAsync(this.SelectedItem);
      }

      public override Task OnAppearing()
      {
         LoadData();
         return base.OnAppearing();
      }

      public override Task OnDisappearing()
      {
         UpdateItemAsync();
         return base.OnDisappearing();
      }

   }
}
