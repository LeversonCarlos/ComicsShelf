using System.Collections.Generic;
using System.Threading.Tasks;
using ComicsShelf.ComicFiles;
using ComicsShelf.Libraries;
using Xamarin.Forms;

namespace ComicsShelf.Engines.OneDrive
{
   internal partial class OneDriveEngine : IEngine
   {

      private Helpers.IFileSystem FileSystem
      {
         get { return DependencyService.Get<Helpers.IFileSystem>(); }
      }

      private OneDriveConnector Connector
      {
         get { return DependencyService.Get<OneDriveConnector>(); }
      }

      public Task<byte[]> LoadSyncData(LibraryModel library)
      {
         throw new System.NotImplementedException();
      }

      public Task<bool> SaveSyncData(LibraryModel library, byte[] serializedValue)
      {
         throw new System.NotImplementedException();
      }

      public Task<List<ComicPageVM>> ExtractPages(LibraryModel library, ComicFile comicFile)
      {
         throw new System.NotImplementedException();
      }

   }
}
