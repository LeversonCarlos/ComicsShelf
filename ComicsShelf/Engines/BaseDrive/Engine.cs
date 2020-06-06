using ComicsShelf.Store;
using System;
using Xamarin.CloudDrive.Connector.Common;
using Xamarin.Forms;

namespace ComicsShelf.Engines.BaseDrive
{
   internal partial class BaseDriveEngine<T> : IEngine where T : ICloudDriveService
   {

      public ICloudDriveService Service { get { return DependencyProvider.Get<T>(); } }
      readonly Store.LibraryType LibraryType;

      public BaseDriveEngine(Store.LibraryType libraryType)
      {
         this.LibraryType = libraryType;
      }

      protected Helpers.IFileSystem FileSystem
      {
         get { return DependencyService.Get<Helpers.IFileSystem>(); }
      }

      public virtual string EscapeFileID(string fileID) => throw new NotImplementedException();

   }

   internal class BaseDriveEngine
   {
      public static IEngine GetEngine(string libraryKey)
      {
         try
         {
            var libraryStore = DependencyService.Get<Store.ILibraryStore>();
            var library = libraryStore.GetLibrary(libraryKey);
            return GetEngine(library.Type);
         }
         catch (Exception) { throw; }
      }
      public static IEngine GetEngine(LibraryType libraryType)
      {
         try
         {
            if (libraryType == LibraryType.OneDrive)
            { return DependencyService.Get<Engines.OneDrive.OneDriveEngine>(); }
            else
            { return DependencyService.Get<Engines.LocalDrive.LocalDriveEngine>(); }
         }
         catch (Exception) { throw; }
      }
   }

}
