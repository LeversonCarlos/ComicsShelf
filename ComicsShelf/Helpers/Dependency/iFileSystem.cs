using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers
{
   public interface iFileSystem
   {
      string PathSeparator { get; }
      Task<string> GetLocalPath();
      Task<string> GetComicsPath(string comicsPath);
   }
   public class FileSystem
   {
      public static iFileSystem Get()
      { return DependencyService.Get<iFileSystem>(); }
   }
}