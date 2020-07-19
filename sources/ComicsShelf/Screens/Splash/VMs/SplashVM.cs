using ComicsShelf.Helpers;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Splash
{
   public partial class SplashVM : BaseVM
   {
      IStoreService Store { get => DependencyService.Get<IStoreService>(); }
      InsightsLogger _Log { get; set; }

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
         try
         {
            _Log = new InsightsLogger($"{Store?.GetLibrary(SelectedItem?.LibraryID)?.Type} Splash Screen");
            Task.Run(async () =>
            {
               await EditionsList.ReplaceRangeAsync(EditionsArray);
               _Log?.Add("Editions", $"{EditionsArray?.Length}");

               var itemsPerLine = Helpers.Cover.ItemsPerLine;
               var editionsRows = (int)Math.Ceiling((double)EditionsList.Count / (double)itemsPerLine);
               EditionsHeight = (Helpers.Cover.DefaultHeight + (Helpers.Cover.ItemMargin * 2)) * editionsRows;

               IsBusy = false;
            });
         }
         catch (Exception ex) { _Log?.Add(ex); }
         return base.OnAppearing();
      }

      public override async Task OnDisappearing()
      {
         try
         {
            if (SelectedItem?.IsDirty ?? false)
               await Store.UpdateItemAsync(SelectedItem);
            await base.OnDisappearing();
         }
         catch (Exception ex) { _Log?.Add(ex); }
         finally { _Log?.Dispose(); }
      }

   }
}
