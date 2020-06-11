using ComicsShelf.Store;
using System;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;
using Xamarin.Forms;

namespace ComicsShelf.Drive
{

   public abstract partial class Engine<T> : Engine, IEngine where T : ICloudDriveService
   {
      public ICloudDriveService DriveService { get { return DependencyProvider.Get<T>(); } }

      public Task<LibraryStoreVM> AddLibrary() => throw new NotImplementedException();

      public Task<bool> ExtractCover(LibraryStoreVM library, ItemStoreVM comicFile) => throw new NotImplementedException();

      public virtual string EscapeFileID(string fileID) => throw new NotImplementedException();

   }

   public class Engine
   {

      public static IEngine GetEngine(string libraryID) =>
         GetEngine(DependencyService.Get<Store.IStoreService>().GetLibrary(libraryID));

      public static IEngine GetEngine(LibraryStoreVM library) =>
         GetEngine(library.Type);

      public static IEngine GetEngine(enLibraryType libraryType)
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
