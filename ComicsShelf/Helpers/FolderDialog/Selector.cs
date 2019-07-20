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

            // LOCATE ROOT CHILDS
            var rootChilds = await getFolderChilds(initialFolder);
            vm.Data.AddRange(rootChilds);
            vm.CurrentItem = initialFolder;

            // HOOK UP EVENT FOR ITEM SELECTION
            vm.OnItemSelected += async (sender, item) =>
            {
               try
               {
                  vm.IsBusy = true;
                  var folderChilds = await getFolderChilds(item);
                  vm.Data.ReplaceRange(folderChilds);
                  if (((Folder)item).FullPath != initialFolder.FullPath)
                  {
                     var parent = new Folder { Name = "..", Key = vm.CurrentItem.Key, FullPath = vm.CurrentItem.FullPath };
                     vm.Data.Insert(0, parent);
                  }
                  vm.CurrentItem = item;
               }
               catch { }
               finally { vm.IsBusy = false; }
            };

            // SHOW DIALOG
            var folder = await vm.OpenPage();
            return folder;

         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

   }
}
