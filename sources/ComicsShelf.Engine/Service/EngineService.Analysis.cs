using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Engine
{
   partial class EngineService
   {

      public static async Task<bool> AnalyseFiles(ItemVM[] itemList)
      {
         try
         {

            // NOTIFY START MESSAGE
            // Notifyers.Notify.Message(library, R.Search.START_MESSAGE);

            var store = DependencyService.Get<IStoreService>();
            var libraries = store.GetLibraries();

            var libraryItems = libraries
               .Select(x => store.GetLibraryItems(x))
               .SelectMany(x => x)
               .ToList();
            // Notifyers.Notify.Message(library, R.Search.START_MESSAGE);

            var sections = libraryItems
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

            store.SetSections(sections);
            Notifyers.Notify.SectionsUpdate(sections);

            await Task.CompletedTask;
            return true;
         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); return false; }
      }

   }
}
