using ComicsShelf.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Splash
{
   partial class SplashVM
   {

      ItemVM _SelectedItem;
      public ItemVM SelectedItem
      {
         get => _SelectedItem;
         set => SetProperty(ref _SelectedItem, value);
      }

      public Command SelectItemCommand { get; }

      async Task SelectItemAsync(object item)
      {
         if (SelectedItem?.IsDirty ?? false)
            await Store.UpdateItemAsync(SelectedItem);
         SelectedItem = (item as ItemVM);
      }

   }
}
