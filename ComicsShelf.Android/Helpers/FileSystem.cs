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

      #region GetDataPath
      public async Task<string> GetDataPath()
      {
         var filesPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
         return filesPath;
      }
      #endregion

      #region GetCachePath
      public async Task<string> GetCachePath()
      {
         var filesPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
         var cachePath = filesPath.Replace("/files", "/cache");
         return cachePath;
      }
      #endregion

      #region GetComicsPath
      public async Task<string> GetComicsPath(string comicsPath)
      {

         if (!string.IsNullOrEmpty(comicsPath) && System.IO.Directory.Exists(comicsPath))
         { return comicsPath; }

         var fileDialog = new FileDialog(Forms.Context, FileDialog.FileSelectionMode.FolderChooseRoot);
         comicsPath = await fileDialog.GetFileOrDirectoryAsync(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);
         return comicsPath;
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