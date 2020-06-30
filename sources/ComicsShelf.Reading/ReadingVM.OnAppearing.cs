using System.Threading.Tasks;

namespace ComicsShelf.Reading
{
   partial class ReadingVM
   {

      public override Task OnAppearing()
      {
         Task.Run(() =>
         {
            PagesList.ReplaceRange(PagesArray);
            ReadingPage = Item.ReadingPage.HasValue ? Item.ReadingPage.Value : (short)0;
            IsBusy = false;
         });
         return base.OnAppearing();
      }

   }
}
