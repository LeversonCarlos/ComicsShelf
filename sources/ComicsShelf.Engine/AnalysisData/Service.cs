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
         try
         {
            var store = DependencyService.Get<IStoreService>();
            var sections = new List<SectionVM>();

            // RETRIEVE ALL LIBRARY ITEMS
            var libraries = store.GetLibraries();
            /*
            var libraryItems = libraries
               .Select(x => store.GetLibraryItems(x))
               .SelectMany(x => x)
               .ToList();
            */

            // LOOP THROUGH LIBRARIES
            foreach (var library in libraries)
            {

               Helpers.Notify.Message(library, Strings.START_MESSAGE);

               // LOCATE LIBRARY ITEMS
               var libraryItems = store
                  .GetLibraryItems(library)
                  .ToList();
               // Notifyers.Notify.Message(library, Strings.START_MESSAGE);

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

               Helpers.Notify.Message(library, "");

            }

            var sectionsArray = sections.ToArray();
            store.SetSections(sectionsArray);
            Helpers.Notify.SectionsUpdate(sectionsArray);

            await Task.CompletedTask;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

   }
}
