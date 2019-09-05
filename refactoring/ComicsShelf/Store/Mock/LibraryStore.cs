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
         Messaging.Send(Messaging.enMessages.LibraryAdded.ToString(), libraryModel);
         return await Task.FromResult(true);
      }

      public async Task<bool> DeleteLibraryAsync(string id)
      {
         var libraryModel = this.Libraries.Where(x => x.ID == id).FirstOrDefault();
         Messaging.Send(Messaging.enMessages.LibraryRemoved.ToString(), libraryModel);
         var removed = this.Libraries.Remove(libraryModel);
         return await Task.FromResult(removed);
      }

      public async Task<LibraryModel> GetLibraryAsync(string id)
      {
         return await Task.FromResult(this.Libraries.Where(x => x.ID == id).FirstOrDefault());
      }

   }
}
