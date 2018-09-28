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

      Task<bool> ValidateLibraryPath(string libraryPath);
      Task<string> GetLibraryPath();

      Task<string[]> GetFiles(string path);

      Task<System.IO.Compression.ZipArchive> GetZipArchive(Helpers.Settings.Settings settings, string fullPath);
      Task Thumbnail(System.IO.Stream imageStream, string imagePath);

      void CheckPermissions(Action grantedCallback, Action revokedCallback);
   }

   public class FileSystem
   {
      public static iFileSystem Get()
      { return DependencyService.Get<iFileSystem>(); }

   }

}