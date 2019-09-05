using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   public interface ILibraryStore
   {
      List<LibraryModel> Libraries { get; set; }
      Task<bool> NewLibraryAsync(LibraryType libraryType);
      Task<bool> AddLibraryAsync(LibraryModel libraryModel);
      Task<bool> DeleteLibraryAsync(string id);
      Task<LibraryModel> GetLibraryAsync(string id);
   }
}
