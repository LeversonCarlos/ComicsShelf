using ComicsShelf.Libraries;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engines.LocalDrive
{
   internal partial class LocalDriveEngine : IEngine
   {

      public Task<bool> RemoveLibrary(LibraryModel library)
      {
         throw new NotImplementedException();
      }

      private Helpers.IFileSystem FileSystem
      {
         get { return DependencyService.Get<Helpers.IFileSystem>(); }
      }

   }
}
