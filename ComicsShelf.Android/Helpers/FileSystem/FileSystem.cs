using Xamarin.Forms;

[assembly: Dependency(typeof(ComicsShelf.Droid.FileSystem))]
namespace ComicsShelf.Droid
{
   public partial class FileSystem : Helpers.IFileSystem
   {

      public string PathSeparator
      { get { return System.IO.Path.DirectorySeparatorChar.ToString(); } }

      public void Dispose()
      {
      }

   }
}