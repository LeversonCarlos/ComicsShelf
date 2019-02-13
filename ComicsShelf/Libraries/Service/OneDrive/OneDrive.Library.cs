using System;
using System.Linq;
using System.Threading.Tasks;
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

         library.LibraryID = folder.id;
         library.Description = $"{folder.FileName}";
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

         vm.CurrentItem = await this.GetRootDetailsAsync();
         var rootChilds = await this.Connector.GetChildFoldersAsync();
         vm.Data.ReplaceRange(rootChilds);

         var folder = await vm.OpenPage();
         return folder;
      }

      private async Task<FileData> GetRootDetailsAsync()
      {
         try
         {
            var httpPath = $"me/drive/root?select=id,name";

            // REQUEST DATA FROM SERVER
            var httpMessage = await this.Connector.GetAsync(httpPath);
            if (!httpMessage.IsSuccessStatusCode)
            { throw new Exception(await httpMessage.Content.ReadAsStringAsync()); }

            // SERIALIZE AND STORE RESULT
            var httpContent = await httpMessage.Content.ReadAsStringAsync();
            var folder = vTwo.Helpers.FileStream.Deserialize<FileData>(httpContent);
            if (folder == null) { return null; }

            // RESULT
            folder.FilePath = "/";
            return folder;

         }
         catch (Exception) { throw; }
      }

      public async Task<bool> RemoveLibrary(Library library)
      {
         if (App.Settings.Libraries.Where(x=> x.Type == TypeEnum.OneDrive).Count()<=1)
         { await this.Connector.DisconnectAsync(); }
         return true;
      }

   }
}