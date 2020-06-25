using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class LocalDrive
   {

      public override async Task<bool> ValidateLibrary(LibraryVM library)
      {
         try
         {
            if (!await this.CloudService.CheckConnectionAsync()) { return false; }
            if (string.IsNullOrEmpty(library.ID)) { return false; }
            if (!await Task.FromResult(System.IO.Directory.Exists(library.ID))) { return false; }
            return true;
         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); return false; }
      }

   }
}
