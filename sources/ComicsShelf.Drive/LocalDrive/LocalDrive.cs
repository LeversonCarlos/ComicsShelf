using ComicsShelf.ViewModels;
using Xamarin.CloudDrive.Connector;

namespace ComicsShelf.Drive
{
   public partial class LocalDrive : BaseDrive<LocalDriveService>
   {
      public override enLibraryType LibraryType => enLibraryType.LocalDrive;
   }
}
