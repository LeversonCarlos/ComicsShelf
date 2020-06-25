using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;
using Xamarin.CloudDrive.Connector.OneDrive;
using Xamarin.Forms;

namespace ComicsShelf.Drive
{

   public abstract partial class BaseDrive<T> : BaseDrive, IDrive where T : ICloudDriveService
   {

      public ICloudDriveService CloudService { get { return DependencyProvider.Get<T>(); } }

      public Task<bool> ExtractCover(LibraryVM library, ItemVM libraryItem) => throw new NotImplementedException();

      public virtual string EscapeFileID(string fileID) => throw new NotImplementedException();

   }

   public class BaseDrive
   {

      public static IDrive GetDrive(enLibraryType libraryType)
      {
         try
         {
            if (libraryType == enLibraryType.OneDrive)
               return DependencyService.Get<OneDrive>();
            else if (libraryType == enLibraryType.LocalDrive)
               return DependencyService.Get<LocalDrive>();
            else
               throw new NotImplementedException($"Drive for library type {libraryType} not implemented");
         }
         catch (Exception) { throw; }
      }

   }

}
