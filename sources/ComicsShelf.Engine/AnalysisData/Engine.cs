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
            var sections = new List<FolderVM>();

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

               Notifyers.Notify.Message(library, Strings.START_MESSAGE);

               var libraryItems = store
                  .GetLibraryItems(library)
                  .ToList();
               // Notifyers.Notify.Message(library, Strings.START_MESSAGE);

               var librarySections = libraryItems
                  .GroupBy(section => new { Path = section.SectionPath })
                  .Select(section => new FolderVM(section.Key.Path)
                  {
                     Folders = libraryItems
                        .Where(folder => folder.SectionPath == section.Key.Path)
                        .GroupBy(folder => new { Path = folder.FolderPath })
                        .Select(folder => new FolderVM(folder.Key.Path)
                        {
                           Items = libraryItems
                              .Where(item => item.FolderPath == folder.Key.Path)
                              .OrderBy(item => item.FullText)
                              .ToArray()
                        })
                        .ToArray()
                  })
                  .ToArray();
               sections.AddRange(librarySections);

               Notifyers.Notify.Message(library, "");

            }

            var sectionsArray = sections.ToArray();
            store.SetSections(sectionsArray);
            Notifyers.Notify.SectionsUpdate(sectionsArray);

            await Task.CompletedTask;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

   }
}
