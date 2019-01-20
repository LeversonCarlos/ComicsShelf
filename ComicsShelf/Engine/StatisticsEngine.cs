using System;
using System.Linq;

namespace ComicsShelf.Engine
{
   internal class Statistics : BaseEngine
   {

      #region Execute
      public static async void Execute(vTwo.Libraries.Library library)
      {
         try
         {
            using (var engine = new Statistics(library))
            {
               engine.RecentFiles();
               engine.ReadingFiles();
            }
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion


      #region Constructor
      private vTwo.Libraries.Library library { get; set; }
      private Views.Home.LibraryData libraryData { get; set; }
      public Statistics(vTwo.Libraries.Library library)
      {
         this.library = library;
         this.libraryData = App.HomeData.Libraries
            .Where(x => x.ComicFolder.LibraryPath == this.library.LibraryID)
            .FirstOrDefault();
      }
      #endregion

      #region HasChanged
      private bool HasChanged(Helpers.Observables.ObservableList<Views.File.FileData> from, System.Collections.Generic.List<Views.File.FileData> to)
      {
         var fromArray = from.Select(x => x.FullPath).ToList();
         var fromText = ""; fromArray.ForEach(x => fromText += $"{x}|");
         var toArray = to.Select(x => x.FullPath).ToList();
         var toText = ""; toArray.ForEach(x => toText += $"{x}|");
         return fromText != toText;
      }
      #endregion

      #region RecentFiles
      private void RecentFiles()
      {
         try
         {
            var recentFiles = this.libraryData.Files
               .Where(x => !string.IsNullOrEmpty(x.ComicFile.ReleaseDate))
               .OrderByDescending(x => x.ComicFile.ReleaseDate)
               .Take(5)
               .ToList();
            if (this.HasChanged(this.libraryData.RecentFiles.Files, recentFiles))
            {
               this.libraryData.RecentFiles.Files.ReplaceRange(recentFiles);
               this.libraryData.RecentFiles.HasFiles = this.libraryData.RecentFiles.Files.Count != 0;
            }
         }
         catch (Exception) { throw; }
      }
      #endregion

      #region ReadingFiles
      private void ReadingFiles()
      {
         try
         {

            // GET LAST 10 OPENED FILES
            var openedFiles = this.libraryData.Files
               .Where(x => x.ComicFile.ReadingPercent > 0.0 && x.ComicFile.ReadingPercent < 1.0)
               .OrderByDescending(x => x.ComicFile.ReadingDate)
               .Take(10)
               .ToList();

            // GET ALL READED FILES
            var readedFiles = this.libraryData.Files
               .Where(x => x.ComicFile.ReadingPercent == 1.0)
               .ToList();

            // REMOVE GROUPS THAT ALREADY HAS SOME OPENED FILES
            readedFiles
               .RemoveAll(x => openedFiles
                  .Select(g => g.ComicFile.ParentPath)
                  .Contains(x.ComicFile.ParentPath));

            // GROUP FILES AND MANTAIN ONLY THE MOST RECENT OPENED FILE FOR EACH GROUP
            readedFiles = readedFiles
               .GroupBy(x => x.ComicFile.ParentPath)
               .Select(x => readedFiles
                  .Where(g => g.ComicFile.ParentPath == x.Key)
                  .OrderByDescending(g => g.ComicFile.ReadingDate)
                  .FirstOrDefault())
               .ToList();

            // FROM THAT, TAKE THE NEXT FILE FOR EACH GROUP
            var readedNextFiles = readedFiles
               .Select(x => this.libraryData.Files
                  .Where(f => f.ComicFile.ParentPath == x.ComicFile.ParentPath)
                  .Where(f => String.Compare(f.ComicFile.FullPath, x.ComicFile.FullPath) > 0)
                  .OrderBy(f => f.ComicFile.FullPath)
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

            // APPLY
            if (this.HasChanged(this.libraryData.ReadingFiles.Files, readingFiles))
            {
               this.libraryData.ReadingFiles.Files.ReplaceRange(readingFiles);
               this.libraryData.ReadingFiles.HasFiles = this.libraryData.ReadingFiles.Files.Count != 0;
            }

         }
         catch (Exception) { throw; }
      }
      #endregion

   }
}
