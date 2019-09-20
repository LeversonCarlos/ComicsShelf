using Xamarin.Forms;

[assembly: Dependency(typeof(ComicsShelf.Droid.FileSystem))]
namespace ComicsShelf.Droid
{
   public partial class FileSystem : Helpers.IFileSystem
   {

      public string PathSeparator
      { get { return System.IO.Path.DirectorySeparatorChar.ToString(); } }

      public string GetKey(string value)
      {
         return value
            .Replace("#", "")
            .Replace(".", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace(" ", "")
            .Replace(this.PathSeparator, "_")
            .Replace("___", "_")
            .Replace("__", "_");
      }

      public void Dispose()
      { }

   }
}