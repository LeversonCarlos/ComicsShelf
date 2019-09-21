using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   public enum enLibraryFilesGroup : short { RecentFiles, ReadingFiles, Libraries };
   public interface ILibraryStore
   {

      List<LibraryModel> Libraries { get; set; }
      Dictionary<enLibraryFilesGroup, List<ComicFiles.ComicFileVM>> LibraryFiles { get; set; }

      Task LoadLibrariesAsync();
      Task<bool> NewLibraryAsync(LibraryType libraryType);
      Task<bool> AddLibraryAsync(LibraryModel libraryModel);
      Task<bool> DeleteLibraryAsync(string id);

      LibraryModel GetLibrary(string id);
      List<ComicFiles.ComicFileVM> GetLibraryFiles(LibraryModel library);

   }
}
