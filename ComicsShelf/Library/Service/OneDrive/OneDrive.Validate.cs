using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   partial class OneDrive
   {

      public async Task<bool> Validate(Helpers.Database.Library library)
      {
         library.Available = await this.Connector.ConnectAsync();
         return library.Available;
      }

   }
}