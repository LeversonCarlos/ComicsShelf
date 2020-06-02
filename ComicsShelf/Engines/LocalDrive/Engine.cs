using Xamarin.CloudDrive.Connector.LocalDrive;

namespace ComicsShelf.Engines.LocalDrive
{
   internal partial class LocalDriveEngine : BaseDrive.BaseDriveEngine<LocalDriveService>
   {

      public LocalDriveEngine() : base(Store.LibraryType.LocalDrive)
      { }

      public override string EscapeFileID(string fileID) => fileID
         .Replace("#", "")
            .Replace(".", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace(" ", "")
            .Replace("/", "_")
            .Replace("___", "_")
            .Replace("__", "_");

   }
}
