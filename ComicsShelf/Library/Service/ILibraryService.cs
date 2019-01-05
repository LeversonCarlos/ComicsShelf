using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicsShelf.Library
{
   public enum LibraryTypeEnum : short { FileSystem = 0, OneDrive = 1 }
   internal interface ILibraryService
   {
      Task<bool> Validate(Helpers.Database.Library library);
      Task<bool> AddLibrary(Helpers.Database.Library library);
      Task<List<Helpers.Database.ComicFile>> SearchFilesAsync(Helpers.Database.Library library);
      Task ExtractCoverAsync(Helpers.Database.Library library, Helpers.Database.ComicFile comicFile, Action successCallback);
      Task ExtractPagesAsync(Helpers.Database.Library library, Views.File.FileData fileData);
   }
}