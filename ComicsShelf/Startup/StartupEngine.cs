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


      /*
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
               Engine.SearchEngine.Execute();

               await Helpers.ViewModels.NavVM.PushAsync<Home.HomeVM>(true, App.RootFolder);
               await startupEngine.AnalyseStatistics();
               await startupEngine.ExtractComicCover();
               await startupEngine.AnalyseStatistics();
            }
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion
      */

      /*
      #region Search
      public static async void Search()
      {
         try
         {
            Engine.SearchEngine.Execute();
            using (var startupEngine = new StartupEngine())
            {
               await startupEngine.AnalyseStatistics();
               await startupEngine.ExtractComicCover();
            }
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion
      */     

      #region Properties
      private Helpers.iFileSystem FileSystem { get; set; }
      private List<Database.ComicFiles> ComicFiles { get; set; }
      private List<Database.ComicFolders> ComicFolders { get; set; }
      private Dictionary<string, Folder.FolderData> ComicFoldersDictionary { get; set; }
      private StartupData Data { get; set; }
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