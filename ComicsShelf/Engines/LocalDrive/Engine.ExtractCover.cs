using ComicsShelf.ComicFiles;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async Task<bool> ExtractCover(Libraries.LibraryModel library, ComicFile comicFile)
      {
         try
         {
            await this.FileSystem.ExtractCover(library, comicFile);
            return (System.IO.File.Exists(comicFile.CoverPath));
         }
         catch (Exception) { throw; }
      }

   }
}