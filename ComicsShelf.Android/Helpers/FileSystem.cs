using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ComicsShelf.Droid.FileSystem))]
namespace ComicsShelf.Droid
{
   public partial class FileSystem : ComicsShelf.Helpers.iFileSystem
   {

      #region PathSeparator
      public string PathSeparator
      { get { return System.IO.Path.DirectorySeparatorChar.ToString(); } }
      #endregion

      #region GetLocalPath
      public async Task<string> GetLocalPath()
      {
         var localPath = Android.OS.Environment.ExternalStorageDirectory.Path;
         return localPath;
      }
      #endregion

      #region GetComicsPath
      public async Task<string> GetComicsPath(string comicsPath)
      {
         if (string.IsNullOrEmpty(comicsPath) || !System.IO.Directory.Exists(comicsPath))
         { return Android.OS.Environment.ExternalStorageDirectory.Path; }
         else { return comicsPath; }
      }
      #endregion

      #region GetFiles
      public async Task<string[]> GetFiles(string path)
      {
         try
         {
            var fileList = System.IO.Directory
               .EnumerateFiles(path, "*.*", System.IO.SearchOption.AllDirectories)
               .Where(x =>
                  x.EndsWith(".cbz", StringComparison.OrdinalIgnoreCase) ||
                  x.EndsWith(".cbr", StringComparison.OrdinalIgnoreCase))
               .ToList();

            var folderPath = $"{path}{this.PathSeparator}";
            var fileListRenamed = fileList
               .Select(x => x.Replace(folderPath, ""))
               .ToList();

            var fileArray = fileListRenamed.ToArray();
            return fileArray;
         }
         catch (Exception ex) { throw; }
      }
      #endregion

   }
}