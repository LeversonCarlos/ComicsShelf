using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Services
{
   public class LibraryService
   {
      private const string generalRecentFilesKey = "General.RecentFiles";
      private const string generalReadingFilesKey = "General.ReadingFiles";

      readonly Store.LibraryModel Library;
      readonly Notify.NotifyVM Notify;
      readonly Store.ILibraryStore Store;
      List<ComicFiles.ComicFileVM> LibraryFiles => this.Store.ComicFiles[this.Library.ID];

      public LibraryService(Store.LibraryModel Library)
      {
         try
         {
            this.Library = Library;
            this.Notify = new Notify.NotifyVM("General");
            this.Store = DependencyService.Get<Store.ILibraryStore>();

            if (!this.Store.ComicFiles.ContainsKey(generalRecentFilesKey))
            { this.Store.ComicFiles.Add(generalRecentFilesKey, new List<ComicFiles.ComicFileVM>()); }

            if (!this.Store.ComicFiles.ContainsKey(generalReadingFilesKey))
            { this.Store.ComicFiles.Add(generalReadingFilesKey, new List<ComicFiles.ComicFileVM>()); }

            if (!System.IO.Directory.Exists(Helpers.Constants.CoversCachePath))
            { System.IO.Directory.CreateDirectory(Helpers.Constants.CoversCachePath); }

            if (!System.IO.Directory.Exists(Helpers.Constants.FilesCachePath))
            { System.IO.Directory.CreateDirectory(Helpers.Constants.FilesCachePath); }

         }
         catch { }
      }

      #region InitializeLibrary
      private async Task InitializeLibrary()
      {
         try
         {
            this.Notify.Send(this.Library, $"{this.Library.Description}: {R.Strings.STARTUP_ENGINE_LOADING_DATABASE_MESSAGE}");
            if (!await this.LoadData()) { return; }
            if (!await this.NotifyData()) { return; }
            if (!await this.Statistics()) { return; }
            if (!await this.LoadSyncData()) { return; }
            if (!await this.NotifyData()) { return; }
            if (!await this.Statistics()) { return; }
            this.Notify.Send(this.Library, false);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
         finally { GC.Collect(); }
      }
      #endregion

      #region LoadAndSave

      private async Task<bool> LoadData()
      {
         try
         {
            this.Notify.Send(this.Library, $"{this.Library.Description}: {R.Strings.SEARCH_ENGINE_LOADING_DATABASE_DATA_MESSAGE}");

            // EXISTING FILES
            var existingKeys = this.LibraryFiles.Select(x => x.ComicFile.Key).ToList();

            // DATABASE FILES
            var databaseFiles = await Helpers.FileStream.ReadFile<List<ComicFiles.ComicFile>>(Helpers.Constants.DatabaseFile);
            if (databaseFiles == null) { return true; }

            // NEW FILES
            var comicFiles = databaseFiles
               .Where(x => x.LibraryKey == this.Library.Key)
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

            this.LibraryFiles.AddRange(comicFiles);
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private async Task<bool> SaveData()
      {
         try
         {

            var comicFiles = this.LibraryFiles
               .Select(x => x.ComicFile)
               .GroupBy(x => x.Key)
               .Select(x => new { Item = x.FirstOrDefault(), Count = x.Count() })
               .Select(x => x.Item)
               .ToList();
            if (comicFiles == null) { return true; }

            if (!await Helpers.FileStream.SaveFile(Helpers.Constants.DatabaseFile, comicFiles)) { return false; }
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      #endregion

      #region NotifyData

      private async Task<bool> NotifyData()
      {
         try
         {
            Messaging.Send<List<ComicFiles.ComicFileVM>>("OnRefreshingList", this.Library.ID, this.LibraryFiles);
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private async Task<bool> NotifyData(string prefix, List<ComicFiles.ComicFileVM> comicFiles)
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

      private async Task<bool> LoadSyncData()
      {
         try
         {

            /*
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
            */

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private async Task<bool> SaveSyncData()
      {
         try
         {
            /*
            var comicFiles = this.ComicFiles[library.ID].Select(x => x.ComicFile).ToList();
            return await LibrarySync.SaveSyncData(library, comicFiles);
            */
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      #endregion


      #region Statistics

      private async Task<bool> Statistics()
      {
         try
         {

            // READING FILES
            var readingFiles = this.Statistics_GetReadingFiles();
            if (readingFiles == null) { return false; }
            await this.NotifyData("OnRefreshingReadingFilesList", readingFiles);

            // RECENT FILES
            var recentFiles = this.Statistics_GetRecentFiles();
            if (recentFiles == null) { return false; }
            await this.NotifyData("OnRefreshingRecentFilesList", recentFiles);

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private List<ComicFiles.ComicFileVM> Statistics_GetRecentFiles()
      {
         try
         {

            var recentFiles = this.LibraryFiles
               .Where(file => file.ComicFile.Available)
               .Where(file => file.ComicFile.ReleaseDate != DateTime.MinValue)
               .GroupBy(file => new { file.ComicFile.FolderPath, file.ComicFile.ReleaseDate.Year, file.ComicFile.ReleaseDate.Month, file.ComicFile.ReleaseDate.Day })
               .Select(file => file.OrderByDescending(g => g.ComicFile.ReleaseDate).FirstOrDefault())
               .OrderByDescending(x => x.ComicFile.ReleaseDate)
               .Take(25)
               .ToList();

            var generalFiles = this.Store.ComicFiles[generalRecentFilesKey]
               .Where(x => x.ComicFile.LibraryKey != this.Library.ID)
               .ToList();

            recentFiles = recentFiles
               .Union(generalFiles)
               .OrderByDescending(x => x.ComicFile.ReleaseDate)
               .ToList();
            this.Store.ComicFiles[generalRecentFilesKey] = recentFiles;

            return recentFiles.OrderByDescending(x=> x.ComicFile.ReleaseDate).Take(50).ToList();
         }
         catch (Exception) { throw; }
      }

      private List<ComicFiles.ComicFileVM> Statistics_GetReadingFiles()
      {
         try
         {

            var libraryFiles = this.LibraryFiles
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
            var generalFiles = this.Store.ComicFiles[generalReadingFilesKey]
               .Where(x => x.ComicFile.LibraryKey != this.Library.ID)
               .ToList();
            readingFiles = readingFiles
               .Union(generalFiles)
               .OrderByDescending(x => x.ReadingDate)
               .ToList();

            this.Store.ComicFiles[generalReadingFilesKey] = readingFiles;
            return readingFiles.OrderByDescending(x => x.ReadingDate).Take(10).ToList();            
         }
         catch (Exception) { throw; }
      }

      #endregion




   }
}
