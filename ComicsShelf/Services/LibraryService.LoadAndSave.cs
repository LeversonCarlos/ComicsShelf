using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Services
{
   partial class LibraryService 
   {

      private async Task<bool> LoadData()
      {
         try
         {
            this.Notify.Send(this.Library, $"{this.Library.Description}: {R.Strings.SEARCH_ENGINE_LOADING_DATABASE_DATA_MESSAGE}");

            // EXISTING FILES
            var existingKeys = this.GetLibraryFiles().Select(x => x.ComicFile.Key).ToList();

            // DATABASE FILES
            var databaseFiles = await Helpers.FileStream.ReadFile<List<ComicFiles.ComicFile>>(Helpers.Constants.DatabaseFile);
            if (databaseFiles == null) { return true; }

            // NEW FILES
            var comicFiles = databaseFiles
               .Where(x => x.LibraryKey == this.Library.ID)
               .Where(x => !existingKeys.Contains(x.Key))
               .GroupBy(x => x.Key)
               .Select(x => new { Item = x.FirstOrDefault(), Count = x.Count() })
               .Select(x => new ComicFiles.ComicFileVM(x.Item))
               .ToList();

            foreach (var comicFile in comicFiles)
            {
               if (System.IO.File.Exists(comicFile.ComicFile.CoverPath))
               {
                  comicFile.CoverPath = comicFile.ComicFile.CoverPath;
                  if (comicFile.ComicFile.ReleaseDate == DateTime.MinValue)
                  { comicFile.ComicFile.ReleaseDate = System.IO.File.GetLastWriteTime(comicFile.CoverPath); }
               }
               if (System.IO.Directory.Exists(comicFile.ComicFile.CachePath))
               { comicFile.CachePath = comicFile.ComicFile.CachePath; }
               else { comicFile.CachePath = string.Empty; }
            }

            this.AddLibraryFiles(comicFiles);
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private async Task<bool> SaveData()
      {
         try
         {

            var comicFiles = this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.Libraries]
               .Select(x => x.ComicFile)
               .GroupBy(x => x.Key)
               .Select(x => new { Item = x.FirstOrDefault(), Count = x.Count() })
               .Select(x => x.Item)
               .ToList();
            if (comicFiles == null) { return true; }

            if (!await Helpers.FileStream.SaveFile(Helpers.Constants.DatabaseFile, comicFiles)) { return false; }
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
