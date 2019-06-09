using ComicsShelf.ComicFiles;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async Task<ComicPagesVM> ExtractPages(Libraries.LibraryModel library, ComicFile comicFile)
      {
         try
         {
            return await this.FileSystem.ExtractPages(library, comicFile);
         }
         catch (Exception) { throw; }
      }

   }
}