using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers
{

   public interface IFileSystem : IDisposable
   {

      string PathSeparator { get; }

      string GetCachePath();
      string GetDataPath();

      Task<bool> ValidateLibraryPath(Libraries.Library library);
      Task<string> GetLibraryPath();

      Task<string[]> GetFiles(string path);

      Task CoverExtract(Helpers.Database.dbContext database, Helpers.Database.ComicFile comicFile);
      Task PagesExtract(Views.File.FileData fileData);
      Task PageSize(Views.File.PageData pageData);

      Task SaveThumbnail(System.IO.Stream imageStream, string imagePath);

      Task<bool> SaveDataAsync(Libraries.Library library, byte[] serializedData);
      Task<byte[]> LoadDataAsync(Libraries.Library library);

   }

   public class FileSystem
   {
      public static iFileSystem Get()
      { return DependencyService.Get<iFileSystem>(); }

   }

}