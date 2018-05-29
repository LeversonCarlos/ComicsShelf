using Android;
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

      #region CheckPermissions
      public void CheckPermissions(System.Action grantedCallback, System.Action revokedCallback)
      {
         Permission.Validate(
            new string[] { Android.Manifest.Permission.ReadExternalStorage, Android.Manifest.Permission.WriteExternalStorage },
            grantedCallback, revokedCallback);
      }
      #endregion

      #region GetDataPath
      public async Task<string> GetDataPath()
      {
         var dataPath =$"{Android.OS.Environment.ExternalStorageDirectory.Path}{this.PathSeparator}ComicsShelf";
         if (!System.IO.Directory.Exists(dataPath)) { System.IO.Directory.CreateDirectory(dataPath); }
         // var filesPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
         // var dataPath = filesPath.Replace("/files", "/databases");
         return dataPath;
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

      #region ValidateLibraryPath
      public async Task<bool> ValidateLibraryPath(string libraryPath)
      {
         if (string.IsNullOrEmpty(libraryPath)) { return false; }
         if (!await Task.Run(() => System.IO.Directory.Exists(libraryPath))) { return false; }
         return true;
      }
      #endregion
      
      #region GetLibraryPath
      public async Task<string> GetLibraryPath()
      {
         return await FolderDialog.GetDirectoryAsync(string.Empty);
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