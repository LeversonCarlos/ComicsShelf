using System.Threading.Tasks;

namespace ComicsShelf.Engines
{
   internal interface IEngine
   {
      Task<bool> Validate(Libraries.LibraryModel library);
      Task<Libraries.LibraryModel> NewLibrary();
      Task<bool> RemoveLibrary(Libraries.LibraryModel library);
   }
}
