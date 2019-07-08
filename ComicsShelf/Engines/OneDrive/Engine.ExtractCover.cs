using ComicsShelf.ComicFiles;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async Task<bool> ExtractCover(Libraries.LibraryModel library, ComicFile comicFile)
      {
         try
         {
            return true;
            // await this.FileSystem.ExtractCover(library, comicFile);
            // return (System.IO.File.Exists(comicFile.CoverPath));
         }
         catch (Exception) { throw; }
      }

   }
}