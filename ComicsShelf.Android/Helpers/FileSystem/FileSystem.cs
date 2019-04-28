using Xamarin.Forms;

[assembly: Dependency(typeof(ComicsShelf.Droid.FileSystem))]
namespace ComicsShelf.Droid
{
   public partial class FileSystem : Helpers.IFileSystem
   {

      public void Dispose()
      {
      }

   }
}