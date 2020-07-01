using ComicsShelf.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T>
   {

      public virtual Task<bool> ExtractPages(LibraryVM library, ItemVM libraryItem) => throw new NotImplementedException();

      protected async Task ExtractPage(Stream zipEntryStream, string pagePath)
      {
         try
         {
            using (var pageStream = new FileStream(pagePath, FileMode.CreateNew, FileAccess.Write))
            {
               await zipEntryStream.CopyToAsync(pageStream);
               await pageStream.FlushAsync();
               pageStream.Close();
            }
         }
         catch (Exception) { throw; }
      }

   }
}
