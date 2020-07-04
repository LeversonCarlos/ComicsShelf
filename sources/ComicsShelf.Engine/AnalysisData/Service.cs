using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engine.AnalysisData
{
   public class Service
   {

      public static Task Execute(ItemVM[] itemsList) => Task.Factory.StartNew(() => ExecuteAsync(itemsList), TaskCreationOptions.LongRunning);

      private static async Task ExecuteAsync(ItemVM[] itemsList)
      {
         var start = DateTime.Now;
         try
         {
            var store = DependencyService.Get<IStoreService>();
            var sections = new List<SectionVM>();

            // RETRIEVE ALL LIBRARY ITEMS
            var libraries = store.GetLibraries();
            var allItems = new List<ItemVM>();

            // LOOP THROUGH LIBRARIES
            foreach (var library in libraries)
            {
               Helpers.Notify.Message(library, Strings.START_MESSAGE);

               // LOCATE LIBRARY ITEMS
               var libraryItems = store
                  .GetLibraryItems(library)
                  .ToList();
               allItems.AddRange(libraryItems);

               // SECTIONS FOR THE LIBRARY
               sections.AddRange(ExecuteAsync_GetLibrarySections(library, libraryItems));

               Helpers.Notify.Message(library, "");
            }

            // SECTION FOR THE RECENTELY ADDED ITEMS
            var recentlyAdded = ExecuteAsync_GetRecentlyAddedSection(allItems);
            if (recentlyAdded != null)
               sections.Insert(0, recentlyAdded);

            //SECTION FOR THE ONGOING READING ITEMS
            var onGoingReading = ExecuteAsync_GetOnGoingReadingSection(allItems);
            if (onGoingReading != null)
               sections.Insert(0, onGoingReading);

            var sectionsArray = sections.ToArray();
            store.SetSections(sectionsArray);
            Helpers.Notify.SectionsUpdate(sectionsArray);

            await Task.CompletedTask;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
         finally { Helpers.Insights.TrackMetric($"Analysing Data", DateTime.Now.Subtract(start).TotalSeconds); }
      }

      private static List<SectionVM> ExecuteAsync_GetLibrarySections(LibraryVM library, List<ItemVM> libraryItems)
      {
         try
         {
            var sections = new List<SectionVM>();

            // GROUP ITEMS BY SECTION AND LOOP IT
            var sectionsList = libraryItems
               .GroupBy(section => new { Path = section.SectionPath })
               .Select(section => new { Path = section.Key.Path, Items = section.ToArray() })
               .OrderBy(section => section.Path)
               .ToArray();
            foreach (var sectionItem in sectionsList)
            {

               // PREPARE A SECTION
               var sectionText = System.IO.Path.GetFileNameWithoutExtension(sectionItem.Path);
               var section = new SectionVM(sectionText, library);

               // GROUP ITEMS BY FOLDER AND LOOP IT
               var foldersList = sectionItem.Items
                  .Where(folder => folder.SectionPath == sectionItem.Path)
                  .GroupBy(folder => new { Path = folder.FolderPath })
                  .Select(folder => new { Path = folder.Key.Path, Items = folder.ToArray() })
                  .OrderBy(folder => folder.Path)
                  .ToArray();
               foreach (var folderItem in foldersList)
               {

                  // PREPARE A FOLDER
                  var folderText = System.IO.Path.GetFileNameWithoutExtension(folderItem.Path);
                  var folder = new FolderVM(folderText);

                  // LOCATE THE FOLDERs FIRST ITEM
                  folder.FirstItem = folderItem.Items
                     ?.OrderBy(item => item.ShortText)
                     ?.FirstOrDefault();

                  // ADD FOLDER TO RESULT LIST
                  section.Folders.Add(folder);

               }

               // ADD section TO RESULT LIST
               sections.Add(section);

            }

            return sections;
         }
         catch (Exception) { throw; }
      }

      private static SectionVM ExecuteAsync_GetRecentlyAddedSection(List<ItemVM> libraryItems)
      {
         try
         {

            var sectionItems = libraryItems
               .Where(item => item.Available && item.ReleaseDate != DateTime.MinValue)
               .GroupBy(item => new { item.FolderPath, item.ReleaseDate.Year, item.ReleaseDate.Month, item.ReleaseDate.Day })
               .Select(group => group.OrderByDescending(item => item.ReleaseDate).FirstOrDefault())
               .OrderByDescending(item => item.ReleaseDate)
               .Take(10)
               .ToArray();
            if (sectionItems?.Length == 0) { return null; }

            var section = new SectionVM(Translations.RECENTLY_ADDED_TITLE, null);

            foreach (var item in sectionItems)
               section.Folders.Add(new FolderVM(item.FullText) { FirstItem = item });

            return section;
         }
         catch (Exception) { throw; }
      }

      private static SectionVM ExecuteAsync_GetOnGoingReadingSection(List<ItemVM> libraryItems)
      {
         try
         {

            // LOCATE ALL OPENED ITEMS
            var openedItems = libraryItems
               .Where(item => item.ReadingPercent > 0.0 && item.ReadingPercent < 1.0 && item.ReadingDate.HasValue)
               .OrderByDescending(item => item.ReadingDate.Value)
               .ToArray();

            // LOCATE ALL COMPLETELY READED ITEMS
            var readedFiles = libraryItems
               .Where(item => item.ReadingPercent == 1.0)
               .ToList();

            // REMOVE GROUPS THAT ALREADY HAS SOME OPENED ITEMS
            readedFiles
               .RemoveAll(x => openedItems
                  .Select(g => g.FolderPath)
                  .Contains(x.FolderPath));

            // GROUP ITEMS AND MANTAIN ONLY THE MOST RECENT FOR EACH GROUP
            readedFiles = readedFiles
               .Union(openedItems)
               .ToList();
            readedFiles = readedFiles
               .GroupBy(item => item.FolderPath)
               .Select(x => readedFiles
                  .Where(g => g.FolderPath == x.Key)
                  .OrderByDescending(g => g.FullText)
                  .FirstOrDefault())
               .ToList();

            // FROM THAT, TAKE THE NEXT FILE FOR EACH GROUP
            var unionFiles = readedFiles
               .Select(x => new
               {
                  x.ReadingDate,
                  Item = x,
                  NextItem = libraryItems
                     .Where(f => f.FolderPath == x.FolderPath)
                     .Where(f => String.Compare(f.FullText, x.FullText) > 0)
                     .OrderBy(f => f.FullText)
                     .Take(1)
                     .FirstOrDefault()
               })
               .ToArray();

            // TAKE THE LAST 10
            var sectionItems = unionFiles
               .OrderByDescending(x => x.ReadingDate)
               .Select(x => x.Item.ReadingPercent >= 1 ? x.NextItem : x.Item)
               .Where(x => x != null)
               .Take(10)
               .ToArray();
            if (sectionItems?.Length == 0) { return null; }

            var sectionFactor = Device.Idiom == TargetIdiom.Phone ? 1.6 : 1.8;
            var section = new SectionVM(Translations.ON_GOING_READING_TITLE, null);
            section.SizeFactor = sectionFactor;

            foreach (var item in sectionItems)
               section.Folders.Add(new FolderVM(item.FullText) { FirstItem = item, SizeFactor = sectionFactor });

            return section;
         }
         catch (Exception) { throw; }
      }

   }
}
