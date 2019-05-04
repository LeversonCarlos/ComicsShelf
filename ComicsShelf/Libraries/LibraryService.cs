﻿using System;
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
            if (!System.IO.Directory.Exists(LibraryConstants.CoversCachePath))
            { System.IO.Directory.CreateDirectory(LibraryConstants.CoversCachePath); }
            if (!System.IO.Directory.Exists(LibraryConstants.FilesCachePath))
            { System.IO.Directory.CreateDirectory(LibraryConstants.FilesCachePath); }

            if (!await service.LoadData(library)) { return; }
            if (!await service.Notify(library)) { return; }
            if (!await service.LoadSyncData(library)) { return; }
            if (!await service.Notify(library)) { return; }
            if (!await service.Statistics(library)) { return; }
            if (!await service.SearchData(library)) { return; }
            if (!await service.Notify(library)) { return; }
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

      private async Task<bool> Notify(LibraryModel library)
      {
         try
         {
            await Task.Run(() => Messaging.Send<List<ComicFiles.ComicFileVM>>("OnRefreshingList", library.ID, this.ComicFiles[library.ID]));
            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.Notify", ex); return false; }
      }

      private async Task<bool> Notify(LibraryModel library, string prefix, List<ComicFiles.ComicFileVM> comicFiles)
      {
         try
         {
            await Task.Run(() => Messaging.Send<List<ComicFiles.ComicFileVM>>(prefix, comicFiles));
            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.Notify", ex); return false; }
      }

      private async Task<bool> LoadSyncData(LibraryModel library)
      {
         try
         {
            var engine = Engines.Engine.Get(library.Type);
            if (engine == null) { return false; }

            var byteArray = await engine.LoadSyncData(library);
            if (byteArray == null) { return true; }

            var syncFiles = Helpers.FileStream.Deserialize<List<ComicFiles.ComicFile>>(byteArray);
            if (syncFiles == null) { return true; }

            var comicFiles = this.ComicFiles[library.ID];
            foreach (var syncFile in syncFiles)
            {
               var comicFile = comicFiles.Where(x => x.ComicFile.Key == syncFile.Key).FirstOrDefault();
               if (comicFile != null) { comicFile.Set(syncFile); }
            }

            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.LoadSyncData", ex); return false; }
      }

      private async Task<bool> Statistics(LibraryModel library)
      {
         try
         {

            // READING FILES
            var readingFiles = this.Statistics_GetReadingFiles(library);
            if (readingFiles == null) { return false; }
            await this.Notify(library, "OnRefreshingReadingFilesList", readingFiles);

            // RECENT FILES
            var recentFiles = this.Statistics_GetRecentFiles(library);
            if (recentFiles == null) { return false; }
            for (int i = 0; i < 5; i++)
            { recentFiles.AddRange(recentFiles); }
            await this.Notify(library, "OnRefreshingRecentFilesList", recentFiles);

            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.Statistics", ex); return false; }
      }

      private List<ComicFiles.ComicFileVM> Statistics_GetRecentFiles(LibraryModel library)
      {
         try
         {
            var recentFiles = this.ComicFiles[library.ID]
               .Where(file => file.ComicFile.Available)
               .Where(file => file.ComicFile.ReleaseDate != DateTime.MinValue)
               .OrderByDescending(x => x.ComicFile.ReleaseDate)
               .Take(10)
               .ToList();
            return recentFiles;
         }
         catch (Exception) { throw; }
      }

      private List<ComicFiles.ComicFileVM> Statistics_GetReadingFiles(LibraryModel library)
      {
         try
         {
            var libraryFiles = this.ComicFiles[library.ID]
               .Where(file => file.ComicFile.Available);

            // GET LAST 10 OPENED FILES
            var openedFiles = libraryFiles
               .Where(file => file.ComicFile.ReadingPercent > 0.0 && file.ComicFile.ReadingPercent < 1.0)
               .OrderByDescending(file => file.ComicFile.ReadingDate)
               .Take(10)
               .ToList();

            // GET ALL READED FILES
            var readedFiles = libraryFiles
               .Where(x => x.ComicFile.ReadingPercent == 1.0)
               .ToList();

            // REMOVE GROUPS THAT ALREADY HAS SOME OPENED FILES
            readedFiles
               .RemoveAll(x => openedFiles
                  .Select(g => g.ComicFile.FolderPath)
                  .Contains(x.ComicFile.FolderPath));

            // GROUP FILES AND MANTAIN ONLY THE MOST RECENT FILE FOR EACH GROUP
            readedFiles = readedFiles
               .GroupBy(x => x.ComicFile.FolderPath)
               .Select(x => readedFiles
                  .Where(g => g.ComicFile.FolderPath == x.Key)
                  .OrderByDescending(g => g.ComicFile.FilePath)
                  .FirstOrDefault())
               .ToList();

            // FROM THAT, TAKE THE NEXT FILE FOR EACH GROUP
            var readedNextFiles = readedFiles
               .Select(x => libraryFiles
                  .Where(f => f.ComicFile.FolderPath == x.ComicFile.FolderPath)
                  .Where(f => String.Compare(f.ComicFile.FilePath, x.ComicFile.FilePath) > 0)
                  .OrderBy(f => f.ComicFile.FilePath)
                  .Take(1)
                  .Select(f => new { f, x.ComicFile.ReadingDate })
                  .FirstOrDefault())
               .Where(x => x != null)
               .ToList();

            // UNION OPEN AND READED FILES
            var unionFiles = readedNextFiles;
            unionFiles.AddRange(openedFiles.Select(f => new { f, f.ComicFile.ReadingDate }).AsEnumerable());

            // TAKE THE LAST 10
            var readingFiles = unionFiles
               .OrderByDescending(x => x.ReadingDate)
               .Select(x => x.f)
               .Take(10)
               .ToList();

            return readingFiles;
            ;
         }
         catch (Exception) { throw; }
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
               .Where(x => !searchFiles.Select(i => i.Key).ToList().Contains(x.ComicFile.Key))
               .ToList()
               .ForEach(file => file.ComicFile.Available = false);
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

            // FEATURED FILES
            var featuredFiles = this.ComicFiles[library.ID]
               .Where(file => file.ComicFile.Available)
               .GroupBy(file => file.ComicFile.FolderPath)
               .SelectMany(file => file
                  .OrderByDescending(order => order.ComicFile.FilePath)
                  .Take(5)
                  .ToList())
               .Where(file => string.IsNullOrEmpty(file.CoverPath) || file.CoverPath == LibraryConstants.DefaultCover)
               .ToList();
            if (featuredFiles == null) { return true; }
            if (!await this.ExtractData(library, featuredFiles)) { return false; }

            // REMAINING FILES
            var remainingFiles = this.ComicFiles[library.ID]
               .Where(file => file.ComicFile.Available)
               .Where(file => string.IsNullOrEmpty(file.CoverPath) || file.CoverPath == LibraryConstants.DefaultCover)
               .OrderBy(file => file.ComicFile.FolderPath)
               .ThenByDescending(file => file.ComicFile.FilePath)
               .ToList();
            if (remainingFiles == null) { return true; }
            if (!await this.ExtractData(library, remainingFiles)) { return false; }


            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("LibraryService.ExtractData", ex); return false; }
      }

      private async Task<bool> ExtractData(LibraryModel library, List<ComicFiles.ComicFileVM> comicFiles)
      {
         try
         {
            var engine = Engines.Engine.Get(library.Type);
            if (engine == null) { return false; }

            var filesQuantity = comicFiles.Count;
            for (int fileIndex = 0; fileIndex < filesQuantity; fileIndex++)
            {
               var comicFile = comicFiles[fileIndex];
               try
               {

                  // PROGRESS
                  var progress = ((double)fileIndex / (double)filesQuantity);
                  // this.Notify(fileData.FullText, progress);

                  // CHECK IF THE COVER FILE ALREADY EXISTS
                  if (!string.IsNullOrEmpty(comicFile.CoverPath) && comicFile.CoverPath != LibraryConstants.DefaultCover)
                  { continue; }

                  // COVER EXTRACT
                  if (await engine.ExtractCover(library, comicFile.ComicFile))
                  { comicFile.CoverPath = comicFile.ComicFile.CoverPath; }
                  else { return false; }

               }
               catch (Exception ex) { throw new Exception($"Extracting Comic Data{Environment.NewLine}{comicFile.ComicFile.FilePath}", ex); }
            }

            return true;
         }
         catch (Exception) { throw; }
         finally { GC.Collect(); }
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


   }
}
