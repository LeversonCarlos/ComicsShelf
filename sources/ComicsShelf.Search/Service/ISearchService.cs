using ComicsShelf.Store;
using System.Threading.Tasks;

namespace ComicsShelf.Search
{
   public interface ISearchService
   {

      Task<bool> SearchFiles(LibraryStoreVM library);

   }
}
