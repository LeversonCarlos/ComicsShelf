using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ComicsShelf.UWP.FileSystem))]
namespace ComicsShelf.UWP
{
   public partial class FileSystem : ComicsShelf.Helpers.iFileSystem
   {

      public string PathSeparator
      { get { return System.IO.Path.DirectorySeparatorChar.ToString(); } }

      public string GetDataPath()
      {
         var localPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
         return localPath;
      }

      public string GetCachePath()
      {
         var localPath = Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;
         return localPath;
      }

      public void Dispose()
      { }

   }
}