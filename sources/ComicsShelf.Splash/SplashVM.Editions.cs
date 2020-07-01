using ComicsShelf.Observables;
using ComicsShelf.ViewModels;

namespace ComicsShelf.Splash
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

   }
}
