using System.Threading.Tasks;

namespace ComicsShelf.Engines
{
   internal interface IEngine
   {
      Task<bool> Validate(Libraries.LibraryModel library);
      Task<bool> AddLibrary(Libraries.LibraryModel library);
      Task<bool> RemoveLibrary(Libraries.LibraryModel library);
   }
}
