using ComicsShelf.ViewModels;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{

   partial class BaseDrive<T>
   {

      public virtual Task<bool> ExtractCover(LibraryVM library, ItemVM libraryItem) => throw new NotImplementedException();

   }

}
