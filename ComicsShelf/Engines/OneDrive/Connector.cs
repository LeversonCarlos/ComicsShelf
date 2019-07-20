using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.OneDrive
{
   public class OneDriveConnector : Xamarin.OneDrive.Connector
   {
      public OneDriveConnector() : base(AppKeys.OneDriveApplicationID, "User.Read", "Files.ReadWrite") { }

      public async Task<bool> TryConnectAsync()
      {
         try
         {
            return await this.ConnectAsync();
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return false; }
      }

   }
}
