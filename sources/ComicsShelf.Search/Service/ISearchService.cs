using ComicsShelf.ViewModels;
using System.Threading.Tasks;

namespace ComicsShelf.Search
{
   public interface ISearchService
   {

      Task<bool> SearchFiles(LibraryVM library);

   }
}
