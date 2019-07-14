namespace ComicsShelf.Engines.OneDrive
{
   public class OneDriveConnector : Xamarin.OneDrive.Connector
   {
      public OneDriveConnector() : base(AppKeys.OneDriveApplicationID, "User.Read", "Files.ReadWrite") { }
   }
}
