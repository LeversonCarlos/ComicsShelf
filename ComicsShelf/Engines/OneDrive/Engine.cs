using Xamarin.CloudDrive.Connector.OneDrive;

namespace ComicsShelf.Engines.OneDrive
{
   internal partial class OneDriveEngine : BaseDrive.BaseDriveEngine<OneDriveService>
   {

      public OneDriveEngine() : base(Store.LibraryType.OneDrive)
      { }

      public override string EscapeFileID(string fileID) => fileID;

   }
}
