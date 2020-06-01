using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.LocalDrive;

namespace ComicsShelf.Engines.LocalDrive
{
   internal partial class LocalDriveEngine : BaseDrive.BaseDriveEngine<LocalDriveService>
   {

      public LocalDriveEngine() : base(Store.LibraryType.LocalDrive)
      { }

   }
}
