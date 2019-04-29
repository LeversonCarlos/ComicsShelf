using ComicsShelf.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<File[]> GetFiles(Libraries.LibraryModel library)
      {
         try
         {

            var folderPath = $"{library.Key}";
            var fileListQuery = System.IO.Directory
               .EnumerateFiles(folderPath, "*.*", System.IO.SearchOption.AllDirectories)
               .Where(x =>
                  x.EndsWith(".cbz", StringComparison.OrdinalIgnoreCase)
               )
               .AsQueryable();
            var fileList = await Task.FromResult(fileListQuery.ToList());

            var fileResult = fileList
               .Select(filePath => new File
               {
                  FullPath = filePath,
                  ParentPath = System.IO.Path.GetDirectoryName(filePath),
                  FullText = System.IO.Path.GetFileNameWithoutExtension(filePath).Trim()
               })
               .Select(file => new File
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