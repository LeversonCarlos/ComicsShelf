using System.Threading.Tasks;

namespace ComicsShelf.Interfaces
{
   public interface IFileSystem
   {
      Task<bool> SaveThumbnail(System.IO.Stream imageStream, string imagePath);
      Task<System.Drawing.Size> GetPageSize(string pagePath);
   }
}
