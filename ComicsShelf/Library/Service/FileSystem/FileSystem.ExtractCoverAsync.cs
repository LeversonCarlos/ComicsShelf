using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   partial class FileSystemService
   {

      public async Task<bool> ExtractCoverAsync(Library library, Helpers.Database.ComicFile comicFile)
      {
         await this.FileSystem.CoverExtract(App.Database, comicFile);
         return (System.IO.File.Exists(comicFile.CoverPath));
      }

   }
}