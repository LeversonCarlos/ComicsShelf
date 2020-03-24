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

            string[] folderChilds = null;
            if (folder.FullPath == "/")
            {
               folderChilds = await Task.FromResult(ExternalStorage.GetExternalStorages());
               var i = 0;
               Helpers.AppCenter.TrackEvent("External Storages", folderChilds.Select(x => $"Storage {++i}:{x}").ToArray());
            }
            else { folderChilds = await Task.FromResult(System.IO.Directory.GetDirectories(folder.FullPath)); }

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
         catch { return new Folder[] { }; }
      }

   }
}