using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers;
using ComicsShelf.Store;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async Task<ComicFile[]> SearchFiles(LibraryModel library)
      {
         try
         {
            if (!await this.HasStoragePermission()) { return null; }

            var fileList = await this.FileSystem.GetFiles(new Folder { FullPath = library.Path });
            var result = fileList.Select(file => new ComicFile
            {
               Key = file.FileKey,
               LibraryKey = library.ID,
               FilePath = file.FilePath,
               FolderPath = file.FolderPath.Replace($"{library.Path}{this.FileSystem.PathSeparator}", ""),
               FullText = file.Text,
               SmallText = file.Text.Replace(System.IO.Path.GetFileNameWithoutExtension(file.FolderPath), ""),
               CachePath = $"{Helpers.Constants.FilesCachePath}{file.FileKey}"
            })
            .ToArray();

            return result;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

   }
}
