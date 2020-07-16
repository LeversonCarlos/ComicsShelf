using ComicsShelf.Screens.FolderDialog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector;

namespace ComicsShelf.FolderSelector
{
   internal class Selector
   {

      public static Task<SelectorItemVM> GetFolder(ICloudDriveService driveService) => GetFolder(driveService, null);

      public static async Task<SelectorItemVM> GetFolder(ICloudDriveService driveService, SelectorItemVM initialFolder)
      {
         try
         {

            // INITIATE THE VIEW MODEL 
            using (var vm = new FolderDialogVM())
            {

               // LOCATE ROOT CHILDS
               var rootChilds = await GetFolder_Children(driveService, initialFolder);
               await vm.Data.AddRangeAsync(rootChilds);
               vm.CurrentItem = null;

               // HOOK UP EVENT FOR ITEM SELECTION
               vm.OnItemSelected += async (sender, item) =>
               {
                  try
                  {
                     vm.IsBusy = true;
                     var folderChilds = await GetFolder_Children(driveService, item);
                     await vm.Data.ReplaceRangeAsync(folderChilds);
                     vm.CurrentItem = item;
                  }
                  catch (Exception ex) { Helpers.Insights.TrackException(ex); }
                  finally { vm.IsBusy = false; }
               };

               // SHOW DIALOG
               var selectedFolder = await vm.OpenPage();

               // RESULT
               return selectedFolder;

            }

         }
         catch (Exception ex) { Helpers.Message.Show(ex); return null; }
      }

      static async Task<SelectorItemVM[]> GetFolder_Children(ICloudDriveService driveService, SelectorItemVM item)
      {
         if (item == null || string.IsNullOrEmpty(item.ID))
         { return await GetFolder_Children_Drives(driveService); }
         else if (item.Type == enSelectorItemType.Drive || item.Type == enSelectorItemType.Folder)
         { return await GetFolder_Children_Directories(driveService, item); }
         else { return new SelectorItemVM[] { }; }
      }

      static async Task<SelectorItemVM[]> GetFolder_Children_Drives(ICloudDriveService driveService)
      {
         try
         {
            var driveList = await driveService.GetDrivers();
            var resultList = driveList
               .Select(x => new SelectorItemVM
               {
                  ID = x.ID,
                  Name = x.Name,
                  Path = x.Path,
                  Type = enSelectorItemType.Drive
               })
               .ToArray();
            return resultList;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return new SelectorItemVM[] { }; }
      }

      static async Task<SelectorItemVM[]> GetFolder_Children_Directories(ICloudDriveService driveService, SelectorItemVM item)
      {
         try
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
               .Select(x => new SelectorItemVM
               {
                  ID = x.ID,
                  Name = x.Name,
                  Path = x.Path,
                  Parent = item,
                  Type = enSelectorItemType.Folder
               })
               .ToList();
            resultList.Insert(0, new SelectorItemVM { Name = "..", ID = item.Parent?.ID, Path = item.Parent?.Path, Parent = item?.Parent?.Parent });

            return resultList.ToArray();
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return new SelectorItemVM[] { }; }
      }

   }
}
