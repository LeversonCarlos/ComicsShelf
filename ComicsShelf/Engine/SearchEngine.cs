using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class Search : BaseEngine, IDisposable
   {

      #region Execute
      public static async void Execute()
      {
         try
         {
            System.Diagnostics.Debug.WriteLine("Search Engine Start");
            using (var engine = new Search())
            {
               await engine.LoadDatabaseData();
               await engine.SearchComicFiles();
               if (await engine.IncludePublicDomainExamples())
               {
                  await engine.LoadDatabaseData();
                  await engine.SearchComicFiles();
               }
               await engine.PrepareStructures_Folder();
               await engine.PrepareStructures_File();
               await engine.AnalyseFoldersAvailability();
               engine.DefineFirstFolder();
               await engine.ExtractComicData();
               /*
               Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Statistics.Execute());
               Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Cover.Execute(engine.ComicFoldersDictionary));
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
      private Dictionary<string, Views.Folder.FolderData> ComicFoldersDictionary { get; set; }
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
            this.ComicFoldersDictionary = new Dictionary<string, Views.Folder.FolderData>();

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
            var fileName = System.IO.Path.GetFileName(filePath);
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
                  FullText = folderText
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


      #region PrepareStructures_Folder
      private async Task PrepareStructures_Folder()
      {
         try
         {
            this.Notify(R.Strings.SEARCH_ENGINE_PREPARING_FOLDERS_STRUCTURE_MESSAGE);

            // LOOP THROUGH FOLDER LIST
            var loopQuantity = this.ComicFolders.Count;
            for (int loopIndex = 0; loopIndex < loopQuantity; loopIndex++)
            {
               var DATA = this.ComicFolders[loopIndex];
               try
               {
                  var progress = ((double)loopIndex / (double)loopQuantity);
                  this.Notify(DATA.FullPath, progress);

                  // PARENT FOLDER
                  Views.Folder.FolderData parentFolder = App.HomeData;
                  if (!string.IsNullOrEmpty(DATA.ParentPath))
                  { parentFolder = this.ComicFoldersDictionary[DATA.ParentPath]; }
                  if (parentFolder == null) { throw new Exception($"Could not find Parent:[{DATA.ParentPath}] for Folder:[{DATA.FullPath}]."); }

                  // ADD FOLDER
                  var folder = new Views.Folder.FolderData(DATA);
                  parentFolder.Folders.Add(folder);
                  parentFolder.HasFolders = true;
                  this.ComicFoldersDictionary.Add(DATA.FullPath, folder);
               }
               catch (Exception loopException)
               { if (!await App.ConfirmMessage($"->Path:{DATA.FullPath}\n->File:{loopIndex}/{loopQuantity}\n->Exception:{loopException}")) { Environment.Exit(0); } }
            }

         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region PrepareStructures_File
      private async Task PrepareStructures_File()
      {
         try
         {
            this.Notify(R.Strings.SEARCH_ENGINE_PREPARING_FILES_STRUCTURE_MESSAGE);

            // LOOP THROUGH FILE LIST
            var loopQuantity = this.ComicFiles.Count;
            for (int loopIndex = 0; loopIndex < loopQuantity; loopIndex++)
            {
               var DATA = this.ComicFiles[loopIndex];
               try
               {
                  var progress = ((double)loopIndex / (double)loopQuantity);
                  this.Notify(DATA.FullPath, progress);
                  if (!DATA.Available) { continue; }

                  // PARENT FOLDER
                  Views.Folder.FolderData parentFolder = App.HomeData;
                  if (!string.IsNullOrEmpty(DATA.ParentPath))
                  { parentFolder = this.ComicFoldersDictionary[DATA.ParentPath]; }
                  if (parentFolder == null) { throw new Exception($"Could not find Parent:[{DATA.ParentPath}] for Folder:[{DATA.FullPath}]."); }

                  // COMIC FILE
                  var comicFile = new Views.File.FileData(DATA);
                  parentFolder.Files.Add(comicFile);
                  parentFolder.HasFiles = true;
                  App.HomeData.Files.Add(comicFile);

                  // await ExtractFileData(App.Settings, App.Database, parentFolder, comicFile);

               }
               catch (Exception loopException)
               { if (!await App.ConfirmMessage($"->Path:{DATA.FullPath}\n->File:{loopIndex}/{loopQuantity}\n->Exception:{loopException}")) { Environment.Exit(0); } }
            }

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

            // var files = System.IO.Directory.GetFiles(App.Settings.Paths.CoversCachePath);
            // Parallel.ForEach(files, file => { System.IO.File.Delete(file); });

            // LOOP THROUGH FILES
            var filesQuantity = App.HomeData.Files.Count;
            for (int fileIndex = 0; fileIndex < filesQuantity; fileIndex++)
            {
               var fileData = App.HomeData.Files[fileIndex];
               var progress = ((double)fileIndex / (double)filesQuantity);
               this.Notify(fileData.FullText, progress);

               // VALIDATE
               if (fileData.FullPath.ToLower().EndsWith(".cbr")) { continue; }

               // EXECUTE
               await this.ExtractComicData_File(fileData);
               await this.ExtractComicData_Folder(fileData);

            }

         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region ExtractComicData_File
      private async Task ExtractComicData_File(Views.File.FileData fileData)
      {
         try
         {

            // CHECK IF THE COVER FILE ALREADY EXISTS
            if (System.IO.File.Exists(fileData.ComicFile.CoverPath))
            { fileData.CoverPath = fileData.ComicFile.CoverPath; return; }

            // OPEN ZIP ARCHIVE
            using (var fileSystem = Helpers.FileSystem.Get())
            {
               using (var zipArchive = await fileSystem.GetZipArchive(App.Settings, fileData.FullPath))
               {

                  // LOCATE FIRST JPG ENTRY
                  var zipEntry = zipArchive.Entries
                     .Where(x => x.Name.ToLower().EndsWith(".jpg"))
                     .OrderBy(x => x.Name)
                     .Take(1)
                     .FirstOrDefault();

                  // OPEN STREAM
                  using (System.IO.Stream zipStream = zipEntry.Open())
                  {

                     // RELEASE DATE
                     fileData.ComicFile.ReleaseDate = App.Database.GetDate(zipEntry.LastWriteTime.DateTime.ToLocalTime());
                     await Task.Run(() => App.Database.Update(fileData.ComicFile));

                     // COVER THUMBNAIL
                     await fileSystem.Thumbnail(zipStream, fileData.ComicFile.CoverPath);

                  }

               }
            }

            // APPLY PROPERTY SO THE VIEW GETS REFRESHED
            fileData.CoverPath = fileData.ComicFile.CoverPath;

         }
         catch (Exception ex) { throw; }
         finally { GC.Collect(); }
      }
      #endregion

      #region ExtractComicData_Folder
      private async Task ExtractComicData_Folder(Views.File.FileData fileData)
      {
         try
         {

            // APPLY COVER PATH TO THE FOLDER STRUCTURE
            var parentFolder = this.ComicFoldersDictionary[fileData.ComicFile.ParentPath];
            while (parentFolder != null)
            {
               if (!string.IsNullOrEmpty(parentFolder.CoverPath)) { break; }

               parentFolder.CoverPath = fileData.CoverPath;
               parentFolder.ComicFolder.CoverPath = fileData.CoverPath;
               await Task.Run(() => App.Database.Update(parentFolder.ComicFolder));

               if (string.IsNullOrEmpty(parentFolder.ComicFolder.ParentPath)) { break; }
               parentFolder = this.ComicFoldersDictionary[parentFolder.ComicFolder.ParentPath];
            }

         }
         catch (Exception ex) { throw; }
      }
      #endregion


      #region AnalyseFoldersAvailability
      private async Task AnalyseFoldersAvailability()
      { await this.AnalyseFoldersAvailability(App.HomeData); }
      private async Task AnalyseFoldersAvailability(Views.Folder.FolderData folder)
      {
         try
         {
            folder.Available = folder.HasFiles || folder.HasFolders;
            for (int childrenIndex = (folder.Folders.Count - 1); childrenIndex >= 0; childrenIndex--)
            {
               var childrenFolder = folder.Folders[childrenIndex];
               await this.AnalyseFoldersAvailability(childrenFolder);
               if (!childrenFolder.Available) { folder.Folders.RemoveAt(childrenIndex); }
            }
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region DefineFirstFolder
      private void DefineFirstFolder()
      {
         try
         {
            if (App.HomeData.Folders.Count == 1)
            {
               var initialFolders = App.HomeData.Folders;
               while (true)
               {
                  if (initialFolders.Count == 0) { break; }
                  else if (initialFolders.Count > 1) { break; }
                  else { initialFolders = initialFolders.FirstOrDefault().Folders; }
               }
               App.HomeData.Folders.ReplaceRange(initialFolders);
               App.HomeData.FullText = R.Strings.AppTitle;
            }
            else { App.HomeData.Folders.RefreshNow(); }
         }
         catch (Exception ex) { throw; }
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
         // this.ComicFoldersDictionary = null;
      }
      #endregion

   }
}