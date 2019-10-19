using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Services
{
   partial class LibraryService
   {

      private async Task<bool> SearchData()
      {
         try
         {
            var startTime = DateTime.Now;
            this.Notify.Send(this.Library, $"{this.Library.Description}: {R.Strings.SEARCH_ENGINE_SEARCHING_COMIC_FILES_MESSAGE}");

            // ENGINE
            var engine = Engines.Engine.Get(this.Library.Type);
            var searchFiles = await engine.SearchFiles(this.Library);
            if (searchFiles == null) { return true; }
            var libraryFiles = this.GetLibraryFiles();

            // REMOVED FILES
            libraryFiles
               .Where(x => !searchFiles.Select(i => i.Key).ToList().Contains(x.ComicFile.Key))
               .ToList()
               .ForEach(file => file.ComicFile.Available = false);
            this.RemoveLibraryFiles(libraryFiles.Where(file => file.ComicFile.Available == false).ToList());

            // EXISTING FILES
            var existingFiles = libraryFiles
               .Where(file => file.ComicFile.Available == true);
            foreach (var existingFile in existingFiles)
            {
               var searchFile = searchFiles.Where(x => x.Key == existingFile.ComicFile.Key).FirstOrDefault();
               if (searchFile != null)
               {
                  existingFile.ComicFile.FolderPath = searchFile.FolderPath;
                  existingFile.ComicFile.FilePath = searchFile.FilePath;
                  existingFile.ComicFile.FullText = searchFile.FullText;
                  existingFile.ComicFile.SmallText = searchFile.SmallText;
               }
            }

            // NEW FILES
            var newFiles = searchFiles
               .Where(x => !libraryFiles.Select(i => i.ComicFile.Key).ToList().Contains(x.Key))
               .Select(x => new ComicFiles.ComicFileVM(x))
               .ToList();
            this.AddLibraryFiles(newFiles);

            // CHECK FOR DUPLICITY
            var duplicatedFiles = libraryFiles
               .GroupBy(x => x.ComicFile.Key)
               .Select(x => new { Item = x.FirstOrDefault(), Count = x.Count() })
               .Where(x => x.Count > 1)
               .Select(x => x.Item.ComicFile)
               .ToList();
            if (duplicatedFiles != null && duplicatedFiles.Count != 0)
            {
               foreach (var duplicatedFile in duplicatedFiles)
               {
                  this.ReplaceLibraryFile(duplicatedFile);
               }
            }

            var endTime = DateTime.Now;
            Helpers.AppCenter.TrackEvent($"Library.OnSearch", $"ElapsedSeconds:{((int)(endTime - startTime).TotalSeconds)}", $"NewFiles:{newFiles.Count()}", $"LibraryType:{this.Library.Type.ToString()}");

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
         finally { GC.Collect(); }
      }

   }
}
