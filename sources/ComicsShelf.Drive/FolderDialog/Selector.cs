using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;

namespace ComicsShelf.Drive.FolderDialog
{
   internal class Selector
   {

      public static Task<DriveItemVM> GetFolder(ICloudDriveService driveService) => GetFolder(driveService, null);

      public static async Task<DriveItemVM> GetFolder(ICloudDriveService driveService, DriveItemVM initialFolder)
      {
         try
         {

            // INITIATE THE VIEW MODEL 
            using (var vm = new FolderDialogVM())
            {

               // LOCATE ROOT CHILDS
               var rootChilds = await GetFolder_Children(driveService, initialFolder);
               vm.Data.AddRange(rootChilds);
               vm.CurrentItem = null;

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
                  catch (Exception ex) { Helpers.Insights.TrackException(ex); }
                  finally { vm.IsBusy = false; }
               };

               // SHOW DIALOG
               var selectedFolder = await vm.OpenPage();

               // RESULT
               return selectedFolder;

            }

         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); return null; }
      }

      static async Task<DriveItemVM[]> GetFolder_Children(ICloudDriveService driveService, DriveItemVM item)
      {
         if (item == null || string.IsNullOrEmpty(item.ID))
         { return await GetFolder_Children_Drives(driveService); }
         else if (item.Type == enItemType.Drive || item.Type == enItemType.Folder)
         { return await GetFolder_Children_Directories(driveService, item); }
         else { return new DriveItemVM[] { }; }
      }

      static async Task<DriveItemVM[]> GetFolder_Children_Drives(ICloudDriveService driveService)
      {
         try
         {
            var driveList = await driveService.GetDrivers();
            var resultList = driveList
               .Select(x => new DriveItemVM
               {
                  ID = x.ID,
                  Name = x.Name,
                  Path = x.Path,
                  Type = enItemType.Drive
               })
               .ToArray();
            return resultList;
         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); return new DriveItemVM[] { }; }
      }

      static async Task<DriveItemVM[]> GetFolder_Children_Directories(ICloudDriveService driveService, DriveItemVM item)
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
               .Select(x => new DriveItemVM
               {
                  ID = x.ID,
                  Name = x.Name,
                  Path = x.Path,
                  Parent = item,
                  Type = enItemType.Folder
               })
               .ToList();
            resultList.Insert(0, new DriveItemVM { Name = "..", ID = item.Parent?.ID, Path = item.Parent?.Path, Parent = item?.Parent?.Parent });

            return resultList.ToArray();
         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); return new DriveItemVM[] { }; }
      }

   }
}
