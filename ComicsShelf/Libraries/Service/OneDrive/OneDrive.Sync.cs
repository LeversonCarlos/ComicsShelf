using System;
using System.Threading.Tasks;

namespace ComicsShelf.Libraries.Implementation
{
   partial class OneDrive
   {

      public async Task<bool> SaveDataAsync(Library library, byte[] serializedValue)
      {
         try
         {
            // TODO
            return true;
         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("OneDrive.SaveDataAsync", ex); return false; }
      }

      public async Task<byte[]> LoadDataAsync(Library library)
      {
         try
         {
            // TODO
            return null;
         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("OneDrive.LoadDataAsync", ex); return null; }
      }

   }
}