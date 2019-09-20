using Xamarin.Forms;

namespace ComicsShelf.Engines.OneDrive
{
   internal partial class OneDriveEngine : IEngine
   {

      private Helpers.IFileSystem FileSystem
      {
         get { return DependencyService.Get<Helpers.IFileSystem>(); }
      }

      private OneDriveConnector Connector
      {
         get { return DependencyService.Get<OneDriveConnector>(); }
      }

   }
}
