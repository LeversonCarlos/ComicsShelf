namespace ComicsShelf.UWP
{
   partial class FileSystem
   {

      public void CheckPermissions(System.Action grantedCallback, System.Action revokedCallback)
      {
         grantedCallback?.Invoke();
      }

   }
}