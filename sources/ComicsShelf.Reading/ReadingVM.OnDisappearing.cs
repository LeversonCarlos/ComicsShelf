using System.Threading.Tasks;

namespace ComicsShelf.Reading
{
   partial class ReadingVM
   {

      public override Task OnDisappearing()
      {
         return Store.UpdateItemAsync(this.Item).ContinueWith(t => base.OnDisappearing());
      }

   }
}
