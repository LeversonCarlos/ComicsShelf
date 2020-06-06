using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async override Task<bool> Validate(LibraryModel library)
      {
         try
         {
            if (!await this.Service.CheckConnectionAsync()) { return false; }
            if (string.IsNullOrEmpty(library.Key)) { return false; }
            if (!await Task.FromResult(System.IO.Directory.Exists(library.Key))) { return false; }
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
