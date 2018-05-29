using Android.OS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComicsShelf.Droid
{
   internal class Permission 
   {

      public static async void Validate(string[] permissionList, Action grantedCallback, Action revokedCallback)
      {

         // CHECK SDK VERSION 
         if ((int)Build.VERSION.SdkInt < 23) { grantedCallback?.Invoke(); return; }

         // GET CURRENT ACTIVITY
         var currentActivity = (MainActivity)Xamarin.Forms.Forms.Context;
         const int currentRequestCode = 0;

         // LOAD CURRENT PERMISSION SET
         var permissionGranted = new Dictionary<string, bool>();
         foreach (var permission in permissionList)
         {
            var granted = (currentActivity.CheckSelfPermission(permission) == Android.Content.PM.Permission.Granted);
            permissionGranted.Add(permission, granted);
         }

         // CHECK FOR PENDING PERMISSIONS
         var permissionPending = permissionGranted
            .Where(x => x.Value == false)
            .Select(x => x.Key)
            .ToArray();
         if (permissionPending.Count() == 0) { grantedCallback?.Invoke(); return; }

         // DEFINE RESULT CALLBACK
         MainActivity.PermissionsResultHandler onPermissionResult = null;
         onPermissionResult = new MainActivity.PermissionsResultHandler((requestCode, permissions, grantResults) => {
            currentActivity.OnPermissionsResult -= onPermissionResult;
            switch (requestCode)
            {
               case currentRequestCode:
                  {
                     var grantResultsPending = grantResults
                        .Where(x => x == Android.Content.PM.Permission.Denied)
                        .Count();
                     if (grantResultsPending == 0) { grantedCallback?.Invoke(); }
                     else { revokedCallback?.Invoke(); }
                  }
                  break;
            }
         });
         currentActivity.OnPermissionsResult += onPermissionResult;

         // REQUEST PERMISSION
         currentActivity.RequestPermissions(permissionPending, currentRequestCode);
      }

   }
}