using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers.FolderDialog
{
   internal class Selector
   {

      public static async Task<Folder> GetFolder(Folder initialFolder, Func<Folder, Task<Folder[]>> getFolderChilds)
      {
         try
         {

            // INITIATE THE VIEW MODEL 
            var vm = new FolderDialogVM();

            // HOOK UP EVENT FOR ITEM SELECTION
            vm.OnItemSelected += async (sender, item) =>
            {
               vm.IsBusy = true;
               var folderChilds = await getFolderChilds(item);
               vm.Data.Clear();
               vm.Data.Add(new Folder { Name = "..", Path = ((Folder)item).Path });
               foreach (var folderChild in folderChilds) { vm.Data.Add(folderChild); }
               vm.CurrentItem = item;
               vm.IsBusy = false;
            };

            // LOCATE ROOT CHILDS
            var rootChilds = await getFolderChilds(initialFolder);
            foreach (var rootChild in rootChilds) { vm.Data.Add(rootChild); }
            vm.CurrentItem = initialFolder;

            // SHOW DIALOG
            var folder = await vm.OpenPage();
            return folder;

         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

   }
}
