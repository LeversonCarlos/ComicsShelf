using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class Search : BaseEngine, IDisposable
   {

      #region Execute
      public static async Task Execute()
      {
         try
         {
            using (var engine = new Search())
            {
               engine.TrackEvent("Search: Initializing");

               await engine.LoadDatabaseData();
               engine.TrackEvent("Search: Loading Database Data");

               await engine.SearchComicFiles();
               engine.TrackEvent("Search: Searching Comic Files");

               await engine.PrepareFolders();
               engine.TrackEvent("Search: Preparing Folders Structure");

               await engine.ExtractComicData();
               engine.TrackEvent("Search: Extracting Data Info");

            }
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

      #region Properties
      private List<Helpers.Database.ComicFile> ComicFiles { get; set; }
      #endregion

      #region TrackEvent
      DateTime TrackEventTime = DateTime.MinValue;
      private void TrackEvent(string text)
      {
         var now = DateTime.Now;
         long milliseconds = 0;
         if (TrackEventTime != DateTime.MinValue)
         { milliseconds = (long)(now - TrackEventTime).TotalMilliseconds; }
         AppCenter.TrackEvent(text, "milliseconds", milliseconds.ToString());
         TrackEventTime = now;
      }
      #endregion

      #region LoadDatabaseData
      private async Task LoadDatabaseData()
      {
         try
         {
            this.Notify(R.Strings.SEARCH_ENGINE_LOADING_DATABASE_DATA_MESSAGE);
            await Task.Run(() =>
            {
               var librariesPaths = App.Settings.Paths.Libraries
                  .Select(x => x.LibraryPath)
                  .ToArray();
               this.ComicFiles = App.Database
                  .Table<Helpers.Database.ComicFile>()
                  .Where(x => librariesPaths.Contains(x.LibraryPath))
                  .ToList();
            });
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

            // LOOP THROUGH LIBRARIES
            foreach (var library in App.Settings.Paths.Libraries)
            {
               var libraryService = Library.LibraryService.Get(library);


               // LOCATE COMIC FILES
               var comicFiles = await libraryService.SearchFilesAsync(library);
               if (comicFiles == null || comicFiles.Count == 0) { continue; }

               // ACTIVATE FOUND FILES
               var foundKeys = comicFiles.Select(x => x.Key).ToList();
               this.ComicFiles
                  .Where(x => x.LibraryPath == library.LibraryPath && foundKeys.Contains(x.Key))
                  .AsParallel()
                  .ForAll(x => x.Available = true);

               // ALREADY EXISTING FILES
               var existingKeys = this.ComicFiles.Where(x => x.LibraryPath == library.LibraryPath).Select(x => x.Key).ToList();
               comicFiles
                  .RemoveAll(x => x.LibraryPath == library.LibraryPath && existingKeys.Contains(x.Key));

               // COVER PATH   
               comicFiles
                  .AsParallel()
                  .ForAll(x => x.CoverPath = $"{App.Settings.Paths.CoversCachePath}{App.Settings.Paths.Separator}{x.Key}.jpg");

               await Task.Run(() =>
               {
                  foreach (var comicFile in comicFiles)
                  {
                     this.ComicFiles.Add(comicFile);
                     App.Database.Insert(comicFile);
                  }
               });

            }

            // REORDER THE LISTS
            this.ComicFiles = this.ComicFiles
               .Where(x => x.Available)
               .OrderBy(x => x.LibraryPath)
               .ThenBy(x => x.FullPath)
               .ToList();

         }
         catch (Exception) { throw; }
      }
      #endregion


      #region PrepareFolders
      private async Task PrepareFolders()
      {
         try
         {
            this.Notify(R.Strings.SEARCH_ENGINE_PREPARING_FOLDERS_STRUCTURE_MESSAGE);

            // COMIC FILES
            this.Notify("Loading Files", 0.0);
            await Task.Run(() =>
            {
               this.ComicFiles.ForEach(comicFile => App.HomeData.Files.Add(new Views.File.FileData(comicFile)));
               // App.HomeData.Files.AddRange(comicFiles.Select(x => new Views.File.FileData(x)));
               // Statistics.Execute();
            });

            // COMIC FOLDERS
            this.Notify("Loading Folders", 0.3);
            var comicFolders = this.ComicFiles
               .GroupBy(x => new { x.LibraryPath, x.ParentPath })
               .Select(x => new Helpers.Database.ComicFolder
               {
                  LibraryPath = x.Key.LibraryPath,
                  FullPath = x.Key.ParentPath,
                  ParentPath = System.IO.Path.GetDirectoryName(x.Key.ParentPath),
                  Text = System.IO.Path.GetFileNameWithoutExtension(x.Key.ParentPath),
                  Key = $"{x.Key.LibraryPath}|{x.Key.ParentPath}"
               })
               .ToList();
            comicFolders
               .ForEach(comicFolder =>
               {
                  var folderData = new Views.Folder.FolderData(comicFolder);
                  var folderFiles = App.HomeData.Files.Where(x => x.ComicFile.LibraryPath == comicFolder.LibraryPath && x.ComicFile.ParentPath == comicFolder.FullPath);
                  folderData.Files.AddRange(folderFiles);
                  folderData.HasFiles = folderData.Files.Count != 0;
                  App.HomeData.Folders.Add(folderData);
               });

            // COMIC SECTIONS
            this.Notify("Loading Sections", 0.6);
            var comicSections = comicFolders
               .GroupBy(x => new { x.LibraryPath, x.ParentPath })
               .Select(x => new Helpers.Database.ComicFolder
               {
                  LibraryPath = x.Key.LibraryPath,
                  FullPath = x.Key.ParentPath,
                  Text = x.Key.ParentPath.Replace(x.Key.LibraryPath, ""),
                  Key = $"{x.Key.LibraryPath}|{x.Key.ParentPath}"
               })
               .ToList();
            var comicSectionsData = new List<Views.Folder.FolderData>();
            comicSections
               .ForEach(comicSection =>
               {
                  // if (string.IsNullOrEmpty(comicSection.Text)) { comicSection.Text = R.Strings.HOME_FOLDERS_PAGE_TITLE; }

                  var sectionData = new Views.Folder.FolderData(comicSection);
                  var sectionFolders = App.HomeData.Folders.Where(x => x.ComicFolder.LibraryPath == comicSection.LibraryPath && x.ComicFolder.ParentPath == comicSection.FullPath);
                  sectionData.Folders.AddRange(sectionFolders);
                  sectionData.HasFolders = sectionData.Folders.Count != 0;
                  comicSectionsData.Add(sectionData);
               });

            // LIBRARIES
            this.Notify("Loading Libraries", 0.8);
            var comicLibraries = App.Database.Table<Helpers.Database.Library>()
               .Select(x => new Helpers.Database.ComicFolder
               {
                  LibraryPath = x.LibraryPath,
                  FullPath = x.LibraryPath,
                  Text = x.LibraryText,
                  Key = $"{x.LibraryPath}"
               })
               .OrderBy(x => x.Text)
               .ToList();
            await Task.Run(() =>
            {
               comicLibraries
                  .ForEach(comicLibrary =>
                  {
                     var libraryData = new Views.Home.LibraryData(comicLibrary);
                     var librarySections = comicSectionsData.Where(x => x.ComicFolder.LibraryPath == comicLibrary.LibraryPath);
                     libraryData.Folders.AddRange(librarySections);
                     libraryData.HasFolders = libraryData.Folders.Count != 0;
                     App.HomeData.Libraries.Add(libraryData);
                  });
            });

            this.Notify("Done", 1.0);
         }
         catch (Exception ex) { throw; }
      }
      #endregion


      #region ExtractComicData

      private async Task ExtractComicData()
      {
         try
         {

            // ALREADY EXISTING COVERS 
            this.Notify(R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_REMAINING_FILES_MESSAGE);
            var fileList = App.HomeData.Files
               .OrderBy(x => x.ComicFile.LibraryPath)
               .ThenBy(x => x.FullPath)
               .ToList();
            await Task.Run(() => {
               foreach (var fileData in fileList)
               {
                  if (System.IO.File.Exists(fileData.ComicFile.CoverPath))
                  {
                     fileData.CoverPath = fileData.ComicFile.CoverPath;
                     this.ExtractComicData_ApplyFolderData(fileData);
                  }
               }
            });

            // FIRST FILE FROM EACH FOLDER
            this.Notify(R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_FOLDERS_FILES_MESSAGE);
            fileList = App.HomeData.Files
               .OrderBy(x => x.ComicFile.LibraryPath).ThenBy(x => x.FullPath)
               .GroupBy(x => new { x.ComicFile.LibraryPath, x.ComicFile.ParentPath })
               .Select(x => x.FirstOrDefault())
               .Where(x => string.IsNullOrEmpty(x.CoverPath))
               .ToList();
            await this.ExtractComicData(fileList);

            // FEATURED FILES
            this.Notify(R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_FEATURED_FILES_MESSAGE);
            // Statistics.Execute();
            fileList = App.HomeData.ReadingFiles
               .Union(App.HomeData.RecentFiles)
               .Union(App.HomeData.TopRatedFiles)
               .Where(x => string.IsNullOrEmpty(x.CoverPath))
               .ToList();
            await this.ExtractComicData(fileList);

            // ALL REMAINING FILES WITHOUT COVER
            this.Notify(R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_REMAINING_FILES_MESSAGE);
            fileList = App.HomeData.Files
               .Where(x => string.IsNullOrWhiteSpace(x.CoverPath))
               .OrderBy(x => x.ComicFile.LibraryPath).ThenBy(x => x.FullPath)
               .ToList();
            await this.ExtractComicData(fileList);

         }
         catch (Exception) { throw; }
         finally { GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced); }
      }

      private async Task ExtractComicData(List<Views.File.FileData> fileList)
      {
         try
         {

            // LIBRARY SERVICES
            var libraryServices = App.Settings.Paths.Libraries
               .Select(x => new
               {
                  Library = x,
                  Path = x.LibraryPath,
                  Service = Library.LibraryService.Get(x)
               })
               .ToList();

            // LOOP THROUGH FILES
            var filesQuantity = fileList.Count;
            for (int fileIndex = 0; fileIndex < filesQuantity; fileIndex++)
            {
               var fileData = fileList[fileIndex];
               var progress = ((double)fileIndex / (double)filesQuantity);
               this.Notify(fileData.FullText, progress);

               // CHECK IF THE COVER FILE ALREADY EXISTS
               if (!string.IsNullOrEmpty(fileData.CoverPath)) { continue; }

               // COVER EXTRACT
               var library = libraryServices
                  .Where(x => x.Path == fileData.ComicFile.LibraryPath)
                  .FirstOrDefault();
               await library.Service.ExtractCoverAsync(library.Library, fileData.ComicFile);

               // APPLY PROPERTY SO THE VIEW GETS REFRESHED
               fileData.CoverPath = fileData.ComicFile.CoverPath;
               this.ExtractComicData_ApplyFolderData(fileData); 

            }

         }
         catch (Exception) { throw; }
      }

      #endregion

      #region ExtractComicData_ApplyFolderData
      private void ExtractComicData_ApplyFolderData(Views.File.FileData fileData)
      {
         try
         {
            var folders = App.HomeData.Folders
               .Where(x => x.ComicFolder.LibraryPath == fileData.ComicFile.LibraryPath && x.ComicFolder.FullPath == fileData.ComicFile.ParentPath)
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