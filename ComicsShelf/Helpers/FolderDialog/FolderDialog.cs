using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers.FolderDialog
{
   internal class FolderDialog
   {

      public static async Task<FolderData> GetFolder(FolderData initialFolder, Func<FolderData, Task<List<FolderData>>> getFolderChilds)
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
               vm.Data.ReplaceRange(folderChilds);
               vm.CurrentItem = item;
               vm.IsBusy = false;
            };

            // LOCATE ROOT CHILDS
            var rootChilds = await getFolderChilds(initialFolder);
            vm.Data.ReplaceRange(rootChilds);
            vm.CurrentItem = initialFolder;

            // SHOW DIALOG
            var folder = await vm.OpenPage();
            return folder;

         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

   }
}
