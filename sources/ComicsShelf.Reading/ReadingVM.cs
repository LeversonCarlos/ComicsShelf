using ComicsShelf.Observables;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System.Linq;
using Xamarin.Forms;

namespace ComicsShelf.Reading
{
   public partial class ReadingVM : BaseVM
   {

      public ReadingVM(ItemVM item, PageVM[] pagesList)
      {
         IsBusy = true;
         Item = item;
         PagesList = new ObservableList<PageVM>();
         PagesArray = pagesList.Union(new PageVM[] { new PageVM { } }).ToArray();

         // pagesList = pagesList.Union(new PageVM[] { new PageVM { } }).ToArray();
         // ReadingPage = Item.ReadingPage.HasValue ? Item.ReadingPage.Value : (short)0;
      }

      IStoreService Store { get => DependencyService.Get<IStoreService>(); }
      readonly PageVM[] PagesArray;
      public ItemVM Item { get; }
      public ObservableList<PageVM> PagesList { get; }

   }
}
