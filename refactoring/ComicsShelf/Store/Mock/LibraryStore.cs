using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicsShelf.Store.Mock
{
   public class LibraryStore : ILibraryStore
   {

      public Dictionary<string, LibraryModel> Libraries { get; set; }
      public Dictionary<string, List<ComicFiles.ComicFileVM>> ComicFiles { get; set; }

      public LibraryStore()
      {
         this.Libraries = new Dictionary<string, LibraryModel>();
         this.ComicFiles = new Dictionary<string, List<ComicFiles.ComicFileVM>>();
      }

      public async Task LoadLibrariesAsync()
      {
         await this.AddLibraryAsync(new LibraryModel
         {
            ID = Guid.NewGuid().ToString(),
            Key = DateTime.Now.ToString("HHmmss"),
            Description = $"Library {DateTime.Now.ToString("ssfff")}",
            Type = LibraryType.LocalDrive
         });
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
         this.Libraries.Add(libraryModel.ID, libraryModel);
         this.ComicFiles.Add(libraryModel.ID, new List<ComicFiles.ComicFileVM>());
         Messaging.Send(Messaging.enMessages.LibraryAdded.ToString(), libraryModel);

         var rnd = new Random((int)DateTime.Now.Ticks);
         var folders = rnd.Next(1, 10);
         for (int i = 0; i < folders; i++)
         {
            var folder = Guid.NewGuid().ToString();
            var files = rnd.Next(1, 7);
            for (int j = 0; j < files; j++)
            {
               var comicFile = new ComicFiles.ComicFile
               {
                  LibraryKey = libraryModel.ID,
                  Key = Guid.NewGuid().ToString(),
                  FolderPath = folder,
                  FullText = $"{folder} #{(j + 1).ToString()}",
                  SmallText = (j + 1).ToString(),
                  Available = true
               };
               this.ComicFiles[libraryModel.ID].Add(new ComicFiles.ComicFileVM(comicFile));
            }
         }
         return await Task.FromResult(true);
      }

      public async Task<bool> DeleteLibraryAsync(string id)
      {
         var libraryModel = this.Libraries[id];
         Messaging.Send(Messaging.enMessages.LibraryRemoved.ToString(), libraryModel);
         var removed = this.Libraries.Remove(libraryModel.ID);
         return await Task.FromResult(removed);
      }

      public async Task<LibraryModel> GetLibraryAsync(string id)
      {
         return await Task.FromResult(this.Libraries[id]);
      }

   }
}
