using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem 
   {

      public async Task<bool> Validate(string libraryKey)
      {
         if (string.IsNullOrEmpty(libraryKey)) { return false; }
         if (!await Task.FromResult(System.IO.Directory.Exists(libraryKey))) { return false; }
         return true;
      }

   }
}