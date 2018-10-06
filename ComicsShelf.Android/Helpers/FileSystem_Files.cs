﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<string[]> GetFiles(string path)
      {
         try
         {
            var fileList = System.IO.Directory
               .EnumerateFiles(path, "*.*", System.IO.SearchOption.AllDirectories)
               .Where(x =>
                  x.EndsWith(".cbz", StringComparison.OrdinalIgnoreCase) ||
                  x.EndsWith(".cbr", StringComparison.OrdinalIgnoreCase))
               .ToList();

            var folderPath = $"{path}{this.PathSeparator}";
            var fileListRenamed = fileList
               .Select(x => x.Replace(folderPath, ""))
               .ToList();

            var fileArray = fileListRenamed.ToArray();
            return fileArray;
         }
         catch (Exception ex) { throw; }
      }

   }
}