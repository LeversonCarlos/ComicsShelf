using ComicsShelf.ViewModels;
using System;
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
         set
         {
            SetProperty(ref _SelectedItem, value);
            IsBusy = true;
            Rating = _SelectedItem.Rating;
            Readed = _SelectedItem.Readed;
            IsBusy = false;
         }
      }

      short? _Rating;
      public short? Rating
      {
         get => _Rating;
         set
         {
            SetProperty(ref _Rating, value);
            if (!IsBusy)
            {
               _SelectedItem.Rating = value;
            }
         }
      }

      bool _Readed;
      public bool Readed
      {
         get => _Readed;
         set
         {
            SetProperty(ref _Readed, value);
            if (!IsBusy)
            {
               _SelectedItem.Readed = value;
               _SelectedItem.ReadingPage = 0;
               _SelectedItem.ReadingPercent = (value ? 1 : 0);
               _SelectedItem.ReadingDate = (value ? DateTime.UtcNow : DateTime.MinValue);
            }
         }
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
