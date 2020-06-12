using ComicsShelf.Store;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   public interface IDriveService
   {

      Task<bool> SearchFiles(LibraryStoreVM library);

   }
}
