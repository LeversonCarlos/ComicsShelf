using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(ComicsShelf.Droid.FileSystem))]
namespace ComicsShelf.Droid
{
   public partial class FileSystem : ComicsShelf.Helpers.iFileSystem
   {

      public string PathSeparator
      { get { return System.IO.Path.DirectorySeparatorChar.ToString(); } }

      public string GetDataPath()
      {
         var dataPath = $"{Android.OS.Environment.ExternalStorageDirectory.Path}{this.PathSeparator}ComicsShelf";
         if (!System.IO.Directory.Exists(dataPath)) { System.IO.Directory.CreateDirectory(dataPath); }
         // var filesPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
         // var dataPath = filesPath.Replace("/files", "/databases");
         return dataPath;
      }

      public string GetCachePath()
      {
         var filesPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
         var cachePath = filesPath.Replace("/files", "/cache");
         return cachePath;
      }

      public void Dispose()
      { }

   }
}