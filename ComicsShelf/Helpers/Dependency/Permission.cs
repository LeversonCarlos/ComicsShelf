using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers
{
   internal class Permissions
   {

      public static async Task<bool> HasStoragePermission()
      {
         try
         {  
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            if (permissionStatus != PermissionStatus.Granted)
            {
               if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
               { await App.ShowMessage("Will need storage permission to search for comic files"); }

               var requestPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
               if (requestPermissions.ContainsKey(Permission.Storage))
               { permissionStatus = requestPermissions[Permission.Storage]; }
            }

            return (permissionStatus == PermissionStatus.Granted);

         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
