using ComicsShelf.Helpers;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engine.CoverExtraction
{
   public class Service
   {

      public static Task Execute() => Task.Factory.StartNew(() => ExecuteAsync(), TaskCreationOptions.LongRunning);

      private static async Task ExecuteAsync()
      {
         try
         {
            var store = DependencyService.Get<IStoreService>();
            if (!System.IO.Directory.Exists(Helpers.Paths.CoversCache))
               System.IO.Directory.CreateDirectory(Helpers.Paths.CoversCache);

            // RETRIEVE DATA
            var libraries = store.GetLibraries();
            var sections = store.GetSections();

            // SUGGESTION ITEMS (sections without library)
            var suggestionItems = sections
               .Where(section => section.Library == null)
               .SelectMany(x => x.Folders)
               .Select(x => x.FirstItem)
               .Where(x => x.CoverPath == Helpers.Cover.DefaultCover)
               .ToArray();
            var suggestionData = suggestionItems
               .GroupBy(item => item.LibraryID)
               .Select(grp => new
               {
                  LibraryID = grp.Key,
                  Library = libraries.FirstOrDefault(x => x.ID == grp.Key),
                  Items = grp.ToArray()
               })
               .ToArray();
            foreach (var libraryData in suggestionData)
               if (!await ExtractCovers(libraryData.Library, libraryData.Items)) { return; }

            // FEATURED ITEMS (firstItem of the folders on each section)
            var featuredItems = sections
               .Where(section => section.Library != null)
               .SelectMany(x => x.Folders)
               .Select(x => x.FirstItem)
               .Where(x => x.CoverPath == Helpers.Cover.DefaultCover)
               .ToArray();
            var featuredData = featuredItems
               .GroupBy(item => item.LibraryID)
               .Select(grp => new
               {
                  LibraryID = grp.Key,
                  Library = libraries.FirstOrDefault(x => x.ID == grp.Key),
                  Items = grp.ToArray()
               })
               .ToArray();
            foreach (var libraryData in featuredData)
               if (!await ExtractCovers(libraryData.Library, libraryData.Items)) { return; }

            // REMAINING ITEMS (all that is left)
            foreach (var library in libraries)
            {
               var remainingItems = store
                  .GetLibraryItems(library)
                  .Where(x => x.CoverPath == Helpers.Cover.DefaultCover)
                  .ToArray();
               if (!await ExtractCovers(library, remainingItems)) { return; }
            }

            //store.SetSections(sections);
            //Notifyers.Notify.SectionsUpdate(sections);

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

      private static async Task<bool> ExtractCovers(LibraryVM library, ItemVM[] itemList)
      {
         try
         {

            // NOTIFY START MESSAGE
            Helpers.Notify.Message(library, "Start.Covers.Extraction");
            var drive = Drive.BaseDrive.GetDrive(library.Type);

            // SLEEP NOTIFICATION
            var hasGoneSleep = false;
            Notify.AppSleep(now => hasGoneSleep = true);

            // LOOP THROUGH ITEMS
            int itemIndex = 0;
            foreach (var item in itemList)
            {
               var progress = (double)++itemIndex / (double)itemList.Length;
               Notify.Message(library, item.FullText);
               Notify.Progress(library, progress);

               var start = DateTime.Now;
               try
               {
                  await semaphore.WaitAsync();
                  start = DateTime.Now;
                  if (!await drive.ExtractCover(library, item)) { throw new Exception($"Stop extracting covers at the item [{item.FullText}]"); }
               }
               catch (Exception exI) { Insights.TrackException(exI); break; }
               finally { semaphore.Release(); Helpers.Insights.TrackEvent($"Cover Extracting", $"Seconds:{Math.Round(DateTime.Now.Subtract(start).TotalSeconds, 0)}"); }

         if (hasGoneSleep) { break; }
            }

            // NOTIFY FINISH MESSAGE
            Helpers.Notify.Progress(library, 1);
            Helpers.Notify.Message(library, "");

            return true;
         }
         catch (Exception ex) { Insights.TrackException(ex); return false; }
      }

      static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

   }
}
