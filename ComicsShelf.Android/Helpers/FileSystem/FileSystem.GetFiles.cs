using ComicsShelf.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<File[]> GetFiles(Folder folder)
      {
         try
         {

            /*
            System.IO.Directory
               .EnumerateFiles(folder.Path, "*.cbz.zip", System.IO.SearchOption.AllDirectories)
               .ToList()
               .ForEach(file => {
                  System.IO.File.Move(file, file.Replace(".cbz.zip", ".cbz"));
               });
            */

            var fileListQuery = System.IO.Directory
               .EnumerateFiles(folder.Path, "*.cbz", System.IO.SearchOption.AllDirectories)
               .AsQueryable();
            var fileList = await Task.FromResult(fileListQuery.ToList());

            var fileResult = fileList
               .Select(filePath => new File
               {
                  FileKey = this.GetKey(filePath),
                  FilePath = filePath,
                  FolderPath = System.IO.Path.GetDirectoryName(filePath),
                  Text = System.IO.Path.GetFileNameWithoutExtension(filePath).Trim()
               })
               .ToArray();

            return fileResult;

         }
         catch (Exception) { throw; }
      }

   }
}