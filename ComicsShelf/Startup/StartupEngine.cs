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
               await startupEngine.ReviewComicsData();
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
               await startupEngine.ReviewComicsData();
               await startupEngine.AnalyseStatistics();
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


            // LOCATE COMICS LIST
            var fileList = await this.FileSystem.GetFiles(App.Settings.Paths.ComicsPath);
            // fileList = fileList.Take(10).ToArray();

            // LOAD DATABASE DATA
            this.ComicFolders = App.Database.Table<Database.ComicFolders>().ToList();
            this.ComicFiles = App.Database.Table<Database.ComicFiles>().ToList();

            // REMOVE FILES ALREADY LOADED
            /*
            var comicFilesCurrentList = this.ComicFiles.Select(x => x.FullPath).ToList();
            if (comicFilesCurrentList != null && comicFilesCurrentList.Count != 0) {
               fileList = fileList.Where(x => !comicFilesCurrentList.Contains(x)).ToArray();
            }
            */

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

                  /* LOAD COMIC */
                  File.FileData comicFile = null;
                  Folder.FolderData comicFolder = null;
                  await Task.Run(() => comicFile = this.SearchComicFiles_GetFile(filePath));
                  await Task.Run(() => comicFolder = this.SearchComicFiles_GetFolder(comicFile));
                  comicFolder.Files.Add(comicFile);
                  comicFolder.HasFiles = true;

                  // DATA
                  // await Task.Run(() => this.SearchComicFiles_LoadData(comicFile));
                  // await Task.Run(() => this.SearchComicFiles_LoadCover(comicFile));
                  App.RootFolder.Files.Add(comicFile);

               }
               catch (Exception fileException)
               { if (!await App.Message.Confirm($"->Path:{filePath}\n->File:{fileIndex}/{fileQuantity}\n->Exception:{fileException}")) { Environment.Exit(0); } }
            }

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

      private Folder.FolderData SearchComicFiles_GetFolder(File.FileData comicFile)
      {
         try
         {

            // STARTS WITH THE ROOT FOLDER
            Folder.FolderData fileFolder = App.RootFolder;
            var splitedPath = comicFile.FullPath
               .Split(new string[] { App.Settings.Paths.Separator }, StringSplitOptions.RemoveEmptyEntries);

            // LOOP THROUGH SPLITED PATH PARTS [except last one, that is the file itself]
            var parentPath = "";
            for (int splitIndex = 0; splitIndex < (splitedPath.Length - 1); splitIndex++)
            {
               var folderText = splitedPath[splitIndex];
               var folderPath = $"{parentPath}{(string.IsNullOrEmpty(parentPath) ?  "": App.Settings.Paths.Separator)}{folderText}";

               // TRY TO LOCATE A CHILD FOLDER 
               var folder = fileFolder.Folders
                  .Where(x => x.Text == folderText)
                  .FirstOrDefault();

               //IF HADNT FOUND, ADD A NEW ONE
               if (folder == null)
               {
                  folder = new Folder.FolderData { Text = folderText };
                  fileFolder.Folders.Add(folder);
                  fileFolder.HasFolders = true;
               }
               fileFolder = folder;

               // FOLDER DATA
               if (fileFolder.PersistentData == null)
               {
                  try
                  {
                     fileFolder.PersistentData = this.ComicFolders
                        .Where(x => x.FullPath == folderPath)
                        .ToList()
                        .FirstOrDefault();
                  }
                  catch { }
                  if (fileFolder.PersistentData == null)
                  {
                     fileFolder.PersistentData = new Database.ComicFolders
                     {
                        FullPath = folderPath,
                        ParentPath = parentPath,
                        CoverPath = comicFile.CoverPath
                     };
                     App.Database.Insert(fileFolder.PersistentData);
                  }
                  fileFolder.CoverPath = fileFolder.PersistentData.CoverPath;
               }
               if (!string.IsNullOrEmpty(parentPath))
               { parentPath += App.Settings.Paths.Separator; }
               parentPath += folderText;

            }

            // RESULT
            return fileFolder;

         }
         catch (Exception ex) { throw; }
      }

      private File.FileData SearchComicFiles_GetFile(string filePath)
      {
         try
         {

            // INITIALIZE
            var comicFile = new File.FileData { FullPath = filePath };
            var splitedPath = filePath
               .Split(new string[] { App.Settings.Paths.Separator }, StringSplitOptions.RemoveEmptyEntries);

            // COMIC DATA
            try
            {
               comicFile.PersistentData = this.ComicFiles
                  .Where(x => x.FullPath == filePath)
                  .ToList()
                  .FirstOrDefault();
            }
            catch { }
            if (comicFile.PersistentData == null)
            {
               comicFile.PersistentData = new Database.ComicFiles { FullPath = comicFile.FullPath };
               App.Database.Insert(comicFile.PersistentData);
            }

            // PARENT PATH
            var fileName = splitedPath[splitedPath.Length - 1];
            var parentPath = filePath.Replace($"{App.Settings.Paths.Separator}{fileName}", "");
            var parentName = splitedPath[splitedPath.Length - 2];
            if (string.IsNullOrEmpty(comicFile.PersistentData.ParentPath)) {
               comicFile.PersistentData.ParentPath = parentPath;
               App.Database.Update(comicFile.PersistentData);
            }

            // TEXT
            comicFile.Text = fileName
               .Replace(".cbz", "")
               .Replace(".cbr", "")
               .Trim();
            comicFile.SmallText = comicFile.Text
               .Replace(parentName, "");
            if (string.IsNullOrEmpty(comicFile.SmallText))
            { comicFile.SmallText = comicFile.Text; }

            // COVER PATH   
            if (string.IsNullOrEmpty(comicFile.PersistentData.CoverPath))
            {
               var coverPath = comicFile.FullPath;
               coverPath = coverPath
                  .Replace(".cbz", ".jpg")
                  .Replace(".cbr", ".jpg");
               coverPath = coverPath
                  .Replace(App.Settings.Paths.Separator, "_")
                  .Replace(" ", "_")
                  .Replace("#", "_");
               comicFile.PersistentData.CoverPath = $"{App.Settings.Paths.CoversPath}{App.Settings.Paths.Separator}{coverPath}";
               App.Database.Update(comicFile.PersistentData);
            }
            comicFile.CoverPath = comicFile.PersistentData.CoverPath;

            // RESULT
            return comicFile;

         }
         catch (Exception ex) { throw; }
      }

      /*
      private void SearchComicFiles_LoadData(File.FileData file)
      {
         try
         {

            // DATA
            var comicData = App.Database
               .Table<Database.ComicFiles>()
               .Where(x => x.FullPath == file.FullPath)
               .FirstOrDefault();
            if (comicData == null)
            {
               comicData = new Database.ComicFiles { FullPath = file.FullPath };
               App.Database.Insert(comicData);
            }
            file.PersistentData = comicData;

         }
         catch (Exception ex) { throw; }
      }
      */

      private async Task SearchComicFiles_LoadCover(File.FileData file)
      {
         try
         {

            // CHECK IF COVER ALREADY EXISTS
            if (System.IO.File.Exists(file.CoverPath)) { return; }
            if (file.FullPath.ToLower().EndsWith(".cbr")) { return; }

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

      #endregion

      #region ReviewComicsData

      private async Task ReviewComicsData()
      {
         try
         {

            // INITIALIZE
            this.Data.Text = R.Strings.STARTUP_REVIEWING_COMICS_DATA_MESSAGE;
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
               // await Task.Run(() => this.SearchComicFiles_LoadData(file));
               await Task.Run(() => this.SearchComicFiles_LoadCover(file));
            }

            // COVERS FOR CHILDREN FOLDER 
            this.Data.Progress = 0;
            var folderQuantity = App.RootFolder.Folders.Count;
            for (int folderIndex = 0; folderIndex < folderQuantity; folderIndex++)
            {
               var folder = App.RootFolder.Folders[folderIndex];
               this.Data.Progress = ((double)folderIndex / (double)folderQuantity);
               this.Data.Details = folder.Text;
               this.Notify();
               await this.ReviewComicsData_Cover(folder);
            }

         }
         catch (Exception ex) { throw; }
      }

      private async Task ReviewComicsData_Cover(Folder.FolderData folder)
      {
         try
         {

            /* FILES */
            if (folder.Files != null && folder.Files.Count > 0)
            {

               /* FOLDER COVER */
               folder.CoverPath = folder.Files
                  .Where(x => !string.IsNullOrEmpty(x.CoverPath))
                  .Select(x => x.CoverPath)
                  .OrderBy(x => x)
                  .FirstOrDefault();

            }

            /* CHILD FOLDERS */
            if (folder.Folders != null && folder.Folders.Count > 0)
            {

               /* ANALYSE CHILD FOLDERS META DATA */
               foreach (var childFolder in folder.Folders)
               { await this.ReviewComicsData_Cover(childFolder); }

               /* SELF COVER */
               folder.CoverPath = folder.Folders
                  .Where(x => !string.IsNullOrEmpty(x.CoverPath))
                  .Select(x => x.CoverPath)
                  .FirstOrDefault();

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

            // READING FILES
            var readingFiles = App.RootFolder.Files
               .Where(x => x.PersistentData.ReadingPercent > 0 && x.PersistentData.ReadingPercent < 100)
               .OrderByDescending(x => x.PersistentData.ReadingDate)
               .Take(5)
               .ToList();
            App.RootFolder.ReadingFiles.ReplaceRange(readingFiles);

            // TOP RATED FILES
            var topRatedFiles = App.RootFolder.Files
               .Where(x => x.PersistentData.Rate.HasValue)
               .OrderByDescending(x => x.PersistentData.Rate.Value)
               .ThenByDescending(x => x.PersistentData.ReleaseDate)
               .Take(5)
               .ToList();
            App.RootFolder.TopRatedFiles.ReplaceRange(topRatedFiles);

         }
         catch (Exception ex) { throw; }
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

         this.Data = null;
         this.FileSystem = null;
      }
      #endregion

   }
}