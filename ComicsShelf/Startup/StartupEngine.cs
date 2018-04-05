using System;
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
         this.Data = new StartupData();
         this.FileSystem = Helpers.FileSystem.Get();
      }
      #endregion



      #region Start
      public static async void Start()
      {
         try
         {
            using (var startupEngine = new StartupEngine())
            {
               await Helpers.ViewModels.NavVM.PushAsync<StartupVM>(true);
               await startupEngine.LoadSettings();
               await startupEngine.DefineComicsPath();
               await startupEngine.SearchComicFiles();
               await startupEngine.ReviewFoldersData();
               await Helpers.ViewModels.NavVM.PushAsync<Home.HomeVM>(true, App.RootFolder);
            }
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion     

      #region Properties
      private Helpers.Settings.Settings Settings { get; set; }
      private Helpers.iFileSystem FileSystem { get; set; }
      private StartupData Data { get; set; }
      #endregion

      #region LoadSettings
      private async Task LoadSettings()
      {
         try
         {
            this.Data.Text = R.Strings.STARTUP_LOADING_SETTINGS_MESSAGE;
            this.Notify();

            await App.Settings.Initialize();
            this.Settings = App.Settings;
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
            var configsTable = this.Settings.Database.Table<Helpers.Settings.Configs>();
            var configs = configsTable.FirstOrDefault();
            if (configs == null)
            {
               configs = new Helpers.Settings.Configs();
               this.Settings.Database.Insert(configs);
            }

            /* VALIDATE COMICS PATH */
            configs.ComicsPath = await this.FileSystem.GetComicsPath(configs.ComicsPath);

            /* STORE DATA */
            this.Settings.Database.Update(configs);
            this.Settings.Paths.ComicsPath = configs.ComicsPath;

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
            App.RootFolder = new Home.HomeData { Text = "Root" };
            this.Data.Text = R.Strings.STARTUP_SEARCHING_COMIC_FILES_MESSAGE;
            this.Data.Details = string.Empty;
            this.Data.Progress = 0;
            this.Notify();


            // LOCATE COMICS LIST
            var fileList = await this.FileSystem.GetFiles(this.Settings.Paths.ComicsPath);
            // fileList = fileList.Take(10).ToArray();
            var fileQuantity = fileList.Length;

            // LOOP THROUGH FILE LIST
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
                  var comicFolder = this.SearchComicFiles_GetFolder(filePath);
                  var comicFile = this.SearchComicFiles_GetFile(filePath);
                  comicFolder.Files.Add(comicFile);
                  comicFolder.HasFiles = true;

                  // DATA
                  await Task.Run(() => this.SearchComicFiles_LoadData(comicFile));
                  await Task.Run(() => this.SearchComicFiles_LoadCover(comicFile));
                  App.RootFolder.RecentFiles.Add(comicFile);

               }
               catch (Exception fileException)
               { if (!await App.Message.Confirm($"->Path:{filePath}\n->File:{fileIndex}/{fileQuantity}\n->Exception:{fileException}")) { Environment.Exit(0); } }
            }

         }
         catch (Exception ex) { throw; }
      }

      private Folder.FolderData SearchComicFiles_GetFolder(string filePath)
      {
         try
         {

            // STARTS WITH THE ROOT FOLDER
            Folder.FolderData fileFolder = App.RootFolder;

            // SPLIT PATH
            var splitedPath = filePath
               .Split(new string[] { this.Settings.Paths.Separator }, StringSplitOptions.RemoveEmptyEntries);

            // LOOP THROUGH SPLITED PATH PARTS [except last one, that is the file itself]
            for (int splitIndex = 0; splitIndex < (splitedPath.Length - 1); splitIndex++)
            {
               var folderText = splitedPath[splitIndex];

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

            // SPLIT PATH
            var splitedPath = filePath
               .Split(new string[] { this.Settings.Paths.Separator }, StringSplitOptions.RemoveEmptyEntries);

            // INITIALIZE
            var comicFile = new File.FileData { FullPath = filePath };
            var folderName = splitedPath[splitedPath.Length - 2];
            var fileName = splitedPath[splitedPath.Length - 1];

            // TEXT
            comicFile.Text = fileName
               .Replace(".cbz", "")
               .Replace(".cbr", "")
               .Trim();
            comicFile.SmallText = comicFile.Text
               .Replace(folderName, "");
            if (string.IsNullOrEmpty(comicFile.SmallText))
            { comicFile.SmallText = comicFile.Text; }

            // COVER PATH            
            var coverPath = comicFile.FullPath;
            coverPath = coverPath
               .Replace(".cbz", ".jpg")
               .Replace(".cbr", ".jpg");
            coverPath = coverPath
               .Replace(this.Settings.Paths.Separator, "_")
               .Replace(" ", "_")
               .Replace("#", "_");
            comicFile.CoverPath = $"{this.Settings.Paths.CoversPath}{this.Settings.Paths.Separator}{coverPath}";

            // RESULT
            return comicFile;

         }
         catch (Exception ex) { throw; }
      }

      private void SearchComicFiles_LoadData(File.FileData file)
      {
         try
         {

            // DATA
            var comicData = this.Settings.Database
               .Table<Helpers.Settings.Comics>()
               .Where(x => x.FullPath == file.FullPath)
               .FirstOrDefault();
            if (comicData == null)
            {
               comicData = new Helpers.Settings.Comics { FullPath = file.FullPath };
               this.Settings.Database.Insert(comicData);
            }
            file.PersistentData = comicData;

         }
         catch (Exception ex) { throw; }
      }

      private async Task SearchComicFiles_LoadCover(File.FileData file)
      {
         try
         {

            // CHECK IF COVER ALREADY EXISTS
            if (System.IO.File.Exists(file.CoverPath)) { return; }
            if (file.FullPath.ToLower().EndsWith(".cbr")) { return; }

            // OPEN ZIP ARCHIVE
            using (var zipArchive = await this.FileSystem.GetZipArchive(this.Settings, file))
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
                     file.PersistentData.ReleaseDate = this.Settings.GetDatabaseDate(zipEntry.LastWriteTime.DateTime.ToLocalTime());
                     this.Settings.Database.Update(file.PersistentData);
                  }
                  await this.FileSystem.Thumbnail(zipStream, file.CoverPath);
               }
            }
         }
         catch (Exception ex) { throw; }
         finally { GC.Collect(); }
      }

      #endregion

      #region ReviewFoldersData

      private async Task ReviewFoldersData()
      {
         try
         {

            // INITIALIZE
            this.Data.Text = R.Strings.STARTUP_REVIEWING_FOLDERS_DATA_MESSAGE;
            this.Data.Details = string.Empty;
            this.Data.Progress = 0;
            this.Notify();

            // RECENT FILES
            var recentFiles = App.RootFolder.RecentFiles
               .Where(x => !string.IsNullOrEmpty(x.PersistentData.ReleaseDate))
               .OrderByDescending(x => x.PersistentData.ReleaseDate)
               .Take(5)
               .ToList();

            // READING FILES
            var readingFiles = App.RootFolder.RecentFiles
               .Where(x => x.PersistentData.ReadingPercent > 0 && x.PersistentData.ReadingPercent < 100)
               .OrderByDescending(x => x.PersistentData.ReadingDate)
               .Take(5)
               .ToList();

            // READING FILES
            var topRatedFiles = App.RootFolder.RecentFiles
               .Where(x => x.PersistentData.Rate.HasValue)
               .OrderByDescending(x => x.PersistentData.Rate.Value)
               .ThenByDescending(x => x.PersistentData.ReleaseDate)
               .Take(5)
               .ToList();

            // LOCATE FIRST FOLDER WITH CONTENT
            var initialFolders = App.RootFolder.Folders;
            while (true)
            {
               if (initialFolders.Count == 0) { break; }
               else if (initialFolders.Count > 1) { break; }
               else { initialFolders = initialFolders.FirstOrDefault().Folders; }
            }
            App.RootFolder.Folders = initialFolders;
            App.RootFolder.Text = R.Strings.AppTitle;
            App.RootFolder.RecentFiles.ReplaceRange(recentFiles);
            readingFiles.ForEach(x => App.RootFolder.ReadingFiles.Add(x));
            topRatedFiles.ForEach(x => App.RootFolder.TopRatedFiles.Add(x));

            // COVERS FOR CHILDREN FOLDER 
            var folderQuantity = App.RootFolder.Folders.Count;
            for (int folderIndex = 0; folderIndex < folderQuantity; folderIndex++)
            {
               var folder = App.RootFolder.Folders[folderIndex];
               this.Data.Progress = ((double)folderIndex / (double)folderQuantity);
               this.Data.Details = folder.Text;
               this.Notify();
               await this.ReviewFoldersData_Cover(folder);
            }

         }
         catch (Exception ex) { throw; }
      }

      private async Task ReviewFoldersData_Cover(Folder.FolderData folder)
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
               { await this.ReviewFoldersData_Cover(childFolder); }

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

      #region Notify
      private void Notify()
      { MessagingCenter.Send(this.Data, "Startup"); }
      #endregion


      #region Dispose
      public void Dispose()
      {
         this.Data = null;
         this.FileSystem = null;
      }
      #endregion

   }
}