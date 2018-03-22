using System.Threading.Tasks;

namespace ComicsShelf.Helpers
{
   public interface iFileSystem
   {
      string PathSeparator { get; }
      Task<string> GetComicsPath(string comicsPath);
   }
}