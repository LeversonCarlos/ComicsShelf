using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   public interface IFileSystem
   {
      Task<bool> SaveThumbnail(System.IO.Stream imageStream, string imagePath);
      Task<System.Drawing.Size> GetPageSize(string pagePath);
   }
}
