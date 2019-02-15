namespace ComicsShelf.Libraries.Implementation
{
   internal partial class OneDrive : ILibraryService
   {
      public const string LibraryFileID = "LibraryFileID";
      public const string LibraryPath = "LibraryPath";

      Helpers.iFileSystem FileSystem { get; set; }
      Xamarin.OneDrive.Connector Connector { get; set; }
      public OneDrive()
      {
         this.FileSystem = Helpers.FileSystem.Get();
         this.Connector = new Xamarin.OneDrive.Connector(AppKeys.OneDriveApplicationID, "User.Read", "Files.ReadWrite");
      }

   }
}