using ComicsShelf.Helpers;
using ComicsShelf.Helpers.FolderDialog;
using ComicsShelf.Libraries;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async Task<File[]> SearchFiles(Folder folder)
      {
         try
         {
            if (!await this.HasStoragePermission()) { return null; }
            var fileList = await this.FileSystem.GetFiles(folder);
            return fileList;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

   }
}
