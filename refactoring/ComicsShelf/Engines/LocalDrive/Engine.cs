using Xamarin.Forms;

namespace ComicsShelf.Engines.LocalDrive
{
   internal partial class LocalDriveEngine : IEngine
   {

      private Helpers.IFileSystem FileSystem
      {
         get { return DependencyService.Get<Helpers.IFileSystem>(); }
      }

   }
}
