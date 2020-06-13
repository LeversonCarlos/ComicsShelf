using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;

namespace ComicsShelf.Drive.FolderDialog
{
   internal class Selector
   {

      public static Task<DirectoryVM> GetFolder(ICloudDriveService driveService) => GetFolder(driveService, new DirectoryVM { });

      public static async Task<DirectoryVM> GetFolder(ICloudDriveService driveService, DirectoryVM initialFolder)
      {
         try
         {

            FolderVM initialItem = null;
            if (initialFolder != null)
            {
               initialItem = new FolderVM
               {
                  ID = initialFolder.ID,
                  Path = initialFolder.Path,
                  Name = initialFolder.Name
               };
            }

            var selectedItem = await GetFolder(driveService, initialItem);

            DirectoryVM selectedFolder = null;
            if (selectedItem != null)
            {
               selectedFolder = new DirectoryVM
               {
                  ID = selectedItem.ID,
                  Name = selectedItem.Name,
                  Path = selectedItem.Path
               };
            }

            return selectedFolder;
         }
         catch (Exception ex)
         {
            ComicsShelf.Base.Helpers.AppInsights.TrackException(ex);
            return null;
         }
      }

      public static async Task<FolderVM> GetFolder(ICloudDriveService driveService, FolderVM initialFolder)
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
                  catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); }
                  finally { vm.IsBusy = false; }
               };

               // SHOW DIALOG
               var selectedFolder = await vm.OpenPage();

               // RESULT
               return selectedFolder;

            }

         }
         catch (Exception ex)
         {
            Base.Helpers.AppInsights.TrackException(ex);
            return null;
         }
      }

      static async Task<FolderVM[]> GetFolder_Children(ICloudDriveService driveService, FolderVM item)
      {
         if (item == null || string.IsNullOrEmpty(item.ID))
         { return await GetFolder_Children_Drivers(driveService); }
         else if (item.Type == enFolderType.Drive || item.Type == enFolderType.Folder)
         { return await GetFolder_Children_Directories(driveService, item); }
         else { return new FolderVM[] { }; }
      }

      static async Task<FolderVM[]> GetFolder_Children_Drivers(ICloudDriveService driveService)
      {
         try
         {
            var driveList = await driveService.GetDrivers();
            var resultList = driveList
               .Select(x => new FolderVM
               {
                  ID = x.ID,
                  Name = x.Name,
                  Path = x.Path,
                  Type = enFolderType.Drive
               })
               .ToArray();
            return resultList;
         }
         catch (Exception ex)
         {
            Base.Helpers.AppInsights.TrackException(ex);
            return new FolderVM[] { };
         }
      }

      static async Task<FolderVM[]> GetFolder_Children_Directories(ICloudDriveService driveService, FolderVM item)
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
               .Select(x => new FolderVM
               {
                  ID = x.ID,
                  Name = x.Name,
                  Path = x.Path,
                  Parent = item,
                  Type = enFolderType.Folder
               })
               .ToList();
            resultList.Insert(0, new FolderVM { Name = "..", ID = item.Parent?.ID, Path = item.Parent?.Path, Parent = item?.Parent?.Parent });

            return resultList.ToArray();
         }
         catch (Exception ex)
         {
            Base.Helpers.AppInsights.TrackException(ex);
            return new FolderVM[] { };
         }
      }


   }
}
