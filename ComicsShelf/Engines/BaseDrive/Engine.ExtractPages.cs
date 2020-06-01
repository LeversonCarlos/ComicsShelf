using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.BaseDrive
{
   partial class BaseDriveEngine<T>
   {

      public virtual Task<List<ComicPageVM>> ExtractPages(LibraryModel library, ComicFile comicFile) => throw new NotImplementedException();

   }
}