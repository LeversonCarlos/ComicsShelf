using ComicsShelf.Libraries;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine 
   {

      public async Task<bool> Validate(LibraryModel library)
      {
         return await this.FileSystem.Validate(library.Key);
      }

   }
}
