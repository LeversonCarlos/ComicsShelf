using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(ComicsShelf.Droid.FileSystem))]
namespace ComicsShelf.Droid
{
   public class FileSystem : ComicsShelf.Helpers.iFileSystem
   {

      #region PathSeparator
      public string PathSeparator
      { get { return System.IO.Path.DirectorySeparatorChar.ToString(); } }
      #endregion

      #region GetComicsPath
      public async Task<string> GetComicsPath(string comicsPath)
      {
         if (string.IsNullOrEmpty(comicsPath) || !System.IO.Directory.Exists(comicsPath))
         { return Android.OS.Environment.ExternalStorageDirectory.Path; }
         else { return comicsPath; }
      }
      #endregion

   }
}