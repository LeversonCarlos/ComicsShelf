using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async override Task<bool> Validate(LibraryModel library)
      {
         try
         {
            if (!await this.Service.CheckConnectionAsync()) { return false; }
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
