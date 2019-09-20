using ComicsShelf.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<Folder[]> GetFolders(Folder folder)
      {
         try
         {
            var folderChilds = await Task.FromResult(System.IO.Directory.GetDirectories(folder.FullPath));
            var result = folderChilds
               .Select(path => new Folder
               {
                  Name = System.IO.Path.GetFileNameWithoutExtension(path),
                  Key = path,
                  FullPath = path
               })
               .Where(x => !string.IsNullOrEmpty(x.Name))
               .ToArray();
            return result;
         }
         catch  { return new Folder[] { }; }
      }

   }
}