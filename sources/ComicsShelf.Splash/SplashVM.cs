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

      public string RatingLabel { get; private set; } = Strings.RATING_LABEL;
      public string ReadedLabel { get; private set; } = Strings.READED_LABEL;
      public string ReadingLabel { get; private set; } = Strings.READING_LABEL;
      public string OpenLabel { get; private set; } = Strings.OPEN_LABEL;
      public string EditionsTitle { get; private set; } = Strings.EDITIONS_TITLE;

      public SplashVM()
      {
         SelectItemCommand = new Command(async item => await SelectItemAsync(item));
      }

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
