using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T> 
   {

      public virtual Task<bool> ValidateLibrary(ItemVM library) => throw new NotImplementedException();

   }
}
