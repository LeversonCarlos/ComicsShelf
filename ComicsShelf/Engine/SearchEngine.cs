using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class Search : BaseEngine, IDisposable
   {

      #region Refresh

      public static async void RefreshAll()
      {
         var libraries = App.Settings.Libraries;
         foreach (var library in libraries)
         {
            await Search.Execute(library, false);
         }
         foreach (var library in libraries)
         {
            await Task.Factory.StartNew(async () => await Search.Execute(library, true), TaskCreationOptions.LongRunning);
         }
      }

      public static async void Refresh(vTwo.Libraries.Library library)
      {
         await Search.Execute(library, false);
         await Task.Factory.StartNew(async () => await Search.Execute(library, true), TaskCreationOptions.LongRunning);
      }

      #endregion

      #region Execute
      public static async Task Execute(vTwo.Libraries.Library library, bool deepSearch)
      {
         var searchTitle = (deepSearch ? "DeepSearch" : "Search");
         try
         {

            // TRACK
            var initializeTime = DateTime.Now;
            AppCenter.TrackEvent($"{searchTitle}: Initialize");

            // EXECUTE
            using (var engine = new Search(library))
            {
               await engine.InitializeData();
               if (deepSearch) { await engine.SearchComicFiles(); }
               await engine.PrepareStructure();
               await engine.ExtractAlreadyExistingData();
               if (deepSearch) { await engine.ExtractFeaturedData(); }
               if (deepSearch) { await engine.ExtractRemainingData(); }
            }

            // TRACK
            var milliseconds = (long)(DateTime.Now - initializeTime).TotalMilliseconds;
            AppCenter.TrackEvent($"{searchTitle}: Finalize", "milliseconds", milliseconds.ToString());

         }
         catch (Exception ex) { AppCenter.TrackEvent(searchTitle, ex); await App.ShowMessage(ex); }
         finally { GC.Collect(); }
      }
      #endregion


      #region Constructor
      public Search(vTwo.Libraries.Library library) : base(library.LibraryID)
      {
         this.library = library;
      }
      #endregion

      #region Properties
      private vTwo.Libraries.Library library { get; set; }
      private Views.Home.LibraryData libraryData { get; set; }
      private Library.ILibraryService libraryService { get; set; }
      private List<Helpers.Database.ComicFile> ComicFiles { get; set; }
      #endregion

      #region InitializeData
      private async Task InitializeData()
      {
         try
         {
            this.Notify(R.Strings.SEARCH_ENGINE_LOADING_DATABASE_DATA_MESSAGE);

            // LOAD COMIC FILES FROM DATABASE
            await Task.Run(() =>
            {
               this.ComicFiles = App.Database
                  .Table<Helpers.Database.ComicFile>()
                  .Where(x => x.LibraryPath == this.library.LibraryID)
                  .GroupBy(x => new { x.Key })
                  .Select(x => x.FirstOrDefault())
                  .ToList();
            });

            // ADD FEATURED LIBRARIES
            /*
            if (App.HomeData.Libraries.Count == 0)
            { App.HomeData.Libraries.Add(new Views.Home.FeaturedData()); }
            */

            // INITIALIZE LIBRARY DATA 
            this.libraryData = App.HomeData.Libraries
               .Where(x => x.ComicFolder.LibraryPath == this.library.LibraryID)
               .FirstOrDefault();
            if (this.libraryData == null)
            {
               var comicLibrary = new Helpers.Database.ComicFolder
               {
                  LibraryPath = this.library.LibraryID,
                  FullPath = this.library.LibraryID,
                  Text = this.library.Description,
                  Key = $"{this.library.LibraryID}"
               };
               this.libraryData = new Views.Home.LibraryData(comicLibrary);
               App.HomeData.Libraries.Add(this.libraryData);
            }

            // LIBRARY SERVICE
            this.libraryService = Library.LibraryService.Get(this.library);

         }
         catch (Exception) { throw; }
      }
      #endregion


      #region SearchComicFiles
      private async Task SearchComicFiles()
      {
         try
         {
            this.Notify(R.Strings.SEARCH_ENGINE_SEARCHING_COMIC_FILES_MESSAGE);
            this.ComicFiles.AsParallel().ForAll(x => x.Available = false);
            var settings = App.Settings;

            // LOCATE COMIC FILES
            var comicFiles = await this.libraryService.SearchFilesAsync(library);
            if (comicFiles == null || comicFiles.Count == 0) { return; }

            // ACTIVATE FOUND FILES
            var foundKeys = comicFiles.Select(x => x.Key).ToList();
            this.ComicFiles
               .Where(x => foundKeys.Contains(x.Key))
               .AsParallel()
               .ForAll(x => x.Available = true);

            // ALREADY EXISTING FILES
            var existingKeys = this.ComicFiles.Select(x => x.Key).ToList();
            comicFiles.RemoveAll(x => existingKeys.Contains(x.Key));

            // COVER PATH   
            comicFiles
               .AsParallel()
               .ForAll(x => x.CoverPath = $"{settings.Paths.CoversCachePath}{settings.Paths.Separator}{x.Key}.jpg");

            // SAVE 
            await Task.Run(() =>
            {
               foreach (var comicFile in comicFiles)
               {
                  this.ComicFiles.Add(comicFile);
                  App.Database.Insert(comicFile);
               }
            });

            await Task.Run(() =>
            { this.ComicFiles.AsParallel().ForAll(x => App.Database.Update(x)); });

         }
         catch (Exception) { throw; }
      }
      #endregion


      #region PrepareStructure
      private async Task PrepareStructure()
      {
         try
         {
            this.Notify(R.Strings.SEARCH_ENGINE_PREPARING_FOLDERS_STRUCTURE_MESSAGE);

            this.Notify("Preparing Files", 0.0);
            this.PrepareFiles();

            this.Notify("Preparing Folders", 0.6);
            this.PrepareFolders();

            this.Notify("Preparing Sections", 0.8);
            this.PrepareSections();

            this.Notify("Done", 1.0);
         }
         catch (Exception) { throw; }
      }
      #endregion

      #region PrepareFiles
      private void PrepareFiles()
      {
         try
         {
            List<string> comicKeys = null;

            // REORDER THE LISTS
            var countA = this.ComicFiles.Count;
            this.ComicFiles = this.ComicFiles
               .Where(x => x.Available)
               .Where(x => x.FullPath.ToLower().EndsWith(".cbz"))
               .OrderBy(x => x.LibraryPath)
               .ThenBy(x => x.FullPath)
               .ToList();
            var countB = this.ComicFiles.Count;

            // REMOVE NOEXISTING FILES
            comicKeys = this.ComicFiles.Where(x => x.LibraryPath == library.LibraryID).Select(x => x.Key).ToList();
            var homeDataFiles = this.libraryData.Files
               .Where(x => !comicKeys.Contains(x.ComicFile.Key))
               .ToList();
            foreach (var homeDataFile in homeDataFiles)
            { this.libraryData.Files.Remove(homeDataFile); }

            // DISCARD ALREADY EXISTING FILES
            comicKeys = this.libraryData.Files.Select(x => x.ComicFile.Key).ToList();
            this.ComicFiles.RemoveAll(x => comicKeys.Contains(x.Key));

            // ADD NEW FILES
            this.ComicFiles.ForEach(comicFile => this.libraryData.Files.Add(new Views.File.FileData(comicFile)));
            // Statistics.Execute();

         }
         catch (Exception) { throw; }
      }
      #endregion

      #region PrepareFolders
      private void PrepareFolders()
      {
         try
         {
            List<string> comicKeys = null;

            // PREPARE FOLDERS
            var comicFolders = this.libraryData.Files
               .GroupBy(x => new { x.ComicFile.ParentPath })
               .Select(x => new Helpers.Database.ComicFolder
               {
                  LibraryPath = this.library.LibraryID,
                  FullPath = x.Key.ParentPath,
                  ParentPath = (string.IsNullOrEmpty(x.Key.ParentPath) ? "" : System.IO.Path.GetDirectoryName(x.Key.ParentPath)),
                  Text = (string.IsNullOrEmpty(x.Key.ParentPath) ? this.library.Description : System.IO.Path.GetFileNameWithoutExtension(x.Key.ParentPath)),
                  Key = $"{this.library.LibraryID}|{x.Key.ParentPath}"
               })
               .ToList();

            // REMOVE NOEXISTING FOLDERS
            comicKeys = comicFolders.Select(x => x.Key).ToList();
            var homeDataFolders = this.libraryData.Folders
               .Where(x => !comicKeys.Contains(x.ComicFolder.Key))
               .ToList();
            foreach (var homeDataFolder in homeDataFolders)
            { this.libraryData.Folders.Remove(homeDataFolder); }

            // DISCARD ALREADY EXISTING FOLDERS
            comicKeys = this.libraryData.Folders
               .Select(x => x.ComicFolder.Key).ToList();
            comicFolders.RemoveAll(x => comicKeys.Contains(x.Key));

            // ADD NEW FOLDERS
            comicFolders.ForEach(comicFolder => this.libraryData.Folders.Add(new Views.Folder.FolderData(comicFolder)));

            // REVIEW FOLDER's FILES
            foreach (var folder in this.libraryData.Folders)
            {

               // REMOVE NOEXISTING FILES
               comicKeys = this.libraryData.Files
                  .Where(x => x.ComicFile.ParentPath == folder.ComicFolder.FullPath)
                  .Select(x => x.ComicFile.Key)
                  .ToList();
               var folderFiles = folder.Files
                  .Where(x => !comicKeys.Contains(x.ComicFile.Key))
                  .ToList();
               foreach (var folderFile in folderFiles)
               { folder.Files.Remove(folderFile); }

               // ADD NEW FILES
               comicKeys = folder.Files.Select(x => x.ComicFile.Key).ToList();
               folderFiles = this.libraryData.Files
                  .Where(x => x.ComicFile.ParentPath == folder.ComicFolder.FullPath && !comicKeys.Contains(x.ComicFile.Key))
                  .ToList();
               foreach (var folderFile in folderFiles)
               { folder.Files.Add(folderFile); }

               folder.HasFiles = folder.Files.Count != 0;
            }

         }
         catch (Exception) { throw; }
      }
      #endregion

      #region PrepareSections
      private void PrepareSections()
      {
         try
         {
            List<string> comicKeys = null;

            // PREPARE SECTIONS
            var comicSections = this.libraryData.Folders
               .GroupBy(x => new { x.ComicFolder.ParentPath })
               .Select(x => new Helpers.Database.ComicFolder
               {
                  LibraryPath = this.library.LibraryID,
                  FullPath = x.Key.ParentPath,
                  Text = (string.IsNullOrEmpty(x.Key.ParentPath) ? this.library.Description : x.Key.ParentPath.Replace(this.library.LibraryID, "")),
                  Key = $"{this.library.LibraryID}|{x.Key.ParentPath}"
               })
               .ToList();

            // REMOVE NOEXISTING SECTIONS
            comicKeys = comicSections.Select(x => x.Key).ToList();
            var dataSections = this.libraryData.Sections
               .Where(x => !comicKeys.Contains(x.ComicFolder.Key))
               .ToList();
            foreach (var dataSection in dataSections)
            {
               foreach(var dataFolder in dataSection.Folders)
               {
                  foreach(var dataFile in dataFolder.Files)
                  { dataFolder.Files.Remove(dataFile); }
                  dataSection.Folders.Remove(dataFolder);
               }
               this.libraryData.Sections.Remove(dataSection);
            }

            // DISCARD ALREADY EXISTING SECTIONS
            comicKeys = this.libraryData.Sections
               .Select(x => x.ComicFolder.Key).ToList();
            comicSections.RemoveAll(x => comicKeys.Contains(x.Key));

            // ADD NEW SECTIONS
            comicSections.ForEach(comicSection => this.libraryData.Sections.Add(new Views.Folder.FolderData(comicSection)));

            // REVIEW SECTION's FOLDERS
            foreach (var section in this.libraryData.Sections)
            {

               // REMOVE NOEXISTING FOLDERS
               comicKeys = this.libraryData.Folders
                  .Where(x => x.ComicFolder.ParentPath == section.ComicFolder.FullPath)
                  .Select(x => x.ComicFolder.Key)
                  .ToList();
               var sectionFolders = section.Folders
                  .Where(x => !comicKeys.Contains(x.ComicFolder.Key))
                  .ToList();
               foreach (var sectionFolder in sectionFolders)
               { section.Folders.Remove(sectionFolder); }

               // ADD NEW FOLDERS
               comicKeys = section.Folders.Select(x => x.ComicFolder.Key).ToList();
               sectionFolders = this.libraryData.Folders
                  .Where(x => x.ComicFolder.ParentPath == section.ComicFolder.FullPath && !comicKeys.Contains(x.ComicFolder.Key))
                  .ToList();
               foreach (var sectionFolder in sectionFolders)
               { section.Folders.Add(sectionFolder); }

               section.HasFolders = section.Folders.Count != 0;
            }

         }
         catch (Exception) { throw; }
      }
      #endregion


      #region ExtractAlreadyExistingData
      private async Task ExtractAlreadyExistingData()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_FEATURED_FILES_MESSAGE);
            var fileList = this.libraryData.Files
               .OrderBy(x => x.FullPath)
               .ToList();
            await Task.Run(() =>
            {
               foreach (var fileData in fileList)
               {
                  if (System.IO.File.Exists(fileData.ComicFile.CoverPath))
                  {
                     fileData.CoverPath = fileData.ComicFile.CoverPath;
                     this.ApplyFolderData(fileData);
                  }
               }
            });
         }
         catch (Exception) { throw; }
      }
      #endregion

      #region ExtractFeaturedData
      private async Task ExtractFeaturedData()
      {
         try
         {

            // FEATURED FILES
            this.Notify(R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_FEATURED_FILES_MESSAGE);
            var fileList = App.HomeData.ReadingFiles
               .Union(App.HomeData.RecentFiles)
               .Union(App.HomeData.TopRatedFiles)
               .Where(x => x.ComicFile.LibraryPath == this.library.LibraryID)
               .Where(x => string.IsNullOrEmpty(x.CoverPath))
               .ToList();
            await this.ExtractData(fileList);

            // FIRST FILE FROM EACH FOLDER
            this.Notify(R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_FOLDERS_FILES_MESSAGE);
            fileList = this.libraryData.Files
               .OrderBy(x => x.FullPath)
               .GroupBy(x => new { x.ComicFile.ParentPath })
               .Select(x => x.FirstOrDefault())
               .Where(x => string.IsNullOrEmpty(x.CoverPath))
               .ToList();
            await this.ExtractData(fileList);

         }
         catch (Exception) { throw; }
      }
      #endregion

      #region ExtractRemainingData
      private async Task ExtractRemainingData()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_REMAINING_FILES_MESSAGE);
            var fileList = this.libraryData.Files
               .Where(x => string.IsNullOrWhiteSpace(x.CoverPath))
               .OrderBy(x => x.FullPath)
               .ToList();
            await this.ExtractData(fileList);
         }
         catch (Exception) { throw; }
      }
      #endregion

      #region ExtractData
      private async Task ExtractData(List<Views.File.FileData> fileList)
      {
         try
         {

            // LOOP THROUGH FILES
            var filesQuantity = fileList.Count;
            for (int fileIndex = 0; fileIndex < filesQuantity; fileIndex++)
            {
               try
               {
                  var fileData = fileList[fileIndex];
                  var progress = ((double)fileIndex / (double)filesQuantity);
                  this.Notify(fileData.FullText, progress);

                  // CHECK IF THE COVER FILE ALREADY EXISTS
                  if (!string.IsNullOrEmpty(fileData.CoverPath)) { continue; }

                  // COVER EXTRACT
                  if (await libraryService.ExtractCoverAsync(this.library, fileData.ComicFile))
                  {
                     fileData.CoverPath = fileData.ComicFile.CoverPath;
                     this.ApplyFolderData(fileData);
                  }

               }
               catch (Exception ex) { Engine.AppCenter.TrackEvent("Extracting Comic Data", ex); }
            }

         }
         catch (Exception) { throw; }
         finally { GC.Collect(); }
      }
      #endregion


      #region ApplyFolderData
      private void ApplyFolderData(Views.File.FileData fileData)
      {
         try
         {
            var folders = this.libraryData.Folders
               .Where(x => x.ComicFolder.LibraryPath == this.library.LibraryID)
               .Where(x => x.ComicFolder.FullPath == fileData.ComicFile.ParentPath)
               .Where(x => string.IsNullOrEmpty(x.CoverPath))
               .AsParallel();
            folders.ForAll(x => x.CoverPath = fileData.CoverPath);
         }
         catch { }
      }
      #endregion


      #region Dispose
      public new void Dispose()
      {
         base.Dispose();
         this.ComicFiles = null;
      }
      #endregion

   }
}