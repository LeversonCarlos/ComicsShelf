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

      public SplashVM()
      {
         SelectItemCommand = new Command(async item => await SelectItemAsync(item));
      }

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

      double _EditionsHeight;
      public double EditionsHeight
      {
         get => _EditionsHeight;
         set => SetProperty(ref _EditionsHeight, value);
      }

      private async void UpdateItemAsync()
      {
         if (!this.SelectedItem.IsDirty) { return; }
         await Store.UpdateItemAsync(this.SelectedItem);
      }

      public override Task OnAppearing()
      {
         InitializeData();
         return base.OnAppearing();
      }

      public override Task OnDisappearing()
      {
         UpdateItemAsync();
         return base.OnDisappearing();
      }

   }
}
