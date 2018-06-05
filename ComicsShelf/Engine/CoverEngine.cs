using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class Cover : BaseEngine
   {
      private Dictionary<string, Folder.FolderData> ComicFoldersDictionary { get; set; }

      #region Execute
      public static async void Execute(Dictionary<string, Folder.FolderData> comicFoldersDictionary)
      {
         try
         {
            System.Diagnostics.Debug.WriteLine("Cover Engine Start");
            using (var engine = new Cover())
            {
               engine.ComicFoldersDictionary = comicFoldersDictionary;
               await engine.ExtractComicData();
               Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Statistics.Execute());
            }
            System.Diagnostics.Debug.WriteLine("Cover Engine Finish");
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region ExtractComicData
      private async Task ExtractComicData()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_EXTRACTING_COMICS_COVER_MESSAGE);

            // LOOP THROUGH FILES
            var filesQuantity = App.RootFolder.Files.Count;
            for (int fileIndex = 0; fileIndex < filesQuantity; fileIndex++)
            {
               var file = App.RootFolder.Files[fileIndex];
               var progress = ((double)fileIndex / (double)filesQuantity);
               this.Notify(file.Text, progress);

               /*
               var progressPercent = Math.Round((Math.Round(progress, 2) % (double)0.10), 2);
               if (progressPercent == (double)0)
               { Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Statistics.Execute()); }
               */

               if (!await this.DataAlreadyExists(file))
               {
                  await Task.Run(() => this.ExtractFileData(file));
                  await Task.Run(() => this.ExtractFolderData(file));
               }
            }

         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region DataAlreadyExists
      private async Task< bool> DataAlreadyExists(File.FileData file)
      {
         if (file.FullPath.ToLower().EndsWith(".cbr")) { return true; }
         if (file.PersistentData != null && string.IsNullOrEmpty(file.PersistentData.ReleaseDate)) { return false; }
         if (await Task.Run(() => System.IO.File.Exists(file.CoverPath))) { return true; }
         return false;
      }
      #endregion

      #region ExtractFileData
      private async Task ExtractFileData(File.FileData file)
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
                  if (!await this.FileSystem.FileExists(file.CoverPath))
                  {
                     await this.FileSystem.Thumbnail(zipStream, file.CoverPath);
                     file.CoverPath = file.CoverPath;
                  }
               }
            }

         }
         catch (Exception ex) { throw; }
         finally { GC.Collect(); }
      }
      #endregion

      #region ExtractFolderData
      private async Task ExtractFolderData(File.FileData file)
      {
         try
         {

            // APPLY COVER PATH TO THE FOLDER STRUCTURE
            var parentFolder = this.ComicFoldersDictionary[file.PersistentData.ParentPath];
            while (parentFolder != null)
            {
               if (string.IsNullOrEmpty(parentFolder.PersistentData.CoverPath))
               {
                  parentFolder.CoverPath = file.CoverPath;
                  parentFolder.PersistentData.CoverPath = file.CoverPath;
                  await Task.Run(() => App.Database.Update(parentFolder.PersistentData));
               }
               // else { parentFolder.CoverPath = parentFolder.CoverPath; }

               if (string.IsNullOrEmpty(parentFolder.PersistentData.ParentPath)) { break; }
               parentFolder = this.ComicFoldersDictionary[parentFolder.PersistentData.ParentPath];
            }

         }
         catch (Exception ex) { throw; }
      }
      #endregion

   }
}