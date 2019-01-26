using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   partial class FileSystemService
   {

      public async Task ExtractPagesAsync(Library library, Views.File.FileData fileData)
      {
         await this.FileSystem.PagesExtract(fileData);
      }

   }
}