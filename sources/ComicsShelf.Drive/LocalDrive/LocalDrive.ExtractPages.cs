using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class LocalDrive
   {

      public override Task<PageVM[]> ExtractPages(LibraryVM library, ItemVM libraryItem) => throw new NotImplementedException();

   }
}
