using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   partial class FileSystemService
   {

      public async Task ExtractCoverAsync(Helpers.Database.Library library, Helpers.Database.ComicFile comicFile, System.Action successCallback)
      {
         await this.FileSystem.CoverExtract(App.Settings, App.Database, comicFile);
         if (System.IO.File.Exists(comicFile.CoverPath)) { successCallback(); }
      }

   }
}