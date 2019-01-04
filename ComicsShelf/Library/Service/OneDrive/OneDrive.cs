namespace ComicsShelf.Library.Implementation
{
   internal partial class OneDrive : ILibraryService
   {

      Helpers.iFileSystem FileSystem { get; set; }
      Xamarin.OneDrive.Connector Connector { get; set; }
      public OneDrive()
      {
         this.FileSystem = Helpers.FileSystem.Get();
         var clientID = System.Environment.GetEnvironmentVariable("ComicsShelfApplicationID");
         this.Connector = new Xamarin.OneDrive.Connector(clientID, "User.Read", "Files.Read");
      }

   }
}