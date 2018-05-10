using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class SearchEngine : BaseEngine, IDisposable
   {

      #region Execute
      public static async void Execute()
      {
         try
         {
            Console.WriteLine("SearchEngine: Start");
            using (var engine = new SearchEngine())
            {
               await engine.LoadDatabaseData();
               await engine.SearchComicFiles();
               await engine.PrepareFoldersStructure();
               await engine.PrepareFilesStructure();
               await engine.AnalyseFoldersAvailability();
               await engine.DefineFirstFolder();
               Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Statistics.Execute());
            }
            Console.WriteLine("SearchEngine: Finish");
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region Properties
      private List<Database.ComicFiles> ComicFiles { get; set; }
      private List<Database.ComicFolders> ComicFolders { get; set; }
      private Dictionary<string, Folder.FolderData> ComicFoldersDictionary { get; set; }
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
                  .Table<Database.ComicFiles>()
                  .Where(x => x.LibraryPath == App.Settings.Paths.LibraryPath)
                  .ToList();

               this.ComicFolders = App.Database
                  .Table<Database.ComicFolders>()
                  .Where(x => x.LibraryPath == App.Settings.Paths.LibraryPath)
                  .ToList();

            });
            this.ComicFoldersDictionary = new Dictionary<string, Folder.FolderData>();

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
            for (int fileIndex = 0; fileIndex < fileQuantity; fileIndex++)
            {
               string filePath = "";
               try
               {
                  filePath = fileList[fileIndex];
                  var progress = ((double)fileIndex / (double)fileQuantity);
                  this.Notify(filePath, progress);

                  await Task.Run(()=> {
                     var comicFile = this.SearchComicFiles_GetFile(filePath);
                     this.SearchComicFiles_GetFolder(comicFile);
                  });

               }
               catch (Exception fileException)
               { if (!await App.Message.Confirm($"->Path:{filePath}\n->File:{fileIndex}/{fileQuantity}\n->Exception:{fileException}")) { Environment.Exit(0); } }
            }

            // REORDER THE LISTS
            this.ComicFiles = this.ComicFiles
               .OrderBy(x => x.FullPath)
               .ToList();
            this.ComicFolders = this.ComicFolders
               .OrderBy(x => x.FullPath)
               .ToList();
            App.RootFolder.NoComics = (this.ComicFiles == null || this.ComicFiles.Count == 0);

         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region SearchComicFiles_GetFile
      private Database.ComicFiles SearchComicFiles_GetFile(string filePath)
      {
         try
         {

            // INITIALIZE
            var comicFile = new Database.ComicFiles
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
            comicFile.Text = System.IO.Path.GetFileNameWithoutExtension(filePath).Trim();
            if (!string.IsNullOrEmpty(folderText))
            { comicFile.SmallText = comicFile.Text.Replace(folderText, ""); }
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
            comicFile.CoverPath = $"{App.Settings.Paths.CoversCachePath}{App.Settings.Paths.Separator}{coverPath}";

            // INSERT COMIC FILE
            this.ComicFiles.Add(comicFile);
            App.Database.Insert(comicFile);

            return comicFile;
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region SearchComicFiles_GetFolder
      private void SearchComicFiles_GetFolder(Database.ComicFiles comicFile)
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
               comicFolder = new Database.ComicFolders
               {
                  LibraryPath = App.Settings.Paths.LibraryPath,
                  FullPath = folderPath,
                  ParentPath = folderParent,
                  Text = folderText
               };
               comicFolder.Key = $"{comicFolder.LibraryPath}|{comicFolder.FullPath}";
               this.ComicFolders.Add(comicFolder);
               App.Database.Insert(comicFolder);

               folderPath = folderParent;
            }

         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region PrepareFoldersStructure
      private async Task PrepareFoldersStructure()
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
                  Folder.FolderData parentFolder = App.RootFolder;
                  if (!string.IsNullOrEmpty(DATA.ParentPath))
                  { parentFolder = this.ComicFoldersDictionary[DATA.ParentPath]; }
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

         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region PrepareFilesStructure
      private async Task PrepareFilesStructure()
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
                  Folder.FolderData parentFolder = App.RootFolder;
                  if (!string.IsNullOrEmpty(DATA.ParentPath))
                  { parentFolder = this.ComicFoldersDictionary[DATA.ParentPath]; }
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

      #region AnalyseFoldersAvailability
      private async Task AnalyseFoldersAvailability()
      { await this.AnalyseFoldersAvailability(App.RootFolder); }
      private async Task AnalyseFoldersAvailability(Folder.FolderData folder)
      {
         try
         {
            folder.Available = folder.HasFiles || folder.HasFolders;
            for (int childrenIndex = (folder.Folders.Count - 1); childrenIndex >= 0; childrenIndex--)
            {
               var childrenFolder = folder.Folders[childrenIndex];
               this.AnalyseFoldersAvailability(childrenFolder);
               if (!childrenFolder.Available) { folder.Folders.RemoveAt(childrenIndex); }
            }
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region DefineFirstFolder
      private async Task DefineFirstFolder()
      {
         try
         {
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
            App.RootFolder.Text = R.Strings.AppTitle;
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
         this.ComicFoldersDictionary = null;
      }
      #endregion

   }
}