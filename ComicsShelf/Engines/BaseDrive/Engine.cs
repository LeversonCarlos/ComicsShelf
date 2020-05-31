using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;
using Xamarin.Forms;

namespace ComicsShelf.Engines.BaseDrive
{
   internal partial class BaseDriveEngine<T> : IEngine where T : ICloudDriveService
   {

      public ICloudDriveService Service { get { return DependencyProvider.Get<T>(); } }
      readonly Store.LibraryType LibraryType;

      public BaseDriveEngine(Store.LibraryType libraryType)
      { 
         this.LibraryType = libraryType; 
      }

      protected Helpers.IFileSystem FileSystem
      {
         get { return DependencyService.Get<Helpers.IFileSystem>(); }
      }

   }
}
