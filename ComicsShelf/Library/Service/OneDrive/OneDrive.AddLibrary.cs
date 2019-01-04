using System.Threading.Tasks;
using Xamarin.OneDrive.Profile;

namespace ComicsShelf.Library.Implementation
{
   partial class OneDrive
   {

      public async Task<bool> AddLibrary(Helpers.Database.Library library)
      {
         if (!await this.Connector.ConnectAsync()) { return false; }
         var profile = await this.Connector.GetProfileAsync();
         if (profile == null) { return false; }
         library.LibraryPath = profile.id;
         library.LibraryText = profile.Name;
         library.Available = true;
         return true;
      }

   }
}