using ComicsShelf.Libraries;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engines.LocalDrive
{
   internal partial class LocalDriveEngine : IEngine
   {

      public async Task<bool> DeleteLibrary(LibraryModel library)
      {
         return await Task.FromResult(true);
      }

      private Helpers.IFileSystem FileSystem
      {
         get { return DependencyService.Get<Helpers.IFileSystem>(); }
      }

   }
}
