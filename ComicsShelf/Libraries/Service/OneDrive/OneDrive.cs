namespace ComicsShelf.Libraries.Implementation
{
   internal partial class OneDrive : ILibraryService
   {
      public const string LibraryFileID = "LibraryFileID";

      Helpers.iFileSystem FileSystem { get; set; }
      Xamarin.OneDrive.Connector Connector { get; set; }
      public OneDrive()
      {
         this.FileSystem = Helpers.FileSystem.Get();
         var clientID = "{YOUR_MICROSOFT_APPLICATION_ID}";
         this.Connector = new Xamarin.OneDrive.Connector(clientID, "User.Read", "Files.ReadWrite");
      }

   }
}