using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;
using Xamarin.Forms;

namespace ComicsShelf.Engines.BaseDrive
{
   internal partial class BaseDriveEngine<T> : IEngine where T : ICloudDriveService
   {

      public ICloudDriveService Service { get { return DependencyProvider.Get<T>(); } }

      private Helpers.IFileSystem FileSystem
      {
         get { return DependencyService.Get<Helpers.IFileSystem>(); }
      }

      public async Task<bool> TryConnectAsync()
      {
         try
         { return await this.Service.ConnectAsync(); }
         catch { return false; }
      }

   }
}
