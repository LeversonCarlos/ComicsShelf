﻿using ComicsShelf.Helpers;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<Folder> GetRootPath()
      {

         var result = new Folder
         {
            Name = "/",
            Key = "/",
            FullPath = "/"
         };
         /*
         var absolutePath = await Task.FromResult(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);
         var result = new Folder
         {
            Name = System.IO.Path.GetFileNameWithoutExtension(absolutePath),
            Key = absolutePath,
            FullPath = absolutePath
         };
         */
         await Task.CompletedTask;
         return result;
      }

   }
}