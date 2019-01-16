namespace ComicsShelf.Library.Implementation
{
   internal partial class OneDrive : ILibraryService
   {

      Helpers.iFileSystem FileSystem { get; set; }
      Xamarin.OneDrive.Connector Connector { get; set; }
      public OneDrive()
      {
         this.FileSystem = Helpers.FileSystem.Get();
         var clientID = "97a605b6-d44d-40fe-8e6a-4cbb326ea580"; //System.Environment.GetEnvironmentVariable("ComicsShelfApplicationID");
         this.Connector = new Xamarin.OneDrive.Connector(clientID, "User.Read", "Files.Read");
      }

   }
}