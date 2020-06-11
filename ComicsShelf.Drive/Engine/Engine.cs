using ComicsShelf.Store;
using System;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;

namespace ComicsShelf.Drive
{
   public abstract partial class Engine<T> : IEngine where T : ICloudDriveService
   {
      public ICloudDriveService DriveService { get { return DependencyProvider.Get<T>(); } }

      public Task<bool> ValidateLibrary(LibraryStoreVM library) => throw new NotImplementedException();

      public Task<LibraryStoreVM> AddLibrary() => throw new NotImplementedException();

      public Task<ItemStoreVM[]> SearchFiles(LibraryStoreVM library) => throw new NotImplementedException();

      public Task<bool> ExtractCover(LibraryStoreVM library, ItemStoreVM comicFile) => throw new NotImplementedException();

      public virtual string EscapeFileID(string fileID) => throw new NotImplementedException();

   }
}
