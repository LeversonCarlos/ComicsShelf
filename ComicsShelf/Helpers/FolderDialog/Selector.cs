using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;

namespace ComicsShelf.Helpers.FolderDialog
{

   internal class Selector
   {

      public static async Task<Folder> GetFolder(ICloudDriveService driveService, Folder initialFolder)
      {
         try
         {

            SelectorItem initialItem = null;
            if (initialFolder != null)
            {
               initialItem = new SelectorItem
               {
                  ID = initialFolder.Key,
                  Path = initialFolder.FullPath,
                  Name = initialFolder.Name
               };
            }

            var selectedItem = await GetFolder(driveService, initialItem);

            Folder selectedFolder = null;
            if (selectedItem != null)
            {
               selectedFolder = new Folder
               {
                  Key = selectedItem.ID,
                  Name = selectedItem.Name,
                  FullPath = selectedItem.Path
               };
            }

            return selectedFolder;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

      public static async Task<SelectorItem> GetFolder(ICloudDriveService driveService, SelectorItem initialFolder)
      {
         try
         {

            // INITIATE THE VIEW MODEL 
            var vm = new FolderDialogVM();

            // LOCATE ROOT CHILDS
            var rootChilds = await GetFolder_Children(driveService, initialFolder);
            vm.Data.AddRange(rootChilds);
            vm.CurrentItem = null;

            /*
               var folderList = folderChilds
                  .Select(x => new Helpers.Folder
                  {
                     Key = x.id,
                     FullPath = x.FilePath
                        .Trim()
                        .Replace("/ / ", "/")
                        .Replace("/ /", "/")
                        .Replace("// ", "/")
                        .Replace("//", "/"),
                     Name = x.FileName
                  })
                  .ToArray();
             */

            // HOOK UP EVENT FOR ITEM SELECTION
            vm.OnItemSelected += async (sender, item) =>
            {
               try
               {
                  vm.IsBusy = true;
                  var folderChilds = await GetFolder_Children(driveService, item);
                  vm.Data.ReplaceRange(folderChilds);
                  vm.CurrentItem = item;
               }
               catch (Exception ex) { await App.ShowMessage(ex); }
               finally { vm.IsBusy = false; }
            };

            // SHOW DIALOG
            var selectedFolder = await vm.OpenPage();

            // RESULT
            return selectedFolder;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

      static async Task<SelectorItem[]> GetFolder_Children(ICloudDriveService driveService, SelectorItem item)
      {
         if (item == null || string.IsNullOrEmpty(item.ID))
         { return await GetFolder_Children_Drivers(driveService); }
         else if (item.Type == enSelectorItemType.Drive || item.Type == enSelectorItemType.Folder)
         { return await GetFolder_Children_Directories(driveService, item); }
         else { return new SelectorItem[] { }; }
      }

      static async Task<SelectorItem[]> GetFolder_Children_Drivers(ICloudDriveService driveService)
      {
         var driveList = await driveService.GetDrivers();
         var resultList = driveList
            .Select(x => new SelectorItem
            {
               ID = x.ID,
               Name = x.Name,
               Path = x.Path,
               Type = enSelectorItemType.Drive
            })
            .ToArray();
         return resultList;
      }

      static async Task<SelectorItem[]> GetFolder_Children_Directories(ICloudDriveService driveService, SelectorItem item)
      {

         var folder = new DirectoryVM
         {
            ID = item.ID,
            Name = item.Name,
            Path = item.Path,
            ParentID = item.Parent?.ID
         };
         var directoriesList = await driveService.GetDirectories(folder);

         var resultList = directoriesList
            .Select(x => new SelectorItem
            {
               ID = x.ID,
               Name = x.Name,
               Path = x.Path,
               Parent = item,
               Type = enSelectorItemType.Folder
            })
            .ToList();
         resultList.Insert(0, new SelectorItem { Name = "..", ID = item.Parent?.ID, Path = item.Parent?.Path, Parent = item?.Parent?.Parent });

         return resultList.ToArray();
      }

   }

   internal class SelectorItem
   {
      public string ID { get; set; }
      public string Name { get; set; }
      public string Path { get; set; }
      public SelectorItem Parent { get; set; }
      public enSelectorItemType Type { get; set; }
   }

   internal enum enSelectorItemType : short { Drive, Folder, File }

}
