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

      public SplashVM(ItemVM selectedItem, ItemVM[] editionsArray)
      {
         IsBusy = true;
         SelectedItem = selectedItem;
         EditionsArray = editionsArray;
         EditionsList = new ObservableList<ItemVM>();

         SelectItemCommand = new Command(async item => await SelectItemAsync(item));
         OpenCommand = new Command(async item => await OpenAsync());
      }

      public override Task OnAppearing()
      {
         Task.Run(() =>
         {
            EditionsList.ReplaceRange(EditionsArray);

            var itemsPerLine = Helpers.Cover.ItemsPerLine;
            var editionsRows = (int)Math.Ceiling((double)EditionsList.Count / (double)itemsPerLine);
            EditionsHeight = (Helpers.Cover.DefaultHeight + (Helpers.Cover.ItemMargin * 2)) * editionsRows;

            IsBusy = false;
         });
         return base.OnAppearing();
      }

      public override async Task OnDisappearing()
      {
         if (SelectedItem?.IsDirty ?? false)
            await Store.UpdateItemAsync(SelectedItem);
         await base.OnDisappearing();
      }

   }
}
