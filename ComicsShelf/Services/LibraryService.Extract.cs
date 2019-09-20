using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Services
{
   partial class LibraryService
   {

      private async Task<bool> ExtractData()
      {
         try
         {
            var startTime = DateTime.Now;

            // FEATURED FILES
            this.Notify.Send(this.Library, $"{this.Library.Description}: {R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_FEATURED_FILES_MESSAGE}");
            var recentFiles = this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.RecentFiles]
               .Where(x => x.ComicFile.LibraryKey == this.Library.ID)
               .ToList();
            var readingFiles = this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.ReadingFiles]
               .Where(x => x.ComicFile.LibraryKey == this.Library.ID)
               .ToList();
            var featuredFileIDs = recentFiles
               .Union(readingFiles)
               .Select(x => x.ComicFile.Key)
               .ToList();
            var featuredFiles = this.GetLibraryFiles()
               .Where(file => file.ComicFile.Available)
               .Where(file => string.IsNullOrEmpty(file.CoverPath) || file.CoverPath == Helpers.Constants.DefaultCover)
               .Where(file => featuredFileIDs.Contains(file.ComicFile.Key))
               .OrderBy(file => file.ComicFile.FolderPath)
               .ThenByDescending(file => file.ComicFile.FilePath)
               .ToList();
            if (featuredFiles != null)
            { if (!await this.ExtractData(featuredFiles)) { return false; } }

            // FIRST SIX FILES
            this.Notify.Send(this.Library, $"{this.Library.Description}: {R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_FEATURED_FILES_MESSAGE}");
            var firstSixFiles = this.GetLibraryFiles()
               .Where(file => file.ComicFile.Available)
               .GroupBy(file => file.ComicFile.FolderPath)
               .SelectMany(file => file
                  .OrderByDescending(order => order.ComicFile.FilePath)
                  .Take(6)
                  .ToList())
               .Where(file => string.IsNullOrEmpty(file.CoverPath) || file.CoverPath == Helpers.Constants.DefaultCover)
               .ToList();
            if (firstSixFiles != null)
            { if (!await this.ExtractData(firstSixFiles)) { return false; } }

            // REMAINING FILES
            this.Notify.Send(this.Library, $"{this.Library.Description}: {R.Strings.STARTUP_ENGINE_EXTRACTING_DATA_REMAINING_FILES_MESSAGE}");
            var remainingFiles = this.GetLibraryFiles()
               .Where(file => file.ComicFile.Available)
               .Where(file => string.IsNullOrEmpty(file.CoverPath) || file.CoverPath == Helpers.Constants.DefaultCover)
               .OrderBy(file => file.ComicFile.FolderPath)
               .ThenByDescending(file => file.ComicFile.FilePath)
               .ToList();
            if (remainingFiles != null)
            { if (!await this.ExtractData(remainingFiles)) { return false; } }

            var endTime = DateTime.Now;
            var trackProps = new Dictionary<string, string> {
               { "ElapsedMinutes", ((int)(endTime-startTime).TotalMinutes).ToString() },
               { "FilesCount", (firstSixFiles.Count()+remainingFiles.Count()).ToString() }
            };
            Helpers.AppCenter.TrackEvent($"Library.{this.Library.Type.ToString()}.ExtractData", trackProps);

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private async Task<bool> ExtractData(List<ComicFiles.ComicFileVM> comicFiles)
      {
         try
         {

            var engine = Engines.Engine.Get(this.Library.Type);

            var lastFolderPath = "";
            var filesQuantity = comicFiles.Count;
            for (int fileIndex = 0; fileIndex < filesQuantity; fileIndex++)
            {
               var comicFile = comicFiles[fileIndex];
               try
               {

                  // CHECK IF LIBRARY IS STILL AVAILABLED
                  if (this.Library.Removed) { return false; }

                  // CHECK IF THE APP HAS GONE SLEEP
                  if (App.IsSleeping) { return true; }

                  // PROGRESS
                  var progress = ((double)fileIndex / (double)filesQuantity);
                  this.Notify.Send(this.Library, comicFile.ComicFile.FullText, progress);

                  var coverResult = await Task.Run<bool>(async () =>
                  {
                     try
                     {

                        // CACHE PATH
                        if (System.IO.Directory.Exists(comicFile.ComicFile.CachePath))
                        { comicFile.CachePath = comicFile.ComicFile.CachePath; }
                        else { comicFile.CachePath = string.Empty; }

                        // CHECK IF THE COVER FILE ALREADY EXISTS
                        if (!string.IsNullOrEmpty(comicFile.CoverPath) && comicFile.CoverPath != Helpers.Constants.DefaultCover)
                        { return true; }
                        if (System.IO.File.Exists(comicFile.ComicFile.CoverPath))
                        {
                           comicFile.CoverPath = comicFile.ComicFile.CoverPath;
                           if (comicFile.ComicFile.ReleaseDate == DateTime.MinValue)
                           { comicFile.ComicFile.ReleaseDate = System.IO.File.GetLastWriteTime(comicFile.CoverPath); }
                           return true;
                        }

                        // COVER EXTRACT
                        if (await engine.ExtractCover(this.Library, comicFile.ComicFile))
                        { comicFile.CoverPath = comicFile.ComicFile.CoverPath; return true; }
                        else { return false; }

                     }
                     catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return false; }
                  });
                  if (!coverResult) { return false; }

                  // STATISTCS
                  if (lastFolderPath != comicFile.ComicFile.FolderPath)
                  {
                     lastFolderPath = comicFile.ComicFile.FolderPath;
                     await this.Statistics();
                     GC.Collect();
                  }

               }
               catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
            }

            return true;
         }
         catch (Exception) { throw; }
         finally { GC.Collect(); }
      }

   }
}
