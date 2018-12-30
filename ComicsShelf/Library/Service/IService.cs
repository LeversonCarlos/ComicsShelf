using System;
using System.Threading.Tasks;

namespace ComicsShelf.Library
{
   public enum LibraryTypeEnum : short { FileSystem = 0, OneDrive = 1 }
   internal interface IService : IDisposable
   {
      Task<bool> Validate(Helpers.Database.Library library);
      Task<bool> AddLibrary(Helpers.Database.Library library);
   }
}