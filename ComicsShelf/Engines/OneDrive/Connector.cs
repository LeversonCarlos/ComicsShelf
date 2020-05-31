using System;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;
using Xamarin.CloudDrive.Connector.OneDrive;

namespace ComicsShelf.Engines.OneDrive
{
   public partial class OneDriveConnector : IDisposable
   {
      internal OneDriveService Service { get { return DependencyProvider.Get<OneDriveService>(); } }

      public async Task<bool> TryConnectAsync()
      {
         try
         { return await this.Service.ConnectAsync(); }
         catch { return false; }
      }

      public void Dispose()
      {
         // throw new NotImplementedException();
      }

   }
}
