using ComicsShelf.Store;
using System;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;
using Xamarin.Forms;

namespace ComicsShelf.Drive
{

   public abstract partial class BaseDrive<T> : BaseDrive, IDrive where T : ICloudDriveService
   {
      public ICloudDriveService CloudService { get { return DependencyProvider.Get<T>(); } }

      public Task<LibraryStoreVM> AddLibrary() => throw new NotImplementedException();

      public Task<bool> ExtractCover(LibraryStoreVM library, ItemStoreVM comicFile) => throw new NotImplementedException();

      public virtual string EscapeFileID(string fileID) => throw new NotImplementedException();

   }

   public class BaseDrive
   {

      public static IDrive GetDrive(string libraryID) =>
         GetDrive(DependencyService.Get<Store.IStoreService>().GetLibrary(libraryID));

      public static IDrive GetDrive(LibraryStoreVM library) =>
         GetDrive(library.Type);

      public static IDrive GetDrive(enLibraryType libraryType)
      {
         try
         {
            if (libraryType == enLibraryType.OneDrive)
               return DependencyService.Get<OneDriveEngine>();
            else
               return DependencyService.Get<LocalDriveService>();
         }
         catch (Exception) { throw; }
      }

   }

}
