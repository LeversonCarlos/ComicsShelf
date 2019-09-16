using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engines
{

   internal interface IEngine
   {
      //Task<bool> Validate(LibraryModel library);
      //void NewLibrary(System.Action<LibraryModel> resultCallback);
      //Task<bool> DeleteLibrary(LibraryModel library);

      //Task<byte[]> LoadSyncData(LibraryModel library);
      //Task<bool> SaveSyncData(LibraryModel library, byte[] serializedValue);
      //Task<ComicFile[]> SearchFiles(LibraryModel library);

      //Task<bool> ExtractCover(LibraryModel library, ComicFile comicFile);
      //Task<List<ComicPageVM>> ExtractPages(LibraryModel library, ComicFile comicFile);
   }

   internal class Engine
   {
      public static IEngine Get(LibraryType libraryType)
      {
         if (libraryType == LibraryType.OneDrive)
         {
            return DependencyService.Get<OneDrive.OneDriveEngine>();
         }
         else if (libraryType == LibraryType.LocalDrive)
         {
            return DependencyService.Get<LocalDrive.LocalDriveEngine>();
         }
         else { return null; }
      }
   }

}
