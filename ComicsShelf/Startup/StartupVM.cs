using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Startup
{
   public class StartupVM : Helpers.ViewModels.DataVM<StartupData>
   {

      #region New
      public StartupVM()
      {
         this.Title = R.Strings.AppTitle;
         this.ViewType = typeof(StartupPage);

         this.Data = new StartupData();
         this.Initialize += this.OnInitialize;
      }
      #endregion

      #region OnInitialize
      private async void OnInitialize()
      {
         try
         {
            this.Data.Progress = 0;

            this.Data.Text = R.Strings.STARTUP_LOADING_SETTINGS_MESSAGE;
            await this.OnInitialize_Settings();

            this.Data.Text = R.Strings.STARTUP_DEFINING_COMICS_PATH_MESSAGE;
            await this.OnInitialize_ComicsPath();

            this.Data.Text = R.Strings.STARTUP_SEARCHING_COMIC_FILES_MESSAGE;
            await this.OnInitialize_Search();

            this.Data.Text = R.Strings.STARTUP_LOADING_COMICS_META_DATA_MESSAGE;
            await this.OnInitialize_MetaData();

            this.Data.Text = R.Strings.STARTUP_LOADING_HOME_SCREEN_MESSAGE;
            await this.OnInitialize_HomeScreen();

         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region OnInitialize_Settings
      private async Task OnInitialize_Settings()
      {
         try
         {
            await Task.Delay(1);
            await App.Settings.Initialize();
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region OnInitialize_ComicsPath
      private async Task OnInitialize_ComicsPath()
      {
         try
         {
            await Task.Delay(1);

            /* LOAD CONFIGS DATA */
            var configsTable = App.Settings.Database.Table<Helpers.Settings.Configs>();
            var configs = configsTable.FirstOrDefault();
            if (configs == null)
            {
               configs = new Helpers.Settings.Configs();
               App.Settings.Database.Insert(configs);
            }

            /* VALIDATE COMICS PATH */
            var fileSystem = Helpers.FileSystem.Get();
            configs.ComicsPath = await fileSystem.GetComicsPath(configs.ComicsPath);

            /* STORE DATA */
            App.Settings.Database.Update(configs);
            App.Settings.Paths.ComicsPath = configs.ComicsPath;

         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region OnInitialize_Search

      private async Task OnInitialize_Search()
      {
         try
         {

            // INITIALIZE
            this.Data.Progress = 0;
            this.Data.RootFolder = new Folder.FolderData { Text = "Root" };
            var fileSystem = Helpers.FileSystem.Get();
            var settings = App.Settings;
            await Task.Delay(1);

            // LOCATE COMICS LIST
            var fileList = await fileSystem.GetFiles(App.Settings.Paths.ComicsPath);
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
                  if (Math.Round((Math.Round(this.Data.Progress, 2) % 0.05), 2) == 0)
                  { await Task.Delay(1); }

                  /* LOAD COMIC */
                  var comicFolder = this.OnInitialize_Search_GetFolder(filePath);
                  var comicFile = this.OnInitialize_Search_GetFile(filePath);
                  comicFolder.Files.Add(comicFile);

                  // LOAD COVER
                  await OnInitialize_Search_LoadCover(settings, fileSystem, comicFile);
                  /*
                  await pathService.LoadCover(App.Settings, comicFile);
                  // comicFile.CoverPath = $"file://{comicFile.CoverPath}";
                  if (string.IsNullOrEmpty(comicFolder.CoverPath) && string.IsNullOrEmpty(comicFile.Exception))
                  { comicFolder.CoverPath = comicFile.CoverPath; }
                  */
               }
               catch (Exception fileException)
               { if (!await App.Message.Confirm($"->Path:{filePath}\n->File:{fileIndex}/{fileQuantity}\n->Exception:{fileException}")) { Environment.Exit(0); } }
            }
            this.Data.Progress = 1;
            this.Data.Details = "";

         }
         catch (Exception ex) { throw; }
      }

      private Folder.FolderData OnInitialize_Search_GetFolder(string filePath)
      {
         try
         {

            // STARTS WITH THE ROOT FOLDER
            Folder.FolderData fileFolder = this.Data.RootFolder;

            // SPLIT PATH
            var splitedPath = filePath
               .Split(new string[] { App.Settings.Paths.Separator }, StringSplitOptions.RemoveEmptyEntries);

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
               }

               fileFolder = folder;
            }

            // RESULT
            return fileFolder;

         }
         catch (Exception ex) { throw; }
      }

      private File.FileData OnInitialize_Search_GetFile(string filePath)
      {
         try
         {

            // SPLIT PATH
            var splitedPath = filePath
               .Split(new string[] { App.Settings.Paths.Separator }, StringSplitOptions.RemoveEmptyEntries);

            // INITIALIZE
            var comicFile = new File.FileData { FullPath = filePath };
            var folderName = splitedPath[splitedPath.Length - 2];
            var fileName = splitedPath[splitedPath.Length - 1];

            // TEXT
            comicFile.Text = fileName
               .Replace(folderName, "")
               .Replace(".cbz", "")
               .Replace(".cbr", "")
               .Trim();

            // COVER PATH            
            var coverPath = comicFile.FullPath;
            /* .Replace(App.Settings.Paths.ComicsPath, "") */
            coverPath = coverPath
               .Replace(".cbz", ".jpg")
               .Replace(".cbr", ".jpg");
            coverPath = coverPath
               .Replace(App.Settings.Paths.Separator, "_")
               .Replace(" ", "_")
               .Replace("#", "_");
            comicFile.CoverPath = $"{App.Settings.Paths.CoversPath}{App.Settings.Paths.Separator}{coverPath}";

            // DATA
            /*
            var comicData = App.Settings.Database
               .Table<Helpers.Settings.Comics>()
               .Where(x => x.Key == comicFile.Path)
               .FirstOrDefault();
            if (comicData == null)
            {
               comicData = new Helpers.Settings.Comics { Key = comicFile.Path };
               App.Settings.Database.Insert(comicData);
            }
            comicFile.Data = comicData;
            */

            // RESULT
            return comicFile;

         }
         catch (Exception ex) { throw; }
      }

      private async Task OnInitialize_Search_LoadCover(Helpers.Settings.Settings settings, Helpers.iFileSystem fileSystem, File.FileData file)
      {
         try
         {

            // CHECK IF COVER ALREADY EXISTS
            if (System.IO.File.Exists(file.CoverPath)) { return; }
            if (file.FullPath.ToLower().EndsWith(".cbr")) { return; }

            // OPEN ZIP ARCHIVE
            using (var zipArchive = await fileSystem.GetZipArchive(settings, file))
            {

               // LOOP THROUG ENTIES LOOKING FOR THE FIRST IMAGE
               var zipEntries = zipArchive.Entries
                  .Where(x => x.Name.ToLower().EndsWith(".jpg"))
                  .OrderBy(x => x.Name)
                  .ToList();
               var zipEntry = zipEntries.FirstOrDefault();
               using (System.IO.Stream zipStream = zipEntry.Open())
               {
                  // comicFile.Data.Date = zipEntry.LastWriteTime.DateTime;
                  await fileSystem.Thumbnail(zipStream, file.CoverPath);
               }
            }
         }
         catch (Exception ex) { throw; }
         finally { GC.Collect(); }
      }

      #endregion

      #region OnInitialize_MetaData

      private async Task OnInitialize_MetaData()
      {
         try
         {

            // INITIALIZE
            this.Data.Progress = 0;
            var fileSystem = Helpers.FileSystem.Get();
            var settings = App.Settings;
            await Task.Delay(1);

            // LOCATE FOLDER WITH CONTENT
            this.Data.InitialFolder = this.Data.RootFolder;
            while (true)
            {
               if (this.Data.InitialFolder.Folders.Count == 0) { break; }
               else if (this.Data.InitialFolder.Folders.Count > 1) { break; }
               else { this.Data.InitialFolder = this.Data.InitialFolder.Folders.FirstOrDefault(); }
            }

            // LOOP THROUGH FOLDERS
            var folderQuantity = this.Data.InitialFolder.Folders.Count;
            for (int folderIndex = 0; folderIndex < folderQuantity; folderIndex++)
            {
               var folder = this.Data.InitialFolder.Folders[folderIndex];
               this.Data.Progress = ((double)folderIndex / (double)folderQuantity);
               // this.Data.Details = filePath;
               await this.OnInitialize_MetaData(settings, fileSystem, folder);
            }
            this.Data.Progress = 1;
            this.Data.Details = "";

         }
         catch (Exception ex) { throw; }
      }

      private async Task OnInitialize_MetaData(Helpers.Settings.Settings settings, Helpers.iFileSystem fileSystem, Folder.FolderData folder)
      {
         try
         {

            /* FILES */
            if (folder.Files != null && folder.Files.Count > 0)
            {

               /* ANALYSE FILES META DATA */
               foreach (var file in folder.Files)
               { await this.OnInitialize_MetaData(settings, fileSystem, file); }

               /* SELF COVER */
               folder.CoverPath = folder.Files
                  .Where(x => !string.IsNullOrEmpty(x.CoverPath))
                  .Select(x => x.CoverPath)
                  .FirstOrDefault();

            }

            /* CHILD FOLDERS */
            if (folder.Folders != null && folder.Folders.Count > 0)
            {

               /* ANALYSE CHILD FOLDERS META DATA */
               foreach (var childFolder in folder.Folders)
               { await this.OnInitialize_MetaData(settings, fileSystem, childFolder); }

               /* SELF COVER */
               folder.CoverPath = folder.Folders
                  .Where(x => !string.IsNullOrEmpty(x.CoverPath))
                  .Select(x => x.CoverPath)
                  .FirstOrDefault();

            }

         }
         catch (Exception ex) { throw; }
      }

      private async Task OnInitialize_MetaData(Helpers.Settings.Settings settings, Helpers.iFileSystem fileSystem, File.FileData file)
      {
         try
         {
            this.Data.Details = file.Text;
            // TODO
         }
         catch (Exception ex) { throw; }
      }

      #endregion 

      #region OnInitialize_HomeScreen
      private async Task OnInitialize_HomeScreen()
      {
         try
         {

            // CLOSE STARTUP PAGE
            var mainPage = Application.Current.MainPage as Page;
            var navigation = mainPage.Navigation;
            await navigation.PopModalAsync();

            // OPEN INITIAL FOLDER
            await Helpers.ViewModels.NavVM.PushAsync<Folder.FolderVM>(true, this.Data.InitialFolder);

         }
         catch (Exception ex) { throw; }
      }
      #endregion

   }
}