﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Libraries
{
   internal class LibraryService
   {

      private readonly Notify.NotifyVM Notify;
      public readonly Dictionary<string, LibraryModel> Libraries;
      public readonly Dictionary<string, List<ComicFiles.ComicFileVM>> ComicFiles;
      public LibraryService()
      {
         this.Notify = new Notify.NotifyVM("General");
         this.Libraries = new Dictionary<string, LibraryModel>();
         this.ComicFiles = new Dictionary<string, List<ComicFiles.ComicFileVM>>();
      }


      public static async Task StartupLibrary(LibraryModel library)
      {
         try
         {
            var service = DependencyService.Get<LibraryService>();
            if (service == null) { return; }
            await StartupLibrary(service, library);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      private static async Task StartupLibrary(LibraryService service, LibraryModel library)
      {
         try
         {
            service.Notify.Send(library, $"{library.Description}: {R.Strings.STARTUP_ENGINE_LOADING_DATABASE_MESSAGE}");

            if (!service.ComicFiles.ContainsKey(library.ID))
            { service.ComicFiles.Add(library.ID, new List<ComicFiles.ComicFileVM>()); }

            if (!System.IO.Directory.Exists(LibraryConstants.CoversCachePath))
            { System.IO.Directory.CreateDirectory(LibraryConstants.CoversCachePath); }
            if (!System.IO.Directory.Exists(LibraryConstants.FilesCachePath))
            { System.IO.Directory.CreateDirectory(LibraryConstants.FilesCachePath); }

            if (!service.Libraries.ContainsKey(library.ID))
            { service.Libraries.Add(library.ID, library); }

            if (!await service.LoadData(library)) { return; }
            if (!await service.NotifyData(library)) { return; }
            if (!await service.Statistics(library)) { return; }
            if (!await service.LoadSyncData(library)) { return; }
            if (!await service.NotifyData(library)) { return; }
            if (!await service.Statistics(library)) { return; }
            service.Notify.Send(library, false);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
         finally { GC.Collect(); }
      }


      public static async Task RefreshLibrary(LibraryModel library)
      {
         try
         {
            var service = DependencyService.Get<LibraryService>();
            if (service == null) { return; }
            await RefreshLibrary(service, library);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      private static async Task RefreshLibrary(LibraryService service, LibraryModel library)
      {
         try
         {
            if (!await service.SearchData(library)) { return; }
            if (!await service.LoadSyncData(library)) { return; }
            if (!await service.NotifyData(library)) { return; }
            if (!await service.ExtractData(library)) { return; }
            if (!await service.Statistics(library)) { return; }
            if (!await service.SaveSyncData(library)) { return; }
            if (!await service.SaveData(library)) { return; }
            service.Notify.Send(library, false);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
         finally { GC.Collect(); }
      }


      public async Task UpdateLibrary(LibraryModel library)
      {
         try
         {
            if (!await this.Statistics(library)) { return; }
            if (!await this.SaveSyncData(library)) { return; }
            if (!await this.SaveData(library)) { return; }
            this.Notify.Send(library, false);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      public static async Task RemoveLibrary(LibraryModel library)
      {
         try
         {
            var service = DependencyService.Get<LibraryService>();
            if (service == null) { return; }
            service.ComicFiles[library.ID] = new List<ComicFiles.ComicFileVM>();
            await service.Statistics(library);
            service.ComicFiles.Remove(library.ID);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }



      private async Task<bool> LoadData(LibraryModel library)
      {
         try
         {
            this.Notify.Send(library, $"{library.Description}: {R.Strings.SEARCH_ENGINE_LOADING_DATABASE_DATA_MESSAGE}");

            var files = await Helpers.FileStream.ReadFile<List<ComicFiles.ComicFile>>(LibraryConstants.DatabaseFile);
            if (files == null) { return true; }

            var comicFiles = files.Where(x => x.LibraryKey == library.ID).Select(x => new ComicFiles.ComicFileVM(x)).ToList();
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
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private async Task<bool> NotifyData(LibraryModel library)
      {
         try
         {
            Messaging.Send<List<ComicFiles.ComicFileVM>>("OnRefreshingList", library.ID, this.ComicFiles[library.ID]);
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private async Task<bool> NotifyData(LibraryModel library, string prefix, List<ComicFiles.ComicFileVM> comicFiles)
      {
         try
         {
            Messaging.Send<List<ComicFiles.ComicFileVM>>(prefix, comicFiles);
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private async Task<bool> LoadSyncData(LibraryModel library)
      {
         try
         {

            var syncData = await LibrarySync.LoadSyncData(library);
            if (syncData == null) { return true; }

            if (!this.ComicFiles.ContainsKey(library.ID)) { return true; }
            var comicFiles = this.ComicFiles[library.ID];
            if (comicFiles == null) { return true; }

            if (comicFiles.Count > 0 && syncData.Count > 0)
            {
               foreach (var syncFile in syncData)
               {
                  var comicFile = comicFiles.Where(x => x.ComicFile.Key == syncFile.Key).FirstOrDefault();
                  if (comicFile == null)
                  { comicFile = comicFiles.Where(x => x.ComicFile.OldKey == syncFile.Key).FirstOrDefault(); }
                  if (comicFile != null) { comicFile.Set(syncFile.ToComicFile()); }
               }
            }

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private async Task<bool> Statistics(LibraryModel library)
      {
         try
         {

            // READING FILES
            var readingFiles = this.Statistics_GetReadingFiles(library);
            if (readingFiles == null) { return false; }
            await this.NotifyData(library, "OnRefreshingReadingFilesList", readingFiles);

            // RECENT FILES
            var recentFiles = this.Statistics_GetRecentFiles(library);
            if (recentFiles == null) { return false; }
            await this.NotifyData(library, "OnRefreshingRecentFilesList", recentFiles);

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private List<ComicFiles.ComicFileVM> Statistics_GetRecentFiles(LibraryModel library)
      {
         try
         {
            const string generalKey = "General.RecentFiles";
            if (!this.ComicFiles.ContainsKey(generalKey))
            { this.ComicFiles.Add(generalKey, new List<ComicFiles.ComicFileVM>()); }

            var recentFiles = this.ComicFiles[library.ID]
               .Where(file => file.ComicFile.Available)
               .Where(file => file.ComicFile.ReleaseDate != DateTime.MinValue)
               .OrderByDescending(x => x.ComicFile.ReleaseDate)
               .Take(25)
               .ToList();

            var generalFiles = this.ComicFiles[generalKey]
               .Where(x => x.ComicFile.LibraryKey != library.ID)
               .ToList();
            recentFiles = recentFiles
               .Union(generalFiles)
               .OrderByDescending(x => x.ComicFile.ReleaseDate)
               .ToList();
            this.ComicFiles[generalKey] = recentFiles;

            return recentFiles.Take(50).ToList();
         }
         catch (Exception) { throw; }
      }

      private List<ComicFiles.ComicFileVM> Statistics_GetReadingFiles(LibraryModel library)
      {
         try
         {

            const string generalKey = "General.ReadingFiles";
            if (!this.ComicFiles.ContainsKey(generalKey))
            { this.ComicFiles.Add(generalKey, new List<ComicFiles.ComicFileVM>()); }

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

            // ATTACH GENERAL
            var generalFiles = this.ComicFiles[generalKey]
               .Where(x => x.ComicFile.LibraryKey != library.ID)
               .ToList();
            readingFiles = readingFiles
               .Union(generalFiles)
               .OrderByDescending(x => x.ReadingDate)
               .ToList();
            this.ComicFiles[generalKey] = readingFiles;


            return readingFiles.Take(10).ToList();
            ;
         }
         catch (Exception) { throw; }
      }

      private async Task<bool> SearchData(LibraryModel library)
      {
         try
         {
            var startTime = DateTime.Now;
            this.Notify.Send(library, $"{library.Description}: {R.Strings.SEARCH_ENGINE_SEARCHING_COMIC_FILES_MESSAGE}");

            var engine = Engines.Engine.Get(library.Type);
            if (engine == null) { return false; }
            var libraryFiles = this.ComicFiles[library.ID];

            var searchFiles = await engine.SearchFiles(library);
            if (searchFiles == null) { return true; }

            libraryFiles
               .Where(x => !searchFiles.Select(i => i.Key).ToList().Contains(x.ComicFile.Key))
               .ToList()
               .ForEach(file => file.ComicFile.Available = false);
            var newFiles = searchFiles
               .Where(x => !libraryFiles.Select(i => i.ComicFile.Key).ToList().Contains(x.Key))
               .Select(x => new ComicFiles.ComicFileVM(x))
               .ToList();
            libraryFiles.AddRange(newFiles);

            var endTime = DateTime.Now;
            var trackProps = new Dictionary<string, string> {
               { "ElapsedSeconds", ((int)(endTime-startTime).TotalSeconds).ToString() },
               { "NewFiles", newFiles.Count().ToString() }
            };
            Helpers.AppCenter.TrackEvent($"Library.{library.Type.ToString()}.Search", trackProps);

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
         finally { GC.Collect(); }
      }

      private async Task<bool> ExtractData(LibraryModel library)
      {
         try
         {
            var startTime = DateTime.Now;

            // FEATURED FILES
            this.Notify.Send(library, $"{library.Description}: {R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_FEATURED_FILES_MESSAGE}");
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
            this.Notify.Send(library, $"{library.Description}: {R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_REMAINING_FILES_MESSAGE}");
            var remainingFiles = this.ComicFiles[library.ID]
               .Where(file => file.ComicFile.Available)
               .Where(file => string.IsNullOrEmpty(file.CoverPath) || file.CoverPath == LibraryConstants.DefaultCover)
               .OrderBy(file => file.ComicFile.FolderPath)
               .ThenByDescending(file => file.ComicFile.FilePath)
               .ToList();
            if (remainingFiles == null) { return true; }
            if (!await this.ExtractData(library, remainingFiles)) { return false; }

            var endTime = DateTime.Now;
            var trackProps = new Dictionary<string, string> {
               { "ElapsedMinutes", ((int)(endTime-startTime).TotalMinutes).ToString() },
               { "FilesCount", (featuredFiles.Count()+remainingFiles.Count()).ToString() }
            };
            Helpers.AppCenter.TrackEvent($"Library.{library.Type.ToString()}.ExtractData", trackProps);

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private async Task<bool> ExtractData(LibraryModel library, List<ComicFiles.ComicFileVM> comicFiles)
      {
         try
         {
            var engine = Engines.Engine.Get(library.Type);
            if (engine == null) { return false; }

            var lastFolderPath = "";
            var filesQuantity = comicFiles.Count;
            for (int fileIndex = 0; fileIndex < filesQuantity; fileIndex++)
            {
               var comicFile = comicFiles[fileIndex];
               try
               {

                  // PROGRESS
                  var progress = ((double)fileIndex / (double)filesQuantity);
                  this.Notify.Send(library, comicFile.ComicFile.FullText, progress);

                  // CACHE PATH
                  if (System.IO.Directory.Exists(comicFile.ComicFile.CachePath))
                  { comicFile.CachePath = comicFile.ComicFile.CachePath; }
                  else { comicFile.CachePath = string.Empty; }

                  // CHECK IF THE COVER FILE ALREADY EXISTS
                  if (!string.IsNullOrEmpty(comicFile.CoverPath) && comicFile.CoverPath != LibraryConstants.DefaultCover)
                  { continue; }
                  if (System.IO.File.Exists(comicFile.ComicFile.CoverPath))
                  {
                     comicFile.CoverPath = comicFile.ComicFile.CoverPath;
                     if (comicFile.ComicFile.ReleaseDate == DateTime.MinValue)
                     { comicFile.ComicFile.ReleaseDate = System.IO.File.GetLastWriteTime(comicFile.CoverPath); }
                     continue;
                  }

                  // COVER EXTRACT
                  if (await engine.ExtractCover(library, comicFile.ComicFile))
                  { comicFile.CoverPath = comicFile.ComicFile.CoverPath; }
                  else { return false; }

                  // STATISTCS
                  if (lastFolderPath != comicFile.ComicFile.FolderPath)
                  {
                     lastFolderPath = comicFile.ComicFile.FolderPath;
                     await this.Statistics(library);
                  }

               }
               catch (Exception ex) { throw new Exception($"Extracting Comic Data{Environment.NewLine}{comicFile.ComicFile.FilePath}", ex); }
               finally { GC.Collect(); }
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
            var comicFiles = this.ComicFiles[library.ID].Select(x => x.ComicFile).ToList();
            return await LibrarySync.SaveSyncData(library, comicFiles);
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
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
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
