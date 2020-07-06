using ComicsShelf.ViewModels;
using Xamarin.CloudDrive.Connector;

namespace ComicsShelf.Drive
{
   public partial class OneDrive : BaseDrive<OneDriveService>
   {
      public override enLibraryType LibraryType => enLibraryType.OneDrive;
   }
}
