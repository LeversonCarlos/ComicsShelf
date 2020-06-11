using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class LocalDriveEngine
   {

      public override Task<bool> ValidateLibrary(LibraryStoreVM library) => throw new NotImplementedException();

   }
}
