using ComicsShelf.ViewModels;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class OneDrive
   {

      public override Task<bool> ExtractCover(LibraryVM library, ItemVM libraryItem) => Task.Delay(100).ContinueWith(t => true);

   }
}
