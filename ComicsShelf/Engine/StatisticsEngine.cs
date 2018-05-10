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
               engine.RecentFiles();
               engine.TopRatedFiles();
               engine.ReadingFiles();
            }
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region RecentFiles
      private void RecentFiles()
      {
         try
         {
            var recentFiles = App.RootFolder.Files
               .Where(x => !string.IsNullOrEmpty(x.PersistentData.ReleaseDate))
               .OrderByDescending(x => x.PersistentData.ReleaseDate)
               .Take(5)
               .ToList();
            App.RootFolder.RecentFiles.ReplaceRange(recentFiles);
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region TopRatedFiles
      private void TopRatedFiles()
      {
         try
         {

            // GET ALL RATED FILES
            var allRatedFiles = App.RootFolder.Files
               .Where(x => x.PersistentData.Rate.HasValue)
               .ToList();

            // GROUP FILES WITH ITS AVERAGE RATE
            var groupFiles = allRatedFiles
               .GroupBy(x => x.PersistentData.ParentPath)
               .Select(x => new
               {
                  ParentPath = x.Key,
                  Rate = x.Average(g => (double)g.PersistentData.Rate.Value)
               })
               .ToList();

            // TOP RATED FILES
            var topRatedFiles = allRatedFiles
               .Select(x => new
               {
                  File = x,
                  GroupRate = groupFiles
                     .Where(g => g.ParentPath == x.PersistentData.ParentPath)
                     .Select(g => g.Rate)
                     .FirstOrDefault()
               })
               .OrderByDescending(x => x.GroupRate)
               .ThenByDescending(x => x.File.PersistentData.Rate.Value)
               .ThenByDescending(x => x.File.PersistentData.ReadingDate)
               .Select(x => x.File)
               .Take(10)
               .ToList();

            //APPLY
            App.RootFolder.TopRatedFiles.ReplaceRange(topRatedFiles);
            
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region ReadingFiles
      private void ReadingFiles()
      {
         try
         {

            // GET LAST 10 OPENED FILES
            var openedFiles = App.RootFolder.Files
               .Where(x => x.PersistentData.ReadingPercent > 0.0 && x.PersistentData.ReadingPercent < 1.0)
               .OrderByDescending(x => x.PersistentData.ReadingDate)
               .Take(10)
               .ToList();

            // GET ALL READED FILES
            var readedFiles = App.RootFolder.Files
               .Where(x => x.PersistentData.ReadingPercent == 1.0)
               .ToList();

            // REMOVE GROUPS THAT ALREADY HAS SOME OPENED FILES
            readedFiles
               .RemoveAll(x => openedFiles
                  .Select(g => g.PersistentData.ParentPath)
                  .Contains(x.PersistentData.ParentPath));

            // GROUP FILES AND MANTAIN ONLY THE MOST RECENT OPENED FILE FOR EACH GROUP
            readedFiles = readedFiles
               .GroupBy(x => x.PersistentData.ParentPath)
               .Select(x => readedFiles
                  .Where(g => g.PersistentData.ParentPath == x.Key)
                  .OrderByDescending(g => g.PersistentData.ReadingDate)
                  .FirstOrDefault())
               .ToList();

            // FROM THAT, TAKE THE NEXT FILE FOR EACH GROUP
            var readedNextFiles = readedFiles
               .Select(x => App.RootFolder.Files
                  .Where(f => f.PersistentData.ParentPath == x.PersistentData.ParentPath)
                  .Where(f => String.Compare(f.PersistentData.FullPath, x.PersistentData.FullPath) > 0)
                  .OrderBy(f => f.PersistentData.FullPath)
                  .Take(1)
                  .Select(f => new { f, x.PersistentData.ReadingDate })
                  .FirstOrDefault())
               .Where(x => x != null)
               .ToList();

            // UNION OPEN AND READED FILES
            var unionFiles = readedNextFiles;
            unionFiles.AddRange(openedFiles.Select(f => new { f, f.PersistentData.ReadingDate }).AsEnumerable());

            // TAKE THE LAST 10
            var readingFiles = unionFiles
               .OrderByDescending(x => x.ReadingDate)
               .Select(x => x.f)
               .Take(10)
               .ToList();

            // APPLY
            App.RootFolder.ReadingFiles.ReplaceRange(readingFiles);

         }
         catch (Exception ex) { throw; }
      }
      #endregion

   }
}
