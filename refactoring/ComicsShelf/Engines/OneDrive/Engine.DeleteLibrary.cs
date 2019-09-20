using ComicsShelf.Store;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async Task<bool> DeleteLibrary(LibraryModel library)
      {
         try
         {

            var store = Xamarin.Forms.DependencyService.Get<Store.ILibraryStore>();
            if (!store.Libraries.Any(x => x.Type == LibraryType.OneDrive && x.ID != library.ID))
            {
               await this.Connector.DisconnectAsync();
            }

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
