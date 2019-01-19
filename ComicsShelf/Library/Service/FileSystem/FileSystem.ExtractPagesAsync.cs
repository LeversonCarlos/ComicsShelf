using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   partial class FileSystemService
   {

      public async Task ExtractPagesAsync(vTwo.Libraries.Library library, Views.File.FileData fileData)
      {
         await this.FileSystem.PagesExtract(fileData);
      }

   }
}