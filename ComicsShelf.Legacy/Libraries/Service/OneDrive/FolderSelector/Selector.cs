using System;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Libraries.Service.OneDrive.FolderSelector
{
   internal class Selector
    {

      public static async Task<FileData> FolderSelect(Xamarin.OneDrive.Connector connector, Library library)
      {
         var vm = new Libraries.Service.OneDrive.FolderSelector.SelectorVM();
         vm.OnItemSelected += async (sender, item) =>
         {
            vm.IsBusy = true;
            var folderChilds = await connector.GetChildFoldersAsync(item);
            vm.Data.ReplaceRange(folderChilds);
            vm.CurrentItem = item;
            vm.IsBusy = false;
         };

         vm.CurrentItem = await FolderSelect_GetRootDetailsAsync(connector);
         var rootChilds = await connector.GetChildFoldersAsync();
         vm.Data.ReplaceRange(rootChilds);

         var folder = await vm.OpenPage();
         return folder;
      }

      private static async Task<FileData> FolderSelect_GetRootDetailsAsync(Xamarin.OneDrive.Connector connector)
      {
         try
         {
            var httpPath = $"me/drive/root?select=id,name";

            // REQUEST DATA FROM SERVER
            var httpMessage = await connector.GetAsync(httpPath);
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

   }
}
