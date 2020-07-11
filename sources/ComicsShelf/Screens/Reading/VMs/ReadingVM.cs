using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Reading
{
   public partial class ReadingVM : BaseVM
   {
      IStoreService Store { get => DependencyService.Get<IStoreService>(); }

      public ReadingVM(ItemVM item, PageVM[] pagesList)
      {
         IsBusy = true;
         Item = item;
         PagesList = new ObservableList<PageVM>();
         PagesArray = pagesList.Union(new PageVM[] { new PageVM { } }).ToArray();
      }

      readonly PageVM[] PagesArray;
      public ItemVM Item { get; }
      public ObservableList<PageVM> PagesList { get; }

      PageSizeVM _ScreenSize;
      public PageSizeVM ScreenSize
      {
         get => _ScreenSize;
         set
         {
            SetProperty(ref _ScreenSize, value);
            ScrollComplete = false;
            IsSwipeEnabled = GetSwipeEnabled();
         }
      }

      public override Task OnAppearing()
      {
         Helpers.Notify.AppSleep(this, async now => await Helpers.Modal.Pop());
         Task.Run(async () =>
         {
            await PagesList.ReplaceRangeAsync(PagesArray);
            ReadingPage = Item.ReadingPage.HasValue ? Item.ReadingPage.Value : (short)0;
            IsBusy = false;
         });
         return base.OnAppearing();
      }

      public override async Task OnDisappearing()
      {
         Helpers.Notify.AppSleepUnsubscribe(this);
         await Store.UpdateItemAsync(this.Item);
      }

   }
}
