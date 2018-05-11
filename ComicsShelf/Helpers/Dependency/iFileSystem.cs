using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers
{
   public interface iFileSystem
   {
      string PathSeparator { get; }
      Task<string> GetCachePath();
      Task<string> GetDataPath();
      Task<bool> ValidateLibraryPath(string libraryPath);
      Task<string> GetLibraryPath();
      Task<string[]> GetFiles(string path);
      Task<System.IO.Compression.ZipArchive> GetZipArchive(Settings.Settings settings, File.FileData comicFile);
      Task Thumbnail(System.IO.Stream imageStream, string imagePath);
   }
   public class FileSystem
   {
      public static iFileSystem Get()
      { return DependencyService.Get<iFileSystem>(); }
   }
}