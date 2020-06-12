using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class BaseService<T> 
   {

      public virtual Task<bool> ValidateLibrary(LibraryStoreVM library) => throw new NotImplementedException();

   }
}
