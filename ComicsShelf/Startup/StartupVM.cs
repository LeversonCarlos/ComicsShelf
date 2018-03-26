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

            // LOCATE COMICS LIST
            var fileList = await fileSystem.GetFiles(App.Settings.Paths.ComicsPath);
            var fileQuantity = fileList.Length;
            var eachDelay = 2000 / fileQuantity;

            // LOOP THROUGH FILE LIST
            for (int fileIndex = 0; fileIndex < fileQuantity; fileIndex++)
            {
               var filePath = fileList[fileIndex];
               this.Data.Progress = ((double)fileIndex / (double)fileQuantity);
               this.Data.Details = filePath;

               /* LOAD COMIC */
               var comicFolder = this.OnInitialize_Search_GetFolder(filePath);
               var comicFile = this.OnInitialize_Search_GetFile(filePath);
               comicFolder.Files.Add(comicFile);

               // LOAD COVER
               /*
               await pathService.LoadCover(App.Settings, comicFile);
               // comicFile.CoverPath = $"file://{comicFile.CoverPath}";
               if (string.IsNullOrEmpty(comicFolder.CoverPath) && string.IsNullOrEmpty(comicFile.Exception))
               { comicFolder.CoverPath = comicFile.CoverPath; }
               */

               await Task.Delay(eachDelay);

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
            var coverPath = comicFile.FullPath
               .Replace(App.Settings.Paths.ComicsPath, "");
            coverPath = coverPath
               .Replace(".cbz", ".jpg")
               .Replace(".cbr", ".jpg");
            coverPath = coverPath
               .Replace(App.Settings.Paths.Separator, "_")
               .Replace(" ", "_")
               .Replace("#", "_");
            comicFile.CoverPath = $"{App.Settings.Paths.CoversPath}{coverPath}";

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

      #endregion

      #region OnInitialize_HomeScreen
      private async Task OnInitialize_HomeScreen()
      {
         try
         {

            // LOCATE FOLDER WITH CONTENT
            var initialFolder = this.Data.RootFolder;
            while (true)
            {
               if (initialFolder.Folders.Count == 0) { break; }
               else if (initialFolder.Folders.Count > 1) { break; }
               else { initialFolder = initialFolder.Folders.FirstOrDefault(); }
            }

            // CLOSE STARTUP PAGE
            var mainPage = Application.Current.MainPage as Page;
            var navigation = mainPage.Navigation;
            await navigation.PopModalAsync();

            // OPEN INITIAL FOLDER
            await Helpers.ViewModels.NavVM.PushAsync<Folder.FolderVM>(true, initialFolder);

         }
         catch (Exception ex) { throw; }
      }
      #endregion

   }
}