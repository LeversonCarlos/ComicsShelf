using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Libraries
{
   internal class LibraryService
   {

      public readonly Dictionary<string, List<ComicFiles.ComicFileVM>> ComicFiles;
      public LibraryService()
      {
         this.ComicFiles = new Dictionary<string, List<ComicFiles.ComicFileVM>>();
      }

      public static async Task StartupLibrary(LibraryModel library)
      {
         try
         {
            var service = DependencyService.Get<LibraryService>();
            if (service == null) { return; }
            // Task.Run(async () => await StartupLibrary(service, library));
            await StartupLibrary(service, library);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      private static async Task StartupLibrary(LibraryService service, LibraryModel library)
      {
         try
         {
            service.ComicFiles.Add(library.ID, new List<ComicFiles.ComicFileVM>());
            if (!await service.LoadData(library)) { return; }
            if (!await service.Render(library)) { return; }
            if (!await service.LoadSyncData(library)) { return; }
            if (!await service.Render(library)) { return; }
            if (!await service.Statistics(library)) { return; }
            if (!await service.SearchData(library)) { return; }
            if (!await service.Render(library)) { return; }
            if (!await service.ExtractData(library)) { return; }
            if (!await service.Statistics(library)) { return; }
            if (!await service.SaveSyncData(library)) { return; }
            if (!await service.SaveData(library)) { return; }
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }


      private async Task<bool> LoadData(LibraryModel library)
      {
         try
         {

            var files = await Helpers.FileStream.ReadFile<List<ComicFiles.ComicFile>>(LibraryConstants.DatabaseFile);
            if (files == null) { return true; }

            var comicFiles = files.Select(x => new ComicFiles.ComicFileVM(x)).ToList();
            foreach (var comicFile in comicFiles)
            {
               if (System.IO.File.Exists(comicFile.ComicFile.CoverPath))
               {
                  comicFile.CoverPath = comicFile.ComicFile.CoverPath;
                  if (comicFile.ComicFile.ReleaseDate == DateTime.MinValue)
                  { comicFile.ComicFile.ReleaseDate = System.IO.File.GetLastWriteTime(comicFile.CoverPath); }
               }
               if (System.IO.Directory.Exists(comicFile.ComicFile.CachePath))
               { comicFile.CachePath = comicFile.ComicFile.CachePath; }
               else { comicFile.CachePath = string.Empty; }

            }

            this.ComicFiles[library.ID].AddRange(comicFiles);
            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.LoadData", ex); return false; }
      }

      private async Task<bool> Render(LibraryModel library)
      {
         try
         {

            var rnd = new Random(DateTime.Now.Second);
            foreach (var item in this.ComicFiles[library.ID])
            {
               var hasCacheEnum = ((double)rnd.Next(0, 100) % 2) == 0;
               item.CachePath = (hasCacheEnum ? library.LibraryKey : string.Empty);
            }

            Messaging.Send<List<ComicFiles.ComicFileVM>>("OnRefreshingList", library.ID, this.ComicFiles[library.ID]);

            // TODO
            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.Render", ex); return false; }
      }

      private async Task<bool> LoadSyncData(LibraryModel library)
      {
         try
         {
            var engine = Engines.Engine.Get(library.Type);
            if (engine == null) { return false; }

            var byteArray = await engine.LoadSyncData(library);
            if (byteArray == null) { return true; }

            var comicFiles = Helpers.FileStream.Deserialize<List<ComicFiles.ComicFile>>(byteArray);
            if (comicFiles == null) { return true; }

            var libraryFiles = this.ComicFiles[library.ID];
            foreach (var comicFile in comicFiles)
            {
               var libraryFile = libraryFiles.Where(x => x.ComicFile.Key == comicFile.Key).FirstOrDefault();
               if (libraryFile != null) { libraryFile.Set(comicFile); }
            }

            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.LoadSyncData", ex); return false; }
      }

      private async Task<bool> Statistics(LibraryModel library)
      {
         try
         {
            // TODO
            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.Statistics", ex); return false; }
      }

      private async Task<bool> SearchData(LibraryModel library)
      {
         try
         {
            var engine = Engines.Engine.Get(library.Type);
            if (engine == null) { return false; }
            var libraryFiles = this.ComicFiles[library.ID];

            var searchFiles = await engine.SearchFiles(library);
            if (searchFiles == null) { return true; }

            libraryFiles
               .RemoveAll(x => !searchFiles.Select(i => i.Key).ToList().Contains(x.ComicFile.Key));
            libraryFiles.AddRange(searchFiles
               .Where(x => !libraryFiles.Select(i => i.ComicFile.Key).ToList().Contains(x.Key))
               .Select(x => new ComicFiles.ComicFileVM(x))
               .ToList());

            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.SearchData", ex); return false; }
      }

      private async Task<bool> ExtractData(LibraryModel library)
      {
         try
         {
            // TODO
            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.ExtractData", ex); return false; }
      }

      private async Task<bool> SaveSyncData(LibraryModel library)
      {
         try
         {
            var engine = Engines.Engine.Get(library.Type);
            if (engine == null) { return false; }

            var comicFiles = this.ComicFiles[library.ID].Select(x => x.ComicFile).ToList();
            if (comicFiles == null) { return true; }

            var byteArray = Helpers.FileStream.Serialize(comicFiles);
            if (byteArray == null) { return true; }

            if (!await engine.SaveSyncData(library, byteArray)) { return false; }
            return true;

         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.SaveSyncData", ex); return false; }
      }

      private async Task<bool> SaveData(LibraryModel library)
      {
         try
         {

            var comicFiles = this.ComicFiles[library.ID].Select(x => x.ComicFile).ToList();
            if (comicFiles == null) { return true; }

            if (!await Helpers.FileStream.SaveFile(LibraryConstants.DatabaseFile, comicFiles)) { return false; }
            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.SaveData", ex); return false; }
      }




      public void Test(string libraryID, string comicKey)
      {
         var comicFile = this.ComicFiles[libraryID].Where(x => x.ComicFile.Key == comicKey).FirstOrDefault();
         comicFile.ComicFile.FullText += " [changed]";

         Messaging.Send("OnRefreshingItem", libraryID, comicFile);

      }

   }
}
