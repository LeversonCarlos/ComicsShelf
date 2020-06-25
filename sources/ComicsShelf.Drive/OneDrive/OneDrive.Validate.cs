using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class OneDrive
   {

      public override async Task<bool> ValidateLibrary(LibraryVM library)
      {
         try
         {
            if (!await this.CloudService.CheckConnectionAsync()) { return false; }
            return true;
         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); return false; }
      }

   }
}
