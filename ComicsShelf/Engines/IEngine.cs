using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engines
{

   internal interface IEngine
   {
      Task<bool> Validate(Libraries.LibraryModel library);
      Task<Libraries.LibraryModel> NewLibrary();
      Task<bool> DeleteLibrary(Libraries.LibraryModel library);

      Task<byte[]> LoadSyncData(Libraries.LibraryModel library);
      Task<bool> SaveSyncData(Libraries.LibraryModel library, byte[] serializedValue);
      Task<ComicFile[]> SearchFiles(Libraries.LibraryModel library);

      Task<bool> ExtractCover(Libraries.LibraryModel library, ComicFile comicFile);
      Task<List<ComicPageVM>> ExtractPages(Libraries.LibraryModel library, ComicFile comicFile);

   }

   internal class Engine
   {
      public static IEngine Get(Libraries.LibraryType libraryType)
      {
         if (libraryType == Libraries.LibraryType.OneDrive)
         {
            return null;
         }
         else if (libraryType == Libraries.LibraryType.LocalDrive)
         {
            return DependencyService.Get<LocalDrive.LocalDriveEngine>();
         }
         else { return null; }
      }
   }

}
