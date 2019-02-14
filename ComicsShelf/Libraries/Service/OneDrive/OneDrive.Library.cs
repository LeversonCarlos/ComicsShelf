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

         var folder = await Service.OneDrive.FolderSelector.Selector.FolderSelect(this.Connector, library);
         if (folder == null) { return false; }

         library.LibraryID = folder.id;
         library.Description = $"{folder.FileName}";
         library.Available = true;
         return true;
      }

      public async Task<bool> RemoveLibrary(Library library)
      {
         if (App.Settings.Libraries.Where(x=> x.Type == TypeEnum.OneDrive).Count()<=1)
         { await this.Connector.DisconnectAsync(); }
         return true;
      }

   }
}