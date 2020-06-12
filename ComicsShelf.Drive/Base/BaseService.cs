using ComicsShelf.Store;
using System;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;
using Xamarin.Forms;

namespace ComicsShelf.Drive
{

   public abstract partial class BaseService<T> : BaseService, IDriveService where T : ICloudDriveService
   {
      public ICloudDriveService CloudService { get { return DependencyProvider.Get<T>(); } }

      public Task<LibraryStoreVM> AddLibrary() => throw new NotImplementedException();

      public Task<bool> ExtractCover(LibraryStoreVM library, ItemStoreVM comicFile) => throw new NotImplementedException();

      public virtual string EscapeFileID(string fileID) => throw new NotImplementedException();

   }

   public class BaseService
   {

      public static IDriveService GetDriveService(string libraryID) =>
         GetDriveService(DependencyService.Get<Store.IStoreService>().GetLibrary(libraryID));

      public static IDriveService GetDriveService(LibraryStoreVM library) =>
         GetDriveService(library.Type);

      public static IDriveService GetDriveService(enLibraryType libraryType)
      {
         try
         {
            if (libraryType == enLibraryType.OneDrive)
               return DependencyService.Get<OneDriveEngine>();
            else
               return DependencyService.Get<LocalDriveEngine>();
         }
         catch (Exception) { throw; }
      }

   }

}
