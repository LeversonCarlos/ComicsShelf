﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace ComicsShelf.Engine
{
   internal class Search : BaseEngine, IDisposable
   {

      #region Initialize
      public static async Task Initialize() {
         await Execute(false);

         var task = Execute(true);
         await task.ConfigureAwait(false);
         task.Start();
         // Task.Factory.StartNew(async () => { await Execute(true); }, TaskCreationOptions.LongRunning);
      }
      #endregion

      #region Execute
      public static async Task Execute(bool searchNewFiles)
      {
         try
         {
            using (var engine = new Search())
            {
               engine.TrackEvent("Search: Initializing");

               await engine.LoadDatabaseData();
               engine.TrackEvent("Search: Loading Database Data");

               if (searchNewFiles)
               {
                  await engine.SearchComicFiles();
                  engine.TrackEvent("Search: Searching Comic Files");
               }

               await engine.PrepareStructure();
               engine.TrackEvent("Search: Preparing Structure");

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
                  .GroupBy(x => new { x.LibraryPath, x.Key })
                  .Select(x => x.FirstOrDefault())
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

            await Task.Run(() => {
               this.ComicFiles.AsParallel().ForAll(x => App.Database.Update(x));
            });

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

            this.Notify("Preparing Folders", 0.4);
            this.PrepareFolders();

            this.Notify("Preparing Sections", 0.6);
            this.PrepareSections();

            this.Notify("Preparing Libraries", 0.8);
            this.PrepareLibraries();

            this.Notify("Done", 1.0);
         }
         catch (Exception ex) { throw; }
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

            // LOOP THROUGH LIBRARIES
            foreach (var library in App.Settings.Paths.Libraries)
            {

               /*
               if (App.HomeData.Files.Count != 0)
               { this.ComicFiles.RemoveAll(x => x.FullText.Contains("White Knight [2018]")); }
               */

               // REMOVE NOEXISTING FILES
               comicKeys = this.ComicFiles.Where(x => x.LibraryPath == library.LibraryPath).Select(x => x.Key).ToList();
               var homeDataFiles = App.HomeData.Files
                  .Where(x => x.ComicFile.LibraryPath == library.LibraryPath && !comicKeys.Contains(x.ComicFile.Key))
                  .ToList();
               foreach (var homeDataFile in homeDataFiles)
               { App.HomeData.Files.Remove(homeDataFile); }

               // DISCARD ALREADY EXISTING FILES
               comicKeys = App.HomeData.Files.Where(x => x.ComicFile.LibraryPath == library.LibraryPath).Select(x => x.ComicFile.Key).ToList();
               this.ComicFiles
                  .RemoveAll(x => x.LibraryPath == library.LibraryPath && comicKeys.Contains(x.Key));

               // ADD NEW FILES
               this.ComicFiles.ForEach(comicFile => App.HomeData.Files.Add(new Views.File.FileData(comicFile)));
               // App.HomeData.Files.AddRange(comicFiles.Select(x => new Views.File.FileData(x)));
               // Statistics.Execute();

            }

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
            var comicFolders = App.HomeData.Files
               .GroupBy(x => new { x.ComicFile.LibraryPath, x.ComicFile.ParentPath })
               .Select(x => new Helpers.Database.ComicFolder
               {
                  LibraryPath = x.Key.LibraryPath,
                  FullPath = x.Key.ParentPath,
                  ParentPath = System.IO.Path.GetDirectoryName(x.Key.ParentPath),
                  Text = System.IO.Path.GetFileNameWithoutExtension(x.Key.ParentPath),
                  Key = $"{x.Key.LibraryPath}|{x.Key.ParentPath}"
               })
               .ToList();

            // REMOVE NOEXISTING FOLDERS
            comicKeys = comicFolders.Select(x => x.Key).ToList();
            var homeDataFolders = App.HomeData.Folders
               .Where(x => !comicKeys.Contains(x.ComicFolder.Key))
               .ToList();
            foreach(var homeDataFolder in homeDataFolders)
            { App.HomeData.Folders.Remove(homeDataFolder); }

            // DISCARD ALREADY EXISTING FOLDERS
            comicKeys = App.HomeData.Folders.Select(x => x.ComicFolder.Key).ToList();
            comicFolders
               .RemoveAll(x => comicKeys.Contains(x.Key));

            // ADD NEW FOLDERS
            comicFolders.ForEach(comicFolder => App.HomeData.Folders.Add(new Views.Folder.FolderData(comicFolder)));

            // REVIEW FOLDER's FILES
            foreach (var folder in App.HomeData.Folders)
            {

               // REMOVE NOEXISTING FILES
               comicKeys = App.HomeData.Files
                  .Where(x => x.ComicFile.LibraryPath == folder.ComicFolder.LibraryPath && x.ComicFile.ParentPath == folder.ComicFolder.FullPath)
                  .Select(x => x.ComicFile.Key)
                  .ToList();
               var folderFiles = folder.Files
                  .Where(x => x.ComicFile.LibraryPath == folder.ComicFolder.LibraryPath && !comicKeys.Contains(x.ComicFile.Key))
                  .ToList();
               foreach (var folderFile in folderFiles)
               { folder.Files.Remove(folderFile); }

               // ADD NEW FILES
               comicKeys = folder.Files.Select(x => x.ComicFile.Key).ToList();
               folderFiles = App.HomeData.Files
                  .Where(x => x.ComicFile.LibraryPath == folder.ComicFolder.LibraryPath && x.ComicFile.ParentPath == folder.ComicFolder.FullPath && !comicKeys.Contains(x.ComicFile.Key))
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
            var comicSections = App.HomeData.Folders
               .GroupBy(x => new { x.ComicFolder.LibraryPath, x.ComicFolder.ParentPath })
               .Select(x => new Helpers.Database.ComicFolder
               {
                  LibraryPath = x.Key.LibraryPath,
                  FullPath = x.Key.ParentPath,
                  Text = x.Key.ParentPath.Replace(x.Key.LibraryPath, ""),
                  Key = $"{x.Key.LibraryPath}|{x.Key.ParentPath}"
               })
               .ToList();

            // REMOVE NOEXISTING SECTIONS
            comicKeys = comicSections.Select(x => x.Key).ToList();
            var homeDataSections = App.HomeData.Sections
               .Where(x => !comicKeys.Contains(x.ComicFolder.Key))
               .ToList();
            foreach (var homeDataSection in homeDataSections)
            { App.HomeData.Sections.Remove(homeDataSection); }

            // DISCARD ALREADY EXISTING SECTIONS
            comicKeys = App.HomeData.Sections.Select(x => x.ComicFolder.Key).ToList();
            comicSections
               .RemoveAll(x => comicKeys.Contains(x.Key));

            // ADD NEW SECTIONS
            comicSections.ForEach(comicFolder => App.HomeData.Sections.Add(new Views.Folder.FolderData(comicFolder)));

            // REVIEW SECTION's FOLDERS
            foreach (var section in App.HomeData.Sections)
            {

               // REMOVE NOEXISTING FOLDERS
               comicKeys = App.HomeData.Folders
                  .Where(x => x.ComicFolder.LibraryPath == section.ComicFolder.LibraryPath && x.ComicFolder.ParentPath == section.ComicFolder.FullPath)
                  .Select(x => x.ComicFolder.Key)
                  .ToList();
               var sectionFolders = section.Folders
                  .Where(x => x.ComicFolder.LibraryPath == section.ComicFolder.LibraryPath && !comicKeys.Contains(x.ComicFolder.Key))
                  .ToList();
               foreach (var sectionFolder in sectionFolders)
               { section.Folders.Remove(sectionFolder); }

               // ADD NEW FOLDERS
               comicKeys = section.Folders.Select(x => x.ComicFolder.Key).ToList();
               sectionFolders = App.HomeData.Folders
                  .Where(x => x.ComicFolder.LibraryPath == section.ComicFolder.LibraryPath && x.ComicFolder.ParentPath == section.ComicFolder.FullPath && !comicKeys.Contains(x.ComicFolder.Key))
                  .ToList();
               foreach (var sectionFolder in sectionFolders)
               { section.Folders.Add(sectionFolder); }

               section.HasFolders = section.Folders.Count != 0;
            }

         }
         catch (Exception) { throw; }
      }
      #endregion

      #region PrepareLibraries
      private void PrepareLibraries()
      {
         try
         {
            List<string> comicKeys = null;

            // ADD NEW LIBRARIES
            if (App.HomeData.Libraries.Count == 0)
            {
               var comicLibraries = App.Settings.Paths.Libraries
                  .Select(x => new Helpers.Database.ComicFolder
                  {
                     LibraryPath = x.LibraryPath,
                     FullPath = x.LibraryPath,
                     Text = x.LibraryText,
                     Key = $"{x.LibraryPath}"
                  })
                  .OrderBy(x => x.Text)
                  .ToList();
               App.HomeData.Libraries.Add(new Views.Home.FeaturedData());
               comicLibraries.ForEach(comicLibrary => App.HomeData.Libraries.Add(new Views.Home.LibraryData(comicLibrary)));
            }

            // REVIEW LIBRARIES's SECTIONS
            foreach (var library in App.HomeData.Libraries)
            {
               if (library.IsFeaturedPage) { continue; }

               // REMOVE NOEXISTING SECTIONS
               comicKeys = App.HomeData.Sections
                  .Where(x => x.ComicFolder.LibraryPath == library.ComicFolder.LibraryPath)
                  .Select(x => x.ComicFolder.Key)
                  .ToList();
               var librarySections = library.Folders
                  .Where(x => x.ComicFolder.LibraryPath == library.ComicFolder.LibraryPath && !comicKeys.Contains(x.ComicFolder.Key))
                  .ToList();
               foreach (var librarySection in librarySections)
               { library.Folders.Remove(librarySection); }

               // ADD NEW FOLDERS
               comicKeys = library.Folders.Select(x => x.ComicFolder.Key).ToList();
               librarySections = App.HomeData.Sections
                  .Where(x => x.ComicFolder.LibraryPath == library.ComicFolder.LibraryPath && !comicKeys.Contains(x.ComicFolder.Key))
                  .ToList();
               foreach (var librarySection in librarySections)
               { library.Folders.Add(librarySection); }

               library.HasFolders = library.Folders.Count != 0;
            }

         }
         catch (Exception) { throw; }
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