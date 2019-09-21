using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Services
{
   partial class LibraryService 
   {

      private async Task<bool> Statistics()
      {
         try
         {

            // READING FILES
            var readingFiles = this.Statistics_GetReadingFiles();
            if (readingFiles == null) { return false; }
            await this.NotifyData("OnRefreshingReadingFilesList", readingFiles);

            // RECENT FILES
            var recentFiles = this.Statistics_GetRecentFiles();
            if (recentFiles == null) { return false; }
            await this.NotifyData("OnRefreshingRecentFilesList", recentFiles);

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private List<ComicFiles.ComicFileVM> Statistics_GetRecentFiles()
      {
         try
         {

            var recentFiles = this.GetLibraryFiles()
               .Where(file => file.ComicFile.Available)
               .Where(file => file.ComicFile.ReleaseDate != DateTime.MinValue)
               .GroupBy(file => new { file.ComicFile.FolderPath, file.ComicFile.ReleaseDate.Year, file.ComicFile.ReleaseDate.Month, file.ComicFile.ReleaseDate.Day })
               .Select(file => file.OrderByDescending(g => g.ComicFile.ReleaseDate).FirstOrDefault())
               .OrderByDescending(x => x.ComicFile.ReleaseDate)
               .Take(25)
               .ToList();

            var generalFiles = this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.RecentFiles]
               .Where(x => x.ComicFile.LibraryKey != this.Library.ID)
               .ToList();

            recentFiles = recentFiles
               .Union(generalFiles)
               .OrderByDescending(x => x.ComicFile.ReleaseDate)
               .ToList();

            this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.RecentFiles] = recentFiles;
            return recentFiles.Take(50).ToList();
         }
         catch (Exception) { throw; }
      }

      private List<ComicFiles.ComicFileVM> Statistics_GetReadingFiles()
      {
         try
         {

            var libraryFiles = this.GetLibraryFiles()
               .Where(file => file.ComicFile.Available);

            // GET LAST 10 OPENED FILES
            var openedFiles = libraryFiles
               .Where(file => file.ComicFile.ReadingPercent > 0.0 && file.ComicFile.ReadingPercent < 1.0)
               .OrderByDescending(file => file.ComicFile.ReadingDate)
               .Take(10)
               .ToList();

            // GET ALL READED FILES
            var readedFiles = libraryFiles
               .Where(x => x.ComicFile.ReadingPercent == 1.0)
               .ToList();

            // REMOVE GROUPS THAT ALREADY HAS SOME OPENED FILES
            readedFiles
               .RemoveAll(x => openedFiles
                  .Select(g => g.ComicFile.FolderPath)
                  .Contains(x.ComicFile.FolderPath));

            // GROUP FILES AND MANTAIN ONLY THE MOST RECENT FILE FOR EACH GROUP
            readedFiles = readedFiles
               .GroupBy(x => x.ComicFile.FolderPath)
               .Select(x => readedFiles
                  .Where(g => g.ComicFile.FolderPath == x.Key)
                  .OrderByDescending(g => g.ComicFile.FilePath)
                  .FirstOrDefault())
               .ToList();

            // FROM THAT, TAKE THE NEXT FILE FOR EACH GROUP
            var readedNextFiles = readedFiles
               .Select(x => libraryFiles
                  .Where(f => f.ComicFile.FolderPath == x.ComicFile.FolderPath)
                  .Where(f => String.Compare(f.ComicFile.FilePath, x.ComicFile.FilePath) > 0)
                  .OrderBy(f => f.ComicFile.FilePath)
                  .Take(1)
                  .Select(f => new { f, x.ComicFile.ReadingDate })
                  .FirstOrDefault())
               .Where(x => x != null)
               .ToList();

            // UNION OPEN AND READED FILES
            var unionFiles = readedNextFiles;
            unionFiles.AddRange(openedFiles.Select(f => new { f, f.ComicFile.ReadingDate }).AsEnumerable());

            // TAKE THE LAST 10
            var readingFiles = unionFiles
               .OrderByDescending(x => x.ReadingDate)
               .Select(x => x.f)
               .Take(10)
               .ToList();

            // ATTACH GENERAL
            var generalFiles = this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.ReadingFiles]
               .Where(x => x.ComicFile.LibraryKey != this.Library.ID)
               .ToList();
            readingFiles = readingFiles
               .Union(generalFiles)
               .OrderByDescending(x => x.ReadingDate)
               .ToList();

            this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.ReadingFiles] = readingFiles;
            return readingFiles.Take(10).ToList();            
         }
         catch (Exception) { throw; }
      }

   }
}
