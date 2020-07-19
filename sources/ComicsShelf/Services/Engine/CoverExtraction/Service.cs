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
   public class Service : IDisposable
   {

      public static Task Execute()
      {

         if (_CancellationTokenSource != null)
            _CancellationTokenSource?.Cancel();
         while (_CancellationTokenSource != null) { }

         _CancellationTokenSource = new CancellationTokenSource();
         var token = _CancellationTokenSource.Token;
         var taskFactory = new TaskFactory(token);

         var task = taskFactory.StartNew(async () =>
         {
            using (var service = new Service())
               await service.ExecuteAsync(token);
         }, TaskCreationOptions.LongRunning);

         return task;
      }

      static CancellationTokenSource _CancellationTokenSource = null;
      static SemaphoreSlim _Semaphore = new SemaphoreSlim(1, 1);
      bool _CancelExecution = false;

      async Task ExecuteAsync(CancellationToken token)
      {
         try
         {

            // CANCEL NOTIFICATIONS
            Notify.AppSleep(this, now => _CancelExecution = true);
            Notify.ReadingStart(this, now => _CancelExecution = true);
            token.Register(() => _CancelExecution = true);

            // VALIDATE
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
                  Items = grp
                     .OrderBy(item => item.FolderPath)
                     .ThenBy(item => item.FullText)
                     .ToArray()
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
                  Items = grp
                     .OrderBy(item => item.FolderPath)
                     .ThenBy(item => item.FullText)
                     .ToArray()
               })
               .ToArray();
            foreach (var libraryData in featuredData)
               if (!await ExtractCovers(libraryData.Library, libraryData.Items)) { return; }

            // REMAINING ITEMS (all that is left)
            foreach (var library in libraries)
            {
               var remainingItems = store
                  .GetLibraryItems(library)
                  .Where(item => item.CoverPath == Helpers.Cover.DefaultCover)
                  .OrderBy(item => item.FolderPath)
                  .ThenBy(item => item.FullText)
                  .ToArray();
               if (!await ExtractCovers(library, remainingItems)) { return; }
            }

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
         finally
         {
            Notify.AppSleepUnsubscribe(this);
            Notify.ReadingStartUnsubscribe(this);
         }
      }

      async Task<bool> ExtractCovers(LibraryVM library, ItemVM[] itemList)
      {
         using (var log = new Helpers.InsightsLogger($"{library.Type} Cover Extracting"))
         {
            int itemIndex = 0;
            try
            {
               var drive = Drive.BaseDrive.GetDrive(library.Type);

               // LOOP THROUGH ITEMS
               foreach (var item in itemList)
               {
                  var progress = (double)++itemIndex / (double)itemList.Length;
                  Notify.Message(library, item.FullText);
                  Notify.Progress(library, progress);

                  try
                  {
                     await _Semaphore.WaitAsync();
                     if (!await drive.ExtractCover(library, item))
                        break;
                  }
                  catch (Exception exI) { Insights.TrackException(exI); break; }
                  finally { _Semaphore.Release(); }

                  if (_CancelExecution) { break; }
               }

               // NOTIFY FINISH MESSAGE
               Notify.Progress(library, 1);
               Notify.Message(library, "");

               return !_CancelExecution;
            }
            catch (Exception ex) { log.Add(ex); Insights.TrackException(ex); return false; }
            finally { log.SetDurationFactor(itemIndex); }
         }
      }

      public void Dispose()
      {
         _CancellationTokenSource.Dispose();
         _CancellationTokenSource = null;
      }

   }
}
