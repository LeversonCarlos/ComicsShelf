using System.Collections.Generic;
using System.Threading.Tasks;
using ComicsShelf.ComicFiles;
using ComicsShelf.Libraries;
using Xamarin.Forms;

namespace ComicsShelf.Engines.OneDrive
{
   internal partial class OneDriveEngine : IEngine, System.IDisposable
   {
      // public const string LibraryFileID = "LibraryFileID";
      // public const string LibraryPath = "LibraryPath";

      Xamarin.OneDrive.Connector Connector { get; set; }
      public OneDriveEngine()
      {
         this.Connector = new Xamarin.OneDrive.Connector(AppKeys.OneDriveApplicationID, "User.Read", "Files.ReadWrite");
      }

      private Helpers.IFileSystem FileSystem
      {
         get { return DependencyService.Get<Helpers.IFileSystem>(); }
      }

      public void Dispose()
      {
         if (this.Connector != null)
         {
            this.Connector.Dispose();
            this.Connector = null;
         }
      }





      public Task<LibraryModel> NewLibrary()
      {
         throw new System.NotImplementedException();
      }

      public Task<bool> DeleteLibrary(LibraryModel library)
      {
         throw new System.NotImplementedException();
      }

      public Task<byte[]> LoadSyncData(LibraryModel library)
      {
         throw new System.NotImplementedException();
      }

      public Task<bool> SaveSyncData(LibraryModel library, byte[] serializedValue)
      {
         throw new System.NotImplementedException();
      }

      public Task<ComicFile[]> SearchFiles(LibraryModel library)
      {
         throw new System.NotImplementedException();
      }

      public Task<bool> ExtractCover(LibraryModel library, ComicFile comicFile)
      {
         throw new System.NotImplementedException();
      }

      public Task<List<ComicPageVM>> ExtractPages(LibraryModel library, ComicFile comicFile)
      {
         throw new System.NotImplementedException();
      }

   }
}
