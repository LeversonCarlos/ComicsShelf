using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Screens.Splash
{
   partial class SplashVM
   {

      readonly ItemVM[] EditionsArray;
      public ObservableList<ItemVM> EditionsList { get; }

      double _EditionsHeight;
      public double EditionsHeight
      {
         get => _EditionsHeight;
         set => SetProperty(ref _EditionsHeight, value);
      }

      public async Task NextEdition()
      {
         try
         {

            if (SelectedItem == null)
               return;

            if (SelectedItem.IsDirty)
               await Store.UpdateItemAsync(SelectedItem);

            var nextItem = EditionsList
               .Where(item => String.Compare(item.FullText, SelectedItem.FullText) > 0)
               .OrderBy(item => item.FullText)
               .Take(1)
               .FirstOrDefault();

            if (nextItem == null)
               nextItem = EditionsList
                  .OrderBy(item => item.FullText)
                  .Take(1)
                  .FirstOrDefault();

            if (nextItem == null)
               return;

            SelectedItem = nextItem;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

   }
}
