using System.Threading.Tasks;

namespace ComicsShelf.Engines.OneDrive
{
   public partial class OneDriveConnector : Xamarin.OneDrive.Connector
   {
      public OneDriveConnector() : base(OneDriveConnector.onedrive_applicationID, "User.Read", "Files.ReadWrite") { }

      public async Task<bool> TryConnectAsync()
      {
         try
         { return await this.ConnectAsync(); }
         catch { return false; }
      }

   }
}
