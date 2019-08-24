using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Libraries
{
   internal class LibraryService
   {
      private const string generalRecentFilesKey = "General.RecentFiles";
      private const string generalReadingFilesKey = "General.ReadingFiles";

      #region Constructor 

      private readonly Notify.NotifyVM Notify;
      public readonly Dictionary<string, LibraryModel> Libraries;
      public readonly Dictionary<string, List<ComicFiles.ComicFileVM>> ComicFiles;

      public LibraryService()
      {
         this.Notify = new Notify.NotifyVM("General");
         this.Libraries = new Dictionary<string, LibraryModel>();
         this.ComicFiles = new Dictionary<string, List<ComicFiles.ComicFileVM>>();
         Messaging.Subscribe<LibraryModel>("OnRefreshLibrary", async (library) => await this.OnRefreshLibrary(library));
      }

      #endregion


      #region InitializeLibrary
      private void InitializeLibrary(LibraryModel library)
      {
         try
         {

            if (!this.ComicFiles.ContainsKey(library.ID))
            { this.ComicFiles.Add(library.ID, new List<ComicFiles.ComicFileVM>()); }

            if (!this.Libraries.ContainsKey(library.ID))
            { this.Libraries.Add(library.ID, library); }

            if (!System.IO.Directory.Exists(LibraryConstants.CoversCachePath))
            { System.IO.Directory.CreateDirectory(LibraryConstants.CoversCachePath); }
            if (!System.IO.Directory.Exists(LibraryConstants.FilesCachePath))
            { System.IO.Directory.CreateDirectory(LibraryConstants.FilesCachePath); }

         }
         catch { }
      }
      #endregion

      #region StartupLibrary

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
            var comicsCount = "0,";

            service.InitializeLibrary(library);
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.LoadData(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.NotifyData(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.Statistics(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.LoadSyncData(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.NotifyData(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.Statistics(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            service.Notify.Send(library, false);
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            Helpers.AppCenter.TrackEvent("StartupLibrary", $"comicsCount:{comicsCount}");
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
         finally { GC.Collect(); }
      }

      #endregion

      #region RefreshLibrary

      public static async Task RefreshLibrary(LibraryModel library)
      {
         try
         { Messaging.Send("OnRefreshLibrary", library); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      private async Task OnRefreshLibrary(LibraryModel library)
      {
         try
         { await Task.Factory.StartNew(async () => await RefreshLibrary(this, library), TaskCreationOptions.LongRunning); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      private static async Task RefreshLibrary(LibraryService service, LibraryModel library)
      {
         try
         {
            var comicsCount = "0,";
            service.InitializeLibrary(library);
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.SearchData(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.LoadSyncData(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.NotifyData(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.Statistics(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.SaveSyncData(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.SaveData(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.ExtractData(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.Statistics(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.SaveSyncData(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            if (!await service.SaveData(library)) { return; }
            comicsCount += $"{service.ComicFiles[library.ID].Count},";
            Helpers.AppCenter.TrackEvent("RefreshLibrary", $"comicsCount:{comicsCount}");
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
         finally { service.Notify.Send(library, false); GC.Collect(); }
      }

      #endregion

      #region UpdateLibrary

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

      #endregion

      #region RemoveLibrary

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

      #endregion


      #region LoadAndSave

      private async Task<bool> LoadData(LibraryModel library)
      {
         try
         {
            this.Notify.Send(library, $"{library.Description}: {R.Strings.SEARCH_ENGINE_LOADING_DATABASE_DATA_MESSAGE}");

            var files = await Helpers.FileStream.ReadFile<List<ComicFiles.ComicFile>>(LibraryConstants.DatabaseFile);
            if (files == null) { return true; }

            var existingKeys = this.ComicFiles[library.ID].Select(x => x.ComicFile.Key).ToList();
            var comicFiles = files
               .Where(x => x.LibraryKey == library.ID)
               .Where(x => !existingKeys.Contains(x.Key))
               .GroupBy(x => x.Key)
               .Select(x => new { Item = x.FirstOrDefault(), Count = x.Count() })
               .Select(x => new ComicFiles.ComicFileVM(x.Item))
               .ToList();

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

      private async Task<bool> SaveData(LibraryModel library)
      {
         try
         {

            var comicFiles = this.ComicFiles[library.ID]
               .Select(x => x.ComicFile)
               .GroupBy(x => x.Key)
               .Select(x => new { Item = x.FirstOrDefault(), Count = x.Count() })
               .Select(x => x.Item)
               .ToList();
            if (comicFiles == null) { return true; }

            if (!await Helpers.FileStream.SaveFile(LibraryConstants.DatabaseFile, comicFiles)) { return false; }
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }


      #endregion

      #region NotifyData

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

      #endregion

      #region SyncData

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

      private async Task<bool> SaveSyncData(LibraryModel library)
      {
         try
         {
            var comicFiles = this.ComicFiles[library.ID].Select(x => x.ComicFile).ToList();
            return await LibrarySync.SaveSyncData(library, comicFiles);
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }
      #endregion

      #region Statistics

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
            if (!this.ComicFiles.ContainsKey(generalRecentFilesKey))
            { this.ComicFiles.Add(generalRecentFilesKey, new List<ComicFiles.ComicFileVM>()); }

            var recentFiles = this.ComicFiles[library.ID]
               .Where(file => file.ComicFile.Available)
               .Where(file => file.ComicFile.ReleaseDate != DateTime.MinValue)
               .GroupBy(file => new { file.ComicFile.FolderPath, file.ComicFile.ReleaseDate.Year, file.ComicFile.ReleaseDate.Month, file.ComicFile.ReleaseDate.Day })
               .Select(file => file.OrderByDescending(g=> g.ComicFile.ReleaseDate).FirstOrDefault())
               .OrderByDescending(x => x.ComicFile.ReleaseDate)
               .Take(25)
               .ToList();

            var generalFiles = this.ComicFiles[generalRecentFilesKey]
               .Where(x => x.ComicFile.LibraryKey != library.ID)
               .ToList();
            recentFiles = recentFiles
               .Union(generalFiles)
               .OrderByDescending(x => x.ComicFile.ReleaseDate)
               .ToList();
            this.ComicFiles[generalRecentFilesKey] = recentFiles;

            return recentFiles.Take(50).ToList();
         }
         catch (Exception) { throw; }
      }

      private List<ComicFiles.ComicFileVM> Statistics_GetReadingFiles(LibraryModel library)
      {
         try
         {

            if (!this.ComicFiles.ContainsKey(generalReadingFilesKey))
            { this.ComicFiles.Add(generalReadingFilesKey, new List<ComicFiles.ComicFileVM>()); }

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
            var generalFiles = this.ComicFiles[generalReadingFilesKey]
               .Where(x => x.ComicFile.LibraryKey != library.ID)
               .ToList();
            readingFiles = readingFiles
               .Union(generalFiles)
               .OrderByDescending(x => x.ReadingDate)
               .ToList();
            this.ComicFiles[generalReadingFilesKey] = readingFiles;


            return readingFiles.Take(10).ToList();
            ;
         }
         catch (Exception) { throw; }
      }

      #endregion

      #region SearchData

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

            // REMOVED FILES
            libraryFiles
               .Where(x => !searchFiles.Select(i => i.Key).ToList().Contains(x.ComicFile.Key))
               .ToList()
               .ForEach(file => file.ComicFile.Available = false);

            // EXISTING FILES
            var existingFiles = libraryFiles
               .Where(file => file.ComicFile.Available == true);
            foreach (var existingFile in existingFiles)
            {
               var searchFile = searchFiles.Where(x => x.Key == existingFile.ComicFile.Key).FirstOrDefault();
               if (searchFile != null)
               {
                  existingFile.ComicFile.FolderPath = searchFile.FolderPath;
                  existingFile.ComicFile.FilePath = searchFile.FilePath;
                  existingFile.ComicFile.FullText = searchFile.FullText;
                  existingFile.ComicFile.SmallText = searchFile.SmallText;
               }
            }

            // NEW FILES
            var newFiles = searchFiles
               .Where(x => !libraryFiles.Select(i => i.ComicFile.Key).ToList().Contains(x.Key))
               .Select(x => new ComicFiles.ComicFileVM(x))
               .ToList();
            libraryFiles.AddRange(newFiles);

            // CHECK FOR DUPLICITY
            var duplicatedFiles = libraryFiles
               .GroupBy(x => x.ComicFile.Key)
               .Select(x => new { Item = x.FirstOrDefault(), Count = x.Count() })
               .Where(x => x.Count > 1)
               .Select(x => x.Item.ComicFile)
               .ToList();
            if (duplicatedFiles != null && duplicatedFiles.Count != 0)
            {
               Helpers.AppCenter.TrackEvent($"Library.{library.Type.ToString()}.FoundDuplicity",
                  $"NewFiles:{newFiles.Count()}",
                  $"DuplicatedFiles:{duplicatedFiles.Count}");
               foreach (var duplicatedFile in duplicatedFiles)
               {
                  libraryFiles.RemoveAll(x => x.ComicFile.Key == duplicatedFile.Key);
                  libraryFiles.Add(new ComicFiles.ComicFileVM(duplicatedFile));
               }
            }

            var endTime = DateTime.Now;
            Helpers.AppCenter.TrackEvent($"Library.{library.Type.ToString()}.Search",
               $"ElapsedSeconds:{((int)(endTime - startTime).TotalSeconds)}",
               $"NewFiles:{newFiles.Count()}");

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
         finally { GC.Collect(); }
      }

      #endregion

      #region ExtractData

      private async Task<bool> ExtractData(LibraryModel library)
      {
         try
         {
            var startTime = DateTime.Now;

            // FEATURED FILES
            this.Notify.Send(library, $"{library.Description}: {R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_FEATURED_FILES_MESSAGE}");
            var recentFiles = this.ComicFiles[generalRecentFilesKey]
               .Where(x => x.ComicFile.LibraryKey == library.ID)
               .ToList();
            var readingFiles = this.ComicFiles[generalReadingFilesKey]
               .Where(x => x.ComicFile.LibraryKey == library.ID)
               .ToList();
            var featuredFileIDs = recentFiles
               .Union(readingFiles)
               .Select(x => x.ComicFile.Key)
               .ToList();
            var featuredFiles = this.ComicFiles[library.ID]
               .Where(file => file.ComicFile.Available)
               .Where(file => string.IsNullOrEmpty(file.CoverPath) || file.CoverPath == LibraryConstants.DefaultCover)
               .Where(file => featuredFileIDs.Contains(file.ComicFile.Key))
               .OrderBy(file => file.ComicFile.FolderPath)
               .ThenByDescending(file => file.ComicFile.FilePath)
               .ToList();
            if (featuredFiles != null)
            { if (!await this.ExtractData(library, featuredFiles)) { return false; } }

            // FIRST FIVE FILES
            this.Notify.Send(library, $"{library.Description}: {R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_FEATURED_FILES_MESSAGE}");
            var firstFiveFiles = this.ComicFiles[library.ID]
               .Where(file => file.ComicFile.Available)
               .GroupBy(file => file.ComicFile.FolderPath)
               .SelectMany(file => file
                  .OrderByDescending(order => order.ComicFile.FilePath)
                  .Take(5)
                  .ToList())
               .Where(file => string.IsNullOrEmpty(file.CoverPath) || file.CoverPath == LibraryConstants.DefaultCover)
               .ToList();
            if (firstFiveFiles == null) { return true; }
            if (!await this.ExtractData(library, firstFiveFiles)) { return false; }

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
               { "FilesCount", (firstFiveFiles.Count()+remainingFiles.Count()).ToString() }
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

                  // CHECK IF LIBRARY IS STILL AVAILABLED
                  if (library.Removed) { return false; }

                  // CHECK IF THE APP HAS GONE SLEEP
                  if (this.suspended) { return true; }

                  // PROGRESS
                  var progress = ((double)fileIndex / (double)filesQuantity);
                  this.Notify.Send(library, comicFile.ComicFile.FullText, progress);

                  var coverResult = await Task.Run<bool>(async () =>
                  {

                     // CACHE PATH
                     if (System.IO.Directory.Exists(comicFile.ComicFile.CachePath))
                     { comicFile.CachePath = comicFile.ComicFile.CachePath; }
                     else { comicFile.CachePath = string.Empty; }

                     // CHECK IF THE COVER FILE ALREADY EXISTS
                     if (!string.IsNullOrEmpty(comicFile.CoverPath) && comicFile.CoverPath != LibraryConstants.DefaultCover)
                     { return true; }
                     if (System.IO.File.Exists(comicFile.ComicFile.CoverPath))
                     {
                        comicFile.CoverPath = comicFile.ComicFile.CoverPath;
                        if (comicFile.ComicFile.ReleaseDate == DateTime.MinValue)
                        { comicFile.ComicFile.ReleaseDate = System.IO.File.GetLastWriteTime(comicFile.CoverPath); }
                        return true;
                     }

                     // COVER EXTRACT
                     if (await engine.ExtractCover(library, comicFile.ComicFile))
                     { comicFile.CoverPath = comicFile.ComicFile.CoverPath; return true; }
                     else { return false; }

                  });
                  if (!coverResult) { return false; }

                  // STATISTCS
                  if (lastFolderPath != comicFile.ComicFile.FolderPath)
                  {
                     lastFolderPath = comicFile.ComicFile.FolderPath;
                     await this.Statistics(library);
                  }

               }
               catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
               finally { GC.Collect(); }
            }

            return true;
         }
         catch (Exception) { throw; }
         finally { GC.Collect(); }
      }

      #endregion

      #region SleepAndResume

      private bool suspended = false;
      private bool suspending = false;

      public void Sleep()
      {
         suspending = true;
         var timer = new System.Timers.Timer(10000);
         timer.AutoReset = false;
         timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
         {
            timer.Stop();
            if (suspending) { suspended = true; }
            timer.Dispose();
            timer = null;
         };
         timer.Start();
      }

      public async Task Resume()
      {
         try
         {
            suspending = false;
            if (suspended)
            {
               suspended = false;
               foreach (var library in this.Libraries)
               { await RefreshLibrary(library.Value); }
            }
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      #endregion

   }
}
