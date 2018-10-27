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
            System.Diagnostics.Debug.WriteLine("Search Engine Start");
            using (var engine = new Search())
            {
               await engine.LoadDatabaseData();
               await engine.SearchComicFiles();
               /*
               if (await engine.IncludePublicDomainExamples()) {
                  await engine.LoadDatabaseData();
                  await engine.SearchComicFiles();
               }
               */
               await engine.PrepareFolders();
               await engine.ExtractComicData();

               /*
               Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Statistics.Execute());
               */
            }
            System.Diagnostics.Debug.WriteLine("Search Engine Finish");
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

      #region Properties
      private List<Helpers.Database.ComicFile> ComicFiles { get; set; }
      private List<Helpers.Database.ComicFolder> ComicFolders { get; set; }
      #endregion

      #region LoadDatabaseData
      private async Task LoadDatabaseData()
      {
         try
         {
            this.Notify(R.Strings.SEARCH_ENGINE_LOADING_DATABASE_DATA_MESSAGE);

            await Task.Run(() =>
            {

               this.ComicFiles = App.Database
                  .Table<Helpers.Database.ComicFile>()
                  .Where(x => x.LibraryPath == App.Settings.Paths.LibraryPath)
                  .ToList();

               this.ComicFolders = App.Database
                  .Table<Helpers.Database.ComicFolder>()
                  .Where(x => x.LibraryPath == App.Settings.Paths.LibraryPath)
                  .ToList();

            });

         }
         catch (Exception ex) { throw; }
      }
      #endregion


      #region SearchComicFiles
      private async Task SearchComicFiles()
      {
         try
         {
            this.Notify(R.Strings.SEARCH_ENGINE_SEARCHING_COMIC_FILES_MESSAGE);

            // LOCATE COMICS LIST
            var fileList = await this.FileSystem.GetFiles(App.Settings.Paths.LibraryPath);
            // fileList = fileList.Take(10).ToArray();

            // MARK AVAILABLE FILES
            this.ComicFiles.Where(x => fileList.Contains(x.FullPath)).ToList().ForEach(x => x.Available = true);
            this.ComicFiles.Where(x => !fileList.Contains(x.FullPath)).ToList().ForEach(x => x.Available = false);

            // REMOVE FILES ALREADY LOADED
            var comicFilesCurrentList = this.ComicFiles.Select(x => x.FullPath).ToList();
            if (comicFilesCurrentList != null && comicFilesCurrentList.Count != 0)
            { fileList = fileList.Where(x => !comicFilesCurrentList.Contains(x)).ToArray(); }

            // LOOP THROUGH FILE LIST
            var fileQuantity = fileList.Length;
            if (fileQuantity != 0) {
               for (int fileIndex = 0; fileIndex < fileQuantity; fileIndex++)
               {
                  string filePath = "";
                  try
                  {
                     filePath = fileList[fileIndex];
                     var progress = ((double)fileIndex / (double)fileQuantity);
                     this.Notify(filePath, progress);

                     var comicFile = await this.SearchComicFiles_GetFile(filePath);
                     await this.SearchComicFiles_GetFolder(comicFile);

                  }
                  catch (Exception fileException)
                  { if (!await App.ConfirmMessage($"->Path:{filePath}\n->File:{fileIndex}/{fileQuantity}\n->Exception:{fileException}")) { Environment.Exit(0); } }
               }
            }

            // REORDER THE LISTS
            this.ComicFiles = this.ComicFiles
               .OrderBy(x => x.FullPath)
               .ToList();
            this.ComicFolders = this.ComicFolders
               .OrderBy(x => x.FullPath)
               .ToList();
            App.HomeData.NoComics = (this.ComicFiles == null || this.ComicFiles.Count == 0);

         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region SearchComicFiles_GetFile
      private async Task<Helpers.Database.ComicFile> SearchComicFiles_GetFile(string filePath)
      {
         try
         {

            // INITIALIZE
            var comicFile = new Helpers.Database.ComicFile
            {
               LibraryPath = App.Settings.Paths.LibraryPath,
               FullPath = filePath,
               Available = true
            };
            comicFile.Key = $"{comicFile.LibraryPath}|{comicFile.FullPath}";

            // PARENT PATH
            var folderPath = System.IO.Path.GetDirectoryName(filePath);
            var folderText = System.IO.Path.GetFileNameWithoutExtension(folderPath);
            comicFile.ParentPath = folderPath;

            // TEXT
            comicFile.FullText = System.IO.Path.GetFileNameWithoutExtension(filePath).Trim();
            if (!string.IsNullOrEmpty(folderText))
            { comicFile.SmallText = comicFile.FullText.Replace(folderText, ""); }
            if (string.IsNullOrEmpty(comicFile.SmallText))
            { comicFile.SmallText = comicFile.FullText; }

            // COVER PATH   
            var coverPath = $"{filePath}.jpg";
            coverPath = coverPath
               .Replace(App.Settings.Paths.Separator, "_")
               .Replace("#", "")
               .Replace(" ", "_")
               .Replace("___", "_")
               .Replace("__", "_");
            comicFile.CoverPath = $"{App.Settings.Paths.CoversCachePath}{App.Settings.Paths.Separator}{coverPath}";

            // INSERT COMIC FILE
            this.ComicFiles.Add(comicFile);
            await Task.Run(() => App.Database.Insert(comicFile));

            return comicFile;
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region SearchComicFiles_GetFolder
      private async Task SearchComicFiles_GetFolder(Helpers.Database.ComicFile comicFile)
      {
         try
         {

            // INITIALIZE
            var folderPath = comicFile.ParentPath;

            // ADD FOLDER STRUCTURE
            while (!string.IsNullOrEmpty(folderPath) && folderPath != App.Settings.Paths.LibraryPath)
            {

               // CHECK IF FOLDER ALREADY EXISTS
               var comicFolder = this.ComicFolders
                  .Where(x => x.FullPath == folderPath)
                  .FirstOrDefault();
               if (comicFolder != null) { break; }

               // PROPERTIES
               var folderText = System.IO.Path.GetFileNameWithoutExtension(folderPath);
               var folderParent = System.IO.Path.GetDirectoryName(folderPath);

               // ADD FOLDER
               comicFolder = new Helpers.Database.ComicFolder
               {
                  LibraryPath = App.Settings.Paths.LibraryPath,
                  FullPath = folderPath,
                  ParentPath = folderParent,
                  Text = folderText
               };
               comicFolder.Key = $"{comicFolder.LibraryPath}|{comicFolder.FullPath}";
               this.ComicFolders.Add(comicFolder);
               await Task.Run(() => App.Database.Insert(comicFolder));

               folderPath = folderParent;
            }

         }
         catch (Exception ex) { throw; }
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
            var comicFiles = this.ComicFiles.Where(x => x.Available).ToList();
            await Task.Run(() => {
               comicFiles.ForEach(x => App.HomeData.Files.Add(new Views.File.FileData(x)));
               // App.HomeData.Files.AddRange(comicFiles.Select(x => new Views.File.FileData(x)));
               // Statistics.Execute();
            });

            // COMIC FOLDERS
            this.Notify("Loading Folders", 0.3);
            List<Helpers.Database.ComicFolder> comicFolders = null;
            await Task.Run(() => {
               comicFolders = comicFiles
                  .GroupBy(x => x.ParentPath)
                  .Select(x => new Helpers.Database.ComicFolder {
                     LibraryPath = App.Settings.Paths.LibraryPath,
                     FullPath = x.Key,
                     ParentPath = System.IO.Path.GetDirectoryName(x.Key),
                     Text = System.IO.Path.GetFileNameWithoutExtension(x.Key),
                     Key = $"{App.Settings.Paths.LibraryPath}|{x.Key}"
                  })
                  .ToList();
            });
            this.Notify("Writing Folders", 0.6);
            foreach (var comicFolder in comicFolders)
            {
               var folder = new Views.Folder.FolderData(comicFolder);
               folder.Files.AddRange(App.HomeData.Files.Where(x => x.ComicFile.ParentPath == comicFolder.FullPath));
               folder.HasFiles = folder.Files.Count != 0;
               await Task.Run(() => App.HomeData.Folders.Add(folder));
            }

            // COMIC SECTIONS
            this.Notify("Loading Sections", 0.9);
            var comicSections = comicFolders
               .GroupBy(x => x.ParentPath)
               .Select(x => x.FirstOrDefault())
               .Select(x => new Helpers.Database.ComicFolder {
                  FullPath = x.ParentPath,
                  LibraryPath = x.LibraryPath,
                  Key = $"{x.LibraryPath}|{x.ParentPath}"
               })
               .ToList();
            await Task.Run(() => {
               foreach (var comicSection in comicSections)
               {

                  comicSection.Text = comicSection.FullPath
                     .Replace(comicSection.LibraryPath, "");
                  if (string.IsNullOrEmpty(comicSection.Text))
                  { comicSection.Text = R.Strings.HOME_FOLDERS_PAGE_TITLE; }

                  var section = new Views.Folder.FolderData(comicSection);
                  section.Folders.AddRange(App.HomeData.Folders.Where(x => x.ComicFolder.ParentPath == comicSection.FullPath));
                  section.HasFolders = section.Folders.Count != 0;

                  App.HomeData.FolderSections.Add(section);
               }
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
            this.Notify(R.Strings.STARTUP_ENGINE_EXTRACTING_COMICS_DATA_MESSAGE);

            var fileList = App.HomeData.Files
               .OrderBy(x => x.FullPath)
               .ToList();
            Parallel.ForEach(fileList, fileData => {
               if (System.IO.File.Exists(fileData.ComicFile.CoverPath))
               {
                  fileData.CoverPath = fileData.ComicFile.CoverPath;
                  this.ExtractComicData_Folder(fileData);
               }
            });

            // LOCATE FILES WITHOUT COVER
            fileList = App.HomeData.Files
               .Where(x => string.IsNullOrWhiteSpace(x.CoverPath))
               .OrderBy(x => x.FullPath)
               .ToList();
            var filesQuantity = fileList.Count;

            // LOOP THROUGH FILES
            for (int fileIndex = 0; fileIndex < filesQuantity; fileIndex++)
            {
               var fileData = fileList[fileIndex];
               var progress = ((double)fileIndex / (double)filesQuantity);
               this.Notify(fileData.FullText, progress);

               // VALIDATE
               // if (fileData.FullPath.ToLower().EndsWith(".cbr")) { continue; }

               // EXECUTE
               // await Task.Factory.StartNew(async () => {
               //Parallel.Invoke(async () => {
               if (await this.ExtractComicData_File(fileData))
               { this.ExtractComicData_Folder(fileData); }
               //});
               // }, TaskCreationOptions.RunContinuationsAsynchronously);

            }

         }
         catch (Exception ex) { throw; }
         finally { GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced); }

      }
      #endregion

      #region ExtractComicData_File
      private async Task<bool> ExtractComicData_File(Views.File.FileData fileData)
      {
         try
         {

            // CHECK IF THE COVER FILE ALREADY EXISTS
            if (!string.IsNullOrEmpty(fileData.CoverPath)) { return false; }
            //if (System.IO.File.Exists(fileData.ComicFile.CoverPath)) { return false; }

            // COVER EXTRACT
            await this.FileSystem.CoverExtract(App.Settings, App.Database, fileData.ComicFile);

            // APPLY PROPERTY SO THE VIEW GETS REFRESHED
            fileData.CoverPath = fileData.ComicFile.CoverPath;
            return true;

         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region ExtractComicData_Folder
      private void ExtractComicData_Folder(Views.File.FileData fileData)
      {
         try
         {

            var folders = App.HomeData.Folders
               .Where(x => x.ComicFolder.FullPath == fileData.ComicFile.ParentPath)
               .Where(x => string.IsNullOrEmpty(x.CoverPath))
               .AsParallel();
            folders.ForAll(x => x.CoverPath = fileData.CoverPath);

         }
         catch { }
      }
      #endregion


      #region IncludePublicDomainExamples
      private async Task<bool> IncludePublicDomainExamples()
      {
         try
         {

            // CHECK LIBRARY
            if (!App.HomeData.NoComics) { return false; }
            /*
            var libraryPath = $"{App.Settings.Paths.DataPath}{App.Settings.Paths.Separator}Examples";

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var manifestResourceNames = assembly.GetManifestResourceNames();
            var exampleNames = manifestResourceNames
               .Where(x => x.StartsWith("ComicsShelf.Examples."))
               .ToList();

            foreach (var exampleName in exampleNames)
            {
               var examplePath = exampleName.Replace("ComicsShelf.Examples.", "");
               var examplePathArray = examplePath.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

               var folderName = examplePathArray[0]
                  .Replace("__", " ")
                  .Replace("_", " ")
                  .Trim();
               var fileName = examplePathArray[1] + "." + examplePathArray[2];
               var folderPath = $"{libraryPath}{App.Settings.Paths.Separator}{folderName}";
               var filePath = $"{folderPath}{App.Settings.Paths.Separator}{fileName}";

               if (!System.IO.File.Exists(filePath)) {
                  if (!System.IO.Directory.Exists(folderPath))
                  { System.IO.Directory.CreateDirectory(folderPath); }
                  var exampleStream = assembly.GetManifestResourceStream(exampleName);
                  System.IO.FileStream fileStream = null;
                  await Task.Run(() => fileStream = System.IO.File.Open(filePath, System.IO.FileMode.OpenOrCreate));
                  await exampleStream.CopyToAsync(fileStream);
                  await fileStream.FlushAsync();
                  fileStream.Dispose();
                  GC.Collect();
               }

            }

            App.Settings.Paths.LibraryPath = libraryPath;
            */
            return true;
         }
         catch (Exception ex) { throw; }
      }
      #endregion


      #region Dispose
      public new void Dispose()
      {
         base.Dispose();
         this.ComicFiles = null;
         this.ComicFolders = null;
      }
      #endregion

   }
}