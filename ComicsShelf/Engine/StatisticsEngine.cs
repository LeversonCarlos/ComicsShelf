using System;
using System.Linq;

namespace ComicsShelf.Engine
{
   internal class Statistics : BaseEngine
   {

      #region Execute
      public static async void Execute()
      {
         try
         {
            using (var engine = new Statistics())
            {

               // TRACK
               var initializeTime = DateTime.Now;
               AppCenter.TrackEvent("Statistics: Executing");

               // EXECUTION
               engine.RecentFiles();
               engine.TopRatedFiles();
               engine.ReadingFiles();

               // TRACK
               var milliseconds = (long)(DateTime.Now - initializeTime).TotalMilliseconds;
               AppCenter.TrackEvent("Statistics: Executed", "milliseconds", milliseconds.ToString());

            }
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
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
            var recentFiles = App.HomeData.Files
               .Where(x => !string.IsNullOrEmpty(x.ComicFile.ReleaseDate))
               .OrderByDescending(x => x.ComicFile.ReleaseDate)
               .Take(5)
               .ToList();
            if (this.HasChanged(App.HomeData.RecentFiles, recentFiles))
            { App.HomeData.RecentFiles.ReplaceRange(recentFiles); }
         }
         catch (Exception) { throw; }
      }
      #endregion

      #region TopRatedFiles
      private void TopRatedFiles()
      {
         try
         {

            // GET ALL RATED FILES
            var allRatedFiles = App.HomeData.Files
               .Where(x => x.ComicFile.Rating > 0)
               .ToList();

            // GROUP FILES WITH ITS AVERAGE RATING
            var groupFiles = allRatedFiles
               .GroupBy(x => x.ComicFile.ParentPath)
               .Select(x => new
               {
                  ParentPath = x.Key,
                  Rating = x.Average(g => (double)g.ComicFile.Rating)
               })
               .ToList();

            // TOP RATED FILES
            var topRatedFiles = allRatedFiles
               .Select(x => new
               {
                  File = x,
                  GroupRating = groupFiles
                     .Where(g => g.ParentPath == x.ComicFile.ParentPath)
                     .Select(g => g.Rating)
                     .FirstOrDefault()
               })
               .OrderByDescending(x => x.GroupRating)
               .ThenByDescending(x => x.File.ComicFile.Rating)
               .ThenByDescending(x => x.File.ComicFile.ReadingDate)
               .Select(x => x.File)
               .Take(10)
               .ToList();

            // APPLY
            if (this.HasChanged(App.HomeData.TopRatedFiles, topRatedFiles))
            { App.HomeData.TopRatedFiles.ReplaceRange(topRatedFiles); }

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
            var openedFiles = App.HomeData.Files
               .Where(x => x.ComicFile.ReadingPercent > 0.0 && x.ComicFile.ReadingPercent < 1.0)
               .OrderByDescending(x => x.ComicFile.ReadingDate)
               .Take(10)
               .ToList();

            // GET ALL READED FILES
            var readedFiles = App.HomeData.Files
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
               .Select(x => App.HomeData.Files
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
            if (this.HasChanged(App.HomeData.ReadingFiles, readingFiles))
            { App.HomeData.ReadingFiles.ReplaceRange(readingFiles); }

         }
         catch (Exception) { throw; }
      }
      #endregion

   }
}
