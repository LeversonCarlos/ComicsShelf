using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   partial class FileSystemService
   {

      public async Task<bool> ExtractCoverAsync(Helpers.Database.Library library, Helpers.Database.ComicFile comicFile)
      {
         await this.FileSystem.CoverExtract(App.Settings, App.Database, comicFile);
         return (System.IO.File.Exists(comicFile.CoverPath));
      }

   }
}