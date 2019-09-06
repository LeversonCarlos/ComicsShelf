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

         var libraries = new List<LibraryModel>
         {
            new LibraryModel {
               ID = Guid.NewGuid().ToString(),
               Key = DateTime.Now.ToString("HHmmss"),
               Description = $"Library {DateTime.Now.ToString("ssfff")}",
               Type = LibraryType.LocalDrive
            }
         };

         foreach (var library in libraries)
         { await this.AddLibraryAsync(library); }

         foreach (var library in libraries)
         {
            using (var services = new Services.LibraryService(library))
            { await services.InitializeLibrary(); }
         }

         foreach (var library in libraries)
         {
            using (var services = new Services.LibraryService(library))
            { await services.RefreshLibrary(); }
         }

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
         if (!await this.AddLibraryAsync(library)) { return false; }
         App.NavigationHide();
         using (var services = new Services.LibraryService(library))
         {
            await services.InitializeLibrary();
            await services.RefreshLibrary();
         }
         return true;
      }

      public async Task<bool> AddLibraryAsync(LibraryModel library)
      {
         if (this.Libraries.Any(x => x.ID == library.ID)) { return false; }
         this.Libraries.Add(library);
         Messaging.Send(Messaging.enMessages.LibraryAdded.ToString(), library);
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

      public List<ComicFiles.ComicFileVM> GetLibraryFiles(LibraryModel library)
      {
         return this.LibraryFiles[enLibraryFilesGroup.Libraries]
            .Where(x => x.ComicFile.LibraryKey == library.ID)
            .ToList();
      }

      public async Task<List<ComicFiles.ComicFile>> SearchFilesAsync(LibraryModel library)
      {
         var comicFiles = new List<ComicFiles.ComicFile>();

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
                  ReleaseDate = DateTime.Now,
                  Available = true
               };
               comicFiles.Add(comicFile);
            }
         }
         return await Task.FromResult(comicFiles);
      }

      public async Task<bool> ExtractCoverAsync(LibraryModel library, ComicFiles.ComicFile comicFile)
      {
         return await Task.FromResult(true);
      }

   }
}
