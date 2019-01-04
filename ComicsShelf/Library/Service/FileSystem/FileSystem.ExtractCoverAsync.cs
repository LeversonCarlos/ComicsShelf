using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   partial class FileSystemService
   {

      public async Task ExtractCoverAsync(Helpers.Database.Library library, Helpers.Database.ComicFile comicFile)
      {
         await this.FileSystem.CoverExtract(App.Settings, App.Database, comicFile);
      }

   }
}