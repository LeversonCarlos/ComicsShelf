using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class Engine<T> 
   {

      public virtual Task<bool> ValidateLibrary(LibraryStoreVM library) => throw new NotImplementedException();

   }
}
