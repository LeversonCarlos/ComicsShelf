using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Libraries
{
   internal class LibraryService
   {

      public readonly Dictionary<string, List<ComicFiles.ComicFile>> ComicFiles;
      public LibraryService()
      {
         this.ComicFiles = new Dictionary<string, List<ComicFiles.ComicFile>>();
      }

      public static ShellItem Add(LibraryModel library)
      {
         try
         {

            var libraryVM = new LibraryVM(library);

            var shellContent = new ShellContent
            {
               Title = library.Description,
               BindingContext = libraryVM,
               ContentTemplate = new DataTemplate(typeof(LibraryPage))
            };

            var shellSection = new ShellSection { Title = library.Description };
            shellSection.Items.Add(shellContent);

            var shellItem = new ShellItem { Title = library.Description, Icon = $"icon_{library.Type.ToString()}.png" };
            shellItem.Items.Add(shellSection);

            Shell.CurrentShell.Items.Add(shellItem);
            return shellItem;

         }
         catch (Exception) { throw; }
      }


      public static async Task StartupLibrary(LibraryModel library)
      {
         try
         {
            var service = DependencyService.Get<LibraryService>();
            if (service == null) { return; }
            Task.Run(async () => await StartupLibrary(service, library));
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      private static async Task StartupLibrary(LibraryService service, LibraryModel library)
      {
         try
         {
            service.ComicFiles.Add(library.ID, new List<ComicFiles.ComicFile>());
            await service.StartupLibrary_LoadLibrary(library);
            await service.StartupLibrary_RefreshLibrary(library);

            Messaging.Send<List<ComicFiles.ComicFile>>("OnRefreshingList", library.ID, service.ComicFiles[library.ID]);

            /*
            Messaging.Send("RefreshLibrary", library.ID, files);
            Messaging.Subscribe<ComicFile[]>("LoadLibrary", this.Library.ID, async (files) =>
            {
               await this.RenderLibrary(files); 
               await this.RefreshLibrary();
            });
            */
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      private async Task StartupLibrary_LoadLibrary(LibraryModel library)
      {
         try
         {
            var engine = Engines.Engine.Get(library.Type);
            if (engine == null) { return; }

            var byteArray = await engine.LoadData(library);
            if (byteArray == null) { return; }

            var comicFiles = Helpers.FileStream.Deserialize<List<ComicFiles.ComicFile>>(byteArray);
            if (comicFiles == null) { return; }

            this.ComicFiles[library.ID].AddRange(comicFiles);
         }
         catch (Exception) { throw; }
      }

      private async Task StartupLibrary_RefreshLibrary(LibraryModel library)
      {
         try
         {
            var engine = Engines.Engine.Get(library.Type);
            if (engine == null) { return; }
            var libraryFiles = this.ComicFiles[library.ID];

            var searchFiles = await engine.SearchFiles(library);
            if (searchFiles == null) { return; }

            libraryFiles
               .RemoveAll(x => !searchFiles.Select(i => i.Key).ToList().Contains(x.Key));
            libraryFiles.AddRange(searchFiles
               .Where(x => !libraryFiles.Select(i => i.Key).ToList().Contains(x.Key))
               .ToList());

         }
         catch (Exception) { throw; }
      }


      public void Test(string libraryID, string comicKey)
      {
         var comicFile = this.ComicFiles[libraryID].Where(x => x.Key == comicKey).FirstOrDefault();
         comicFile.FullText += " [changed]";

         Messaging.Send("OnRefreshingItem", libraryID, comicFile);

      }

   }
}
