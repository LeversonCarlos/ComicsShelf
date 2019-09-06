using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store.Mock
{
   public class MockLibraryStore : ILibraryStore
   {

      public List<LibraryModel> Libraries { get; set; }
      public Dictionary<enLibraryFilesGroup, List<ComicFiles.ComicFileVM>> LibraryFiles { get; set; }

      public MockLibraryStore()
      {
         this.Libraries = new List<LibraryModel>();
         this.LibraryFiles = new Dictionary<enLibraryFilesGroup, List<ComicFiles.ComicFileVM>>();
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

      public async Task<bool> AddLibraryAsync(LibraryModel library)
      {
         if (this.Libraries.Any(x => x.ID == library.ID)) { return false; }
         this.Libraries.Add(library);
         Messaging.Send(Messaging.enMessages.LibraryAdded.ToString(), library);

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
                  LibraryKey = library.ID,
                  Key = Guid.NewGuid().ToString(),
                  FolderPath = folder,
                  FullText = $"{folder} #{(j + 1).ToString()}",
                  SmallText = (j + 1).ToString(),
                  Available = true
               };
               this.ComicFiles[library.ID].Add(new ComicFiles.ComicFileVM(comicFile));
            }
         }
         return await Task.FromResult(true);
      }

      public async Task<bool> DeleteLibraryAsync(string id)
      {
         var libraryModel = this.Libraries.FirstOrDefault(x => x.ID == id);
         Messaging.Send(Messaging.enMessages.LibraryRemoved.ToString(), libraryModel);
         var removed = this.Libraries.RemoveAll(x => x.ID == libraryModel.ID);
         return await Task.FromResult(removed > 0);
      }

      public async Task<LibraryModel> GetLibraryAsync(string id)
      {
         return await Task.FromResult(this.Libraries.FirstOrDefault(x => x.ID == id));
      }

   }
}
