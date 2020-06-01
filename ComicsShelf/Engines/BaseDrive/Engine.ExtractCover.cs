using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.BaseDrive
{
   partial class BaseDriveEngine<T>
   {

      public virtual Task<bool> ExtractCover(LibraryModel library, ComicFile comicFile) => throw new NotImplementedException();

   }
}