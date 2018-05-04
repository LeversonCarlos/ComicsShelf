using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Startup
{
   internal class StartupEngine : IDisposable
   {

      #region New
      public StartupEngine()
      {
         this.FileSystem = Helpers.FileSystem.Get();
         this.Data = new StartupData();
         this.Data.Step = StartupData.enumStartupStep.Running;
         this.Data.Text = string.Empty;
         this.Data.Details = string.Empty;
         this.Notify();
      }
      #endregion



      #region Start
      public static async void Start()
      {
         try
         {
            using (var startupEngine = new StartupEngine())
            {
               App.RootFolder = new Home.HomeData { Text = "Root" };

               await Helpers.ViewModels.NavVM.PushAsync<StartupVM>(true);
               await startupEngine.LoadSettings();
               await startupEngine.DefineComicsPath();
               await startupEngine.SearchComicFiles();

               await Helpers.ViewModels.NavVM.PushAsync<Home.HomeVM>(true, App.RootFolder);
               await startupEngine.AnalyseStatistics();
               await startupEngine.ExtractComicCover();
               await startupEngine.AnalyseStatistics();
            }
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region Search
      public static async void Search()
      {
         try
         {
            using (var startupEngine = new StartupEngine())
            {
               await startupEngine.SearchComicFiles();
               await startupEngine.AnalyseStatistics();
               await startupEngine.ExtractComicCover();
            }
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region Refresh
      public static async void Refresh()
      {
         try
         {
            using (var startupEngine = new StartupEngine())
            {
               await startupEngine.AnalyseStatistics();
            }
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region Properties
      private Helpers.iFileSystem FileSystem { get; set; }
      private List<Database.ComicFiles> ComicFiles { get; set; }
      private List<Database.ComicFolders> ComicFolders { get; set; }
      private Dictionary<string, Folder.FolderData> ComicFoldersDictionary { get; set; }
      private StartupData Data { get; set; }
      #endregion


      #region LoadSettings
      private async Task LoadSettings()
      {
         try
         {
            this.Data.Text = R.Strings.STARTUP_LOADING_SETTINGS_MESSAGE;
            this.Notify();

            App.Settings = new Helpers.Settings.Settings();
            await App.Settings.InitializePath();

            App.Database = new Database.Connector();
            App.Database.InitializeConnector(App.Settings.Paths.DatabasePath);
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region DefineComicsPath
      private async Task DefineComicsPath()
      {
         try
         {
            this.Data.Text = R.Strings.STARTUP_DEFINING_COMICS_PATH_MESSAGE;
            this.Notify();

            /* LOAD CONFIGS DATA */
            var configsTable = App.Database.Table<Database.Configs>();
            var configs = configsTable.FirstOrDefault();
            if (configs == null)
            {
               configs = new Database.Configs();
               App.Database.Insert(configs);
            }

            /* VALIDATE COMICS PATH */
            configs.ComicsPath = await this.FileSystem.GetComicsPath(configs.ComicsPath);

            /* STORE DATA */
            App.Database.Update(configs);
            App.Settings.Paths.ComicsPath = configs.ComicsPath;

         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region SearchComicFiles

      private async Task SearchComicFiles()
      {
         try
         {

            // INITIALIZE
            this.Data.Text = R.Strings.STARTUP_SEARCHING_COMIC_FILES_MESSAGE;
            this.Data.Details = string.Empty;
            this.Data.Progress = 0;
            this.Notify();

            // LOAD DATABASE DATA
            this.ComicFiles = App.Database.Table<Database.ComicFiles>().ToList();
            this.ComicFolders = App.Database.Table<Database.ComicFolders>().ToList();
            this.ComicFoldersDictionary = new Dictionary<string, Folder.FolderData>();

            // SEARCH NEW COMIC FILES
            await this.SearchComicFiles_SearchNewComicFiles();
            this.ComicFiles = this.ComicFiles
               .OrderBy(x => x.FullPath)
               .ToList();
            this.ComicFolders = this.ComicFolders
               .OrderBy(x => x.FullPath)
               .ToList();
            App.RootFolder.NoComics = (this.ComicFiles == null || this.ComicFiles.Count == 0);

            // PREPARE FOLDER STRUCTURE
            await this.SearchComicFiles_PrepareFoldersStructure();

            // PREPARE FILE STRUCTURE
            await this.SearchComicFiles_PrepareFilesStructure();

            // LOCATE FIRST FOLDER WITH CONTENT
            if (App.RootFolder.Folders.Count == 1) {
               var initialFolders = App.RootFolder.Folders;
               while (true)
               {
                  if (initialFolders.Count == 0) { break; }
                  else if (initialFolders.Count > 1) { break; }
                  else { initialFolders = initialFolders.FirstOrDefault().Folders; }
               }
               App.RootFolder.Folders.ReplaceRange(initialFolders);
            }
            App.RootFolder.Text = R.Strings.AppTitle;

         }
         catch (Exception ex) { throw; }
      }

      private async Task SearchComicFiles_SearchNewComicFiles()
      {
         try
         {

            // INITIALIZE
            this.Data.Text = R.Strings.STARTUP_SEARCHING_COMIC_FILES_MESSAGE;
            this.Data.Details = string.Empty;
            this.Data.Progress = 0;
            this.Notify();

            // LOCATE COMICS LIST
            var fileList = await this.FileSystem.GetFiles(App.Settings.Paths.ComicsPath);
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
            for (int fileIndex = 0; fileIndex < fileQuantity; fileIndex++)
            {
               string filePath = "";
               try
               {
                  filePath = fileList[fileIndex];
                  this.Data.Progress = ((double)fileIndex / (double)fileQuantity);
                  this.Data.Details = filePath;
                  this.Notify();

                  var comicFile = await Task.Run(() => this.SearchComicFiles_SearchNewComicFiles_GetFile(filePath));
                  await Task.Run(() => this.SearchComicFiles_SearchNewComicFiles_GetFolder(comicFile));

               }
               catch (Exception fileException)
               { if (!await App.Message.Confirm($"->Path:{filePath}\n->File:{fileIndex}/{fileQuantity}\n->Exception:{fileException}")) { Environment.Exit(0); } }
            }

         }
         catch (Exception ex) { throw; }
      }

      private Database.ComicFiles SearchComicFiles_SearchNewComicFiles_GetFile(string filePath)
      {
         try
         {

            // INITIALIZE
            var comicFile = new Database.ComicFiles { FullPath = filePath, Available = true };

            // PARENT PATH
            var fileName = System.IO.Path.GetFileName(filePath);
            var folderPath = System.IO.Path.GetDirectoryName(filePath);
            var folderText = System.IO.Path.GetFileNameWithoutExtension(folderPath);
            comicFile.ParentPath = folderPath;

            // TEXT
            comicFile.Text = System.IO.Path.GetFileNameWithoutExtension(filePath)
               .Trim();
            comicFile.SmallText = comicFile.Text
               .Replace(folderText, "");
            if (string.IsNullOrEmpty(comicFile.SmallText))
            { comicFile.SmallText = comicFile.Text; }

            // COVER PATH   
            var coverPath = $"{filePath}.jpg";
            coverPath = coverPath
               .Replace(App.Settings.Paths.Separator, "_")
               .Replace("#", "")
               .Replace(" ", "_")
               .Replace("___", "_")
               .Replace("__", "_");
            comicFile.CoverPath = $"{App.Settings.Paths.CoversPath}{App.Settings.Paths.Separator}{coverPath}";

            // INSERT COMIC FILE
            this.ComicFiles.Add(comicFile);
            App.Database.Insert(comicFile);

            return comicFile;
         }
         catch (Exception ex) { throw; }
      }

      private void SearchComicFiles_SearchNewComicFiles_GetFolder(Database.ComicFiles comicFile)
      {
         try
         {

            // INITIALIZE
            var folderPath = comicFile.ParentPath;

            // ADD FOLDER STRUCTURE
            while (!string.IsNullOrEmpty(folderPath) && folderPath != App.Settings.Paths.ComicsPath)
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
               comicFolder = new Database.ComicFolders
               {
                  FullPath = folderPath,
                  Text = folderText,
                  ParentPath = folderParent
               };
               this.ComicFolders.Add(comicFolder);
               App.Database.Insert(comicFolder);

               folderPath = folderParent;
            }

         }
         catch (Exception ex) { throw; }
      }

      private async Task SearchComicFiles_PrepareFoldersStructure()
      {
         try
         {

            // INITIALIZE
            this.Data.Text = R.Strings.STARTUP_PREPARING_FOLDERS_STRUCTURE_MESSAGE;
            this.Data.Details = string.Empty;
            this.Data.Progress = 0;
            this.Notify();

            // LOOP THROUGH FOLDER LIST
            var loopQuantity = this.ComicFolders.Count;
            for (int loopIndex = 0; loopIndex < loopQuantity; loopIndex++)
            {
               var DATA = this.ComicFolders[loopIndex];
               try
               {
                  this.Data.Details = DATA.FullPath;
                  this.Data.Progress = ((double)loopIndex / (double)loopQuantity);
                  this.Notify();

                  // PARENT FOLDER
                  Folder.FolderData parentFolder = App.RootFolder;
                  if (!string.IsNullOrEmpty(DATA.ParentPath))
                  {
                     parentFolder = this.ComicFoldersDictionary[DATA.ParentPath];
                  }
                  if (parentFolder == null) { throw new Exception($"Could not find Parent:[{DATA.ParentPath}] for Folder:[{DATA.FullPath}]."); }

                  // ADD FOLDER
                  var folder = new Folder.FolderData
                  {
                     Text = DATA.Text,
                     CoverPath = DATA.CoverPath,
                     PersistentData = DATA
                  };
                  parentFolder.Folders.Add(folder);
                  parentFolder.HasFolders = true;
                  this.ComicFoldersDictionary.Add(DATA.FullPath, folder);
               }
               catch (Exception loopException)
               { if (!await App.Message.Confirm($"->Path:{DATA.FullPath}\n->File:{loopIndex}/{loopQuantity}\n->Exception:{loopException}")) { Environment.Exit(0); } }
            }

            // LOCATE FIRST FOLDER WITH CONTENT
            /*
            if (App.RootFolder.Folders.Count == 1)
            {
               var initialFolders = App.RootFolder.Folders;
               while (true)
               {
                  if (initialFolders.Count == 0) { break; }
                  else if (initialFolders.Count > 1) { break; }
                  else { initialFolders = initialFolders.FirstOrDefault().Folders; }
               }
               App.RootFolder.Folders.ReplaceRange(initialFolders);
            }
            */
            App.RootFolder.Text = R.Strings.AppTitle;

         }
         catch (Exception ex) { throw; }
      }

      private async Task SearchComicFiles_PrepareFilesStructure()
      {
         try
         {

            // INITIALIZE
            this.Data.Text = R.Strings.STARTUP_PREPARING_FILES_STRUCTURE_MESSAGE;
            this.Data.Details = string.Empty;
            this.Data.Progress = 0;
            this.Notify();

            // LOOP THROUGH FILE LIST
            var loopQuantity = this.ComicFiles.Count;
            for (int loopIndex = 0; loopIndex < loopQuantity; loopIndex++)
            {
               var DATA = this.ComicFiles[loopIndex];
               try
               {
                  this.Data.Details = DATA.FullPath;
                  this.Data.Progress = ((double)loopIndex / (double)loopQuantity);
                  this.Notify();
                  if (!DATA.Available) { continue; }

                  // PARENT FOLDER
                  Folder.FolderData parentFolder = App.RootFolder;
                  if (!string.IsNullOrEmpty(DATA.ParentPath))
                  {
                     parentFolder = this.ComicFoldersDictionary[DATA.ParentPath];
                  }
                  if (parentFolder == null) { throw new Exception($"Could not find Parent:[{DATA.ParentPath}] for Folder:[{DATA.FullPath}]."); }

                  // COMIC FILE
                  var comicFile = new File.FileData
                  {
                     FullPath = DATA.FullPath, 
                     Text = DATA.Text,
                     SmallText = DATA.SmallText,
                     CoverPath = DATA.CoverPath, 
                     PersistentData = DATA
                  };
                  parentFolder.Files.Add(comicFile);
                  parentFolder.HasFiles = true;
                  App.RootFolder.Files.Add(comicFile);

               }
               catch (Exception loopException)
               { if (!await App.Message.Confirm($"->Path:{DATA.FullPath}\n->File:{loopIndex}/{loopQuantity}\n->Exception:{loopException}")) { Environment.Exit(0); } }
            }

         }
         catch (Exception ex) { throw; }
      }

      #endregion

      #region ExtractComicCover

      private async Task ExtractComicCover()
      {
         try
         {

            // INITIALIZE
            this.Data.Text = R.Strings.STARTUP_EXTRACTING_COMICS_COVER_MESSAGE;
            this.Data.Details = string.Empty;
            this.Notify();

            // LOOP THROUGH FILES
            this.Data.Progress = 0;
            var filesQuantity = App.RootFolder.Files.Count;
            for (int fileIndex = 0; fileIndex < filesQuantity; fileIndex++)
            {
               var file = App.RootFolder.Files[fileIndex];
               this.Data.Progress = ((double)fileIndex / (double)filesQuantity);
               this.Data.Details = file.Text;
               this.Notify();

               var mustExtract = await Task.Run(() => this.ExtractComicCover_MustExtract(file));
               if (mustExtract)
               {
                  await Task.Run(() => this.ExtractComicCover_File(file));
                  await Task.Run(() => this.ExtractComicCover_Folder(file));
               }
            }

         }
         catch (Exception ex) { throw; }
      }

      private bool ExtractComicCover_MustExtract(File.FileData file)
      {
         if (System.IO.File.Exists(file.CoverPath)) { return false; }
         if (file.FullPath.ToLower().EndsWith(".cbr")) { return false; }
         return true;
      }

      private async Task ExtractComicCover_File(File.FileData file)
      {
         try
         {

            // OPEN ZIP ARCHIVE
            using (var zipArchive = await this.FileSystem.GetZipArchive(App.Settings, file))
            {

               // LOOP THROUGH ENTIES LOOKING FOR THE FIRST IMAGE
               var zipEntries = zipArchive.Entries
                  .Where(x => x.Name.ToLower().EndsWith(".jpg"))
                  .OrderBy(x => x.Name)
                  .ToList();
               var zipEntry = zipEntries.FirstOrDefault();
               using (System.IO.Stream zipStream = zipEntry.Open())
               {
                  if (file.PersistentData != null && string.IsNullOrEmpty(file.PersistentData.ReleaseDate))
                  {
                     file.PersistentData.ReleaseDate = App.Database.GetDate(zipEntry.LastWriteTime.DateTime.ToLocalTime());
                     App.Database.Update(file.PersistentData);
                  }
                  await this.FileSystem.Thumbnail(zipStream, file.CoverPath);
               }
            }

         }
         catch (Exception ex) { throw; }
         finally { GC.Collect(); }
      }

      private async Task ExtractComicCover_Folder(File.FileData file)
      {
         try
         {

            // APPLY COVER PATH TO THE FOLDER STRUCTURE
            var parentFolder = this.ComicFoldersDictionary[file.PersistentData.ParentPath];
            while (parentFolder != null)
            {
               if (!string.IsNullOrEmpty(parentFolder.PersistentData.CoverPath)) { break; }

               parentFolder.CoverPath = file.CoverPath;
               parentFolder.PersistentData.CoverPath = file.CoverPath;
               App.Database.Update(parentFolder.PersistentData);

               if (string.IsNullOrEmpty(parentFolder.PersistentData.ParentPath)) { break; }
               parentFolder = this.ComicFoldersDictionary[parentFolder.PersistentData.ParentPath];
            }

         }
         catch (Exception ex) { throw; }
      }

      #endregion

      #region AnalyseStatistics

      private async Task AnalyseStatistics()
      {
         try
         {

            // RECENT FILES
            var recentFiles = App.RootFolder.Files
               .Where(x => !string.IsNullOrEmpty(x.PersistentData.ReleaseDate))
               .OrderByDescending(x => x.PersistentData.ReleaseDate)
               .Take(5)
               .ToList();
            App.RootFolder.RecentFiles.ReplaceRange(recentFiles);

            // TOP RATED FILES
            var topRatedFiles = this.AnalyseStatistics_TopRatedFiles();
            App.RootFolder.TopRatedFiles.ReplaceRange(topRatedFiles);

            // READING FILES
            var readingFiles = this.AnalyseStatistics_ReadingFiles();
            App.RootFolder.ReadingFiles.ReplaceRange(readingFiles);

         }
         catch (Exception ex) { throw; }
      }

      private List<File.FileData> AnalyseStatistics_ReadingFiles()
      {         

         // GET LAST 5 OPENED FILES
         var openedFiles = App.RootFolder.Files
            .Where(x => x.PersistentData.ReadingPercent > 0.0 && x.PersistentData.ReadingPercent < 1.0)
            .OrderByDescending(x => x.PersistentData.ReadingDate)
            .Take(5)
            .ToList();

         // GET ALL READED FILES
         var readedFiles = App.RootFolder.Files
            .Where(x => x.PersistentData.ReadingPercent == 1.0)
            .ToList();

         // REMOVE GROUPS THAT ALREADY HAS SOME OPENED FILES
         readedFiles
            .RemoveAll(x => openedFiles
               .Select(g => g.PersistentData.ParentPath)
               .Contains(x.PersistentData.ParentPath));

         // GROUP FILES AND MANTAIN ONLY THE MOST RECENT OPENED FILE FOR EACH GROUP
         readedFiles = readedFiles
            .GroupBy(x => x.PersistentData.ParentPath)
            .Select(x => readedFiles
               .Where(g => g.PersistentData.ParentPath == x.Key)
               .OrderByDescending(g => g.PersistentData.ReadingDate)
               .FirstOrDefault())
            .ToList();

         // FROM THAT, TAKE THE NEXT FILE FOR EACH GROUP
         var readedNextFiles = readedFiles
            .Select(x => App.RootFolder.Files
               .Where(f => f.PersistentData.ParentPath == x.PersistentData.ParentPath)
               .Where(f => String.Compare(f.PersistentData.FullPath, x.PersistentData.FullPath) > 0)
               .OrderBy(f => f.PersistentData.FullPath)
               .Take(1)
               .Select(f => new { f, x.PersistentData.ReadingDate })
               .FirstOrDefault())
            .Where(x => x != null)
            .ToList();

         // UNION OPEN AND READED FILES
         var unionFiles = readedNextFiles;
         unionFiles.AddRange(openedFiles.Select(f => new { f, f.PersistentData.ReadingDate }).AsEnumerable());

         // TAKE THE LAST 5
         var readingFiles = unionFiles
            .OrderByDescending(x => x.ReadingDate)
            .Select(x => x.f)
            .Take(5)
            .ToList();
         return readingFiles;
      }

      private List<File.FileData> AnalyseStatistics_TopRatedFiles()
      {

         // GET ALL RATED FILES
         var allRatedFiles = App.RootFolder.Files
            .Where(x => x.PersistentData.Rate.HasValue)
            .ToList();

         // GROUP FILES WITH ITS AVERAGE RATE
         var groupFiles = allRatedFiles
            .GroupBy(x => x.PersistentData.ParentPath)
            .Select(x => new {
               ParentPath = x.Key,
               Rate = x.Average(g=> (double)g.PersistentData.Rate.Value)
            })
            .ToList();

         // TOP RATED FILES
         var topRatedFiles = allRatedFiles
            .Select(x=> new {
               File = x, 
               GroupRate = groupFiles
                  .Where(g=> g.ParentPath == x.PersistentData.ParentPath)
                  .Select(g=> g.Rate)
                  .FirstOrDefault()
            })
            .OrderByDescending(x => x.GroupRate)
            .ThenByDescending(x => x.File.PersistentData.Rate.Value)
            .ThenByDescending(x => x.File.PersistentData.ReadingDate)
            .Select(x => x.File)
            .Take(5)
            .ToList();
         return topRatedFiles;

      }

      #endregion

      #region Notify
      private void Notify()
      { MessagingCenter.Send(this.Data, "Startup"); }
      #endregion


      #region Dispose
      public void Dispose()
      {
         this.Data.Step = StartupData.enumStartupStep.Finished;
         this.Data.Text = string.Empty;
         this.Data.Details = string.Empty;
         this.Notify();

         this.ComicFoldersDictionary = null;
         this.Data = null;
         this.FileSystem = null;
      }
      #endregion

   }
}