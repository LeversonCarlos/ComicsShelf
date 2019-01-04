using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   internal partial class OneDrive : ILibraryService
   {

      Xamarin.OneDrive.Connector Connector { get; set; }
      public OneDrive()
      {
         var clientID = System.Environment.GetEnvironmentVariable("ComicsShelfApplicationID");
         this.Connector = new Xamarin.OneDrive.Connector(clientID, "User.Read", "Files.Read");
      }

      public async Task ExtractPagesAsync(Helpers.Database.Library library, Views.File.FileData fileData)
      {
      }

   }
}