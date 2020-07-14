using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T>
   {

      public async Task<LibraryVM> AddLibrary()
      {
         try
         {

            if (!await this.CloudService.ConnectAsync()) { return null; }

            var driveItem = await FolderSelector.Selector.GetFolder(this.CloudService);
            if (driveItem == null) { return null; }

            var library = new LibraryVM
            {
               ID = driveItem.ID,
               EscapedID = this.EscapeFileID(driveItem.ID),
               Description = driveItem.Name,
               Path = driveItem.Path,
               KeyValues = driveItem.KeyValues,
               Type = this.LibraryType
            };

            return library;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return null; }
      }

   }
}
