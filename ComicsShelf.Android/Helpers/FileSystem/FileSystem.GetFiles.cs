using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<ComicFile[]> GetFiles(Folder folder)
      {
         try
         {

            System.IO.Directory
               .EnumerateFiles(folder.Path, "*.cbz.zip", System.IO.SearchOption.AllDirectories)
               .ToList()
               .ForEach(file => {
                  System.IO.File.Move(file, file.Replace(".cbz.zip", ".cbz"));
               });

            var fileListQuery = System.IO.Directory
               .EnumerateFiles(folder.Path, "*.cbz", System.IO.SearchOption.AllDirectories)
               .AsQueryable();
            var fileList = await Task.FromResult(fileListQuery.ToList());

            var fileResult = fileList
               .Select(filePath => new ComicFile
               {
                  FullPath = filePath,
                  ParentPath = System.IO.Path.GetDirectoryName(filePath),
                  FullText = System.IO.Path.GetFileNameWithoutExtension(filePath).Trim()
               })
               .Select(file => new ComicFile
               {
                  Key = file.FullPath
                     .Replace("#", "")
                     .Replace(".", "")
                     .Replace("[", "")
                     .Replace("]", "")
                     .Replace("(", "")
                     .Replace(")", "")
                     .Replace(" ", "")
                     .Replace(this.PathSeparator, "_")
                     .Replace("___", "_")
                     .Replace("__", "_"),
                  FullPath = file.FullPath,
                  ParentPath = file.ParentPath,
                  FullText = file.FullText,
                  SmallText = file.FullText
                     .Replace(System.IO.Path.GetFileNameWithoutExtension(file.ParentPath), "")
               })
               .ToArray();

            return fileResult;

         }
         catch (Exception) { throw; }
      }

   }
}