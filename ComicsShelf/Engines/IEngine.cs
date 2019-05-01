using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engines
{

   internal interface IEngine
   {
      Task<bool> Validate(Libraries.LibraryModel library);
      Task<Libraries.LibraryModel> NewLibrary();
      Task<bool> DeleteLibrary(Libraries.LibraryModel library);

      // Task<ComicFile[]> LoadFiles(Libraries.LibraryModel library);
      Task<ComicFile[]> SearchFiles(Folder folder);

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
