using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async Task<ComicFile[]> SearchFiles(Libraries.LibraryModel library)
      {
         try
         {
            if (!await this.HasStoragePermission()) { return null; }

            var fileList = await this.FileSystem.GetFiles(new Folder { Path = library.LibraryKey });
            var result = fileList.Select(file => new ComicFile
            {
               Key = file.FileKey,
               FilePath = file.FilePath,
               FolderPath = file.FolderPath,
               FullText = file.Text,
               SmallText = file.Text.Replace(System.IO.Path.GetFileNameWithoutExtension(file.FolderPath), "")
            })
            .ToArray();

            return result;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

   }
}
