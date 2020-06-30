using ComicsShelf.Observables;
using ComicsShelf.Store;
using Xamarin.Forms;

namespace ComicsShelf.Reading
{
   public partial class ReadingVM : BaseVM
   {

      IStoreService Store { get => DependencyService.Get<IStoreService>(); }

   }
}
