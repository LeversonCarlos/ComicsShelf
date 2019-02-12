using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.OneDrive.Files;
using Xamarin.OneDrive.Profile;

namespace ComicsShelf.Libraries.Implementation
{
   partial class OneDrive
   {

      public async Task<bool> Validate(Library library)
      {
         try
         {
            library.Available = await this.Connector.ConnectAsync();
         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("OneDrive.Validate", ex); library.Available = false; }
         return library.Available;
      }

      public async Task<bool> AddLibrary(Library library)
      {
         if (!await this.Connector.ConnectAsync()) { return false; }
         var profile = await this.Connector.GetProfileAsync();
         if (profile == null) { return false; }

         var folder = await this.AddLibrary_FolderSelect(library);
         if (folder == null) { return false; }
         library.SetKeyValue("MainFolderID", folder.id);

         library.LibraryID = profile.id;
         library.Description = profile.Name;
         library.Available = true;
         return true;
      }

      private async Task<FileData> AddLibrary_FolderSelect(Library library)
      {
         var vm = new Libraries.Service.OneDrive.FolderSelector.SelectorVM();
         vm.OnItemSelected += async (sender, item) =>
         {
            vm.IsBusy = true;
            var folderChilds = await this.Connector.GetChildFoldersAsync(item);
            vm.Data.ReplaceRange(folderChilds);
            vm.CurrentItem = item;
            vm.IsBusy = false;
         };

         var rootChilds = await this.Connector.GetChildFoldersAsync();
         vm.Data.ReplaceRange(rootChilds);

         var folder = await vm.OpenPage();
         return folder;
      }

      public async Task<bool> RemoveLibrary(Library library)
      {
         return await this.Connector.DisconnectAsync();
      }

   }
}