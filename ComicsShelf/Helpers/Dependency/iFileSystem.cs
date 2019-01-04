using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers
{

   public interface iFileSystem : IDisposable
   {

      string PathSeparator { get; }

      string GetCachePath();
      string GetDataPath();

      Task<bool> ValidateLibraryPath(Helpers.Database.Library library);
      Task<string> GetLibraryPath();

      Task<string[]> GetFiles(string path);

      Task CoverExtract(Helpers.Settings.Settings settings, Helpers.Database.dbContext database, Helpers.Database.ComicFile comicFile);
      Task PagesExtract(Helpers.Settings.Settings settings, Views.File.FileData fileData);

      Task SaveThumbnail(System.IO.Stream imageStream, string imagePath);

      void CheckPermissions(Action grantedCallback, Action revokedCallback);
   }

   public class FileSystem
   {
      public static iFileSystem Get()
      { return DependencyService.Get<iFileSystem>(); }

   }

}