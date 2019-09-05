using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store.Mock
{
   public class LibraryStore : ILibraryStore
   {

      public List<LibraryModel> Libraries { get; set; }
      public LibraryStore()
      {
         this.Libraries = new List<LibraryModel> {
            new LibraryModel {
               ID = Guid.NewGuid().ToString(),
               Key = DateTime.Now.ToString("HHmmss"),
               Description = $"Library {DateTime.Now.ToString("ssfff")}",
               Type = LibraryType.LocalDrive
            }
         };
      }

      public async Task<bool> NewLibraryAsync(LibraryType libraryType)
      {
         var library = new LibraryModel
         {
            ID = Guid.NewGuid().ToString(),
            Key = DateTime.Now.ToString("HHmmss"),
            Description = $"Library {DateTime.Now.ToString("ssfff")}",
            Type = libraryType
         };
         return await this.AddLibraryAsync(library);
      }

      public async Task<bool> AddLibraryAsync(LibraryModel libraryModel)
      {
         this.Libraries.Add(libraryModel);
         return await Task.FromResult(true);
      }

      public async Task<bool> DeleteLibraryAsync(string id)
      {
         var removed = this.Libraries.RemoveAll(x => x.ID == id);
         return await Task.FromResult(removed > 0);
      }

      public async Task<LibraryModel> GetLibraryAsync(string id)
      {
         return await Task.FromResult(this.Libraries.Where(x => x.ID == id).FirstOrDefault());
      }

   }
}
