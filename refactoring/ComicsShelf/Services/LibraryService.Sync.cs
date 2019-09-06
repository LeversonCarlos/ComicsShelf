using System;
using System.Threading.Tasks;

namespace ComicsShelf.Services
{
   partial class LibraryService 
   {

      private async Task<bool> LoadSyncData()
      {
         try
         {

            /*
            var syncData = await LibrarySync.LoadSyncData(library);
            if (syncData == null) { return true; }

            if (!this.ComicFiles.ContainsKey(library.ID)) { return true; }
            var comicFiles = this.ComicFiles[library.ID];
            if (comicFiles == null) { return true; }

            if (comicFiles.Count > 0 && syncData.Count > 0)
            {
               foreach (var syncFile in syncData)
               {
                  var comicFile = comicFiles.Where(x => x.ComicFile.Key == syncFile.Key).FirstOrDefault();
                  if (comicFile == null)
                  { comicFile = comicFiles.Where(x => x.ComicFile.OldKey == syncFile.Key).FirstOrDefault(); }
                  if (comicFile != null) { comicFile.Set(syncFile.ToComicFile()); }
               }
            }
            */

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private async Task<bool> SaveSyncData()
      {
         try
         {
            /*
            var comicFiles = this.ComicFiles[library.ID].Select(x => x.ComicFile).ToList();
            return await LibrarySync.SaveSyncData(library, comicFiles);
            */
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
