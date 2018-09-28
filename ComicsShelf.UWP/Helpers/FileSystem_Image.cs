using System;
using System.IO;
using System.Threading.Tasks;

namespace ComicsShelf.UWP
{
   partial class FileSystem
   {

      // private static object saveFileLocker = new object();
      public async Task Thumbnail(Stream imageStream, string imagePath)
      {
         try
         {
            // lock (saveFileLocker)
            {
               FileStream thumbnailFile = null;
               thumbnailFile = System.IO.File.Open(imagePath, FileMode.OpenOrCreate);
               await imageStream.CopyToAsync(thumbnailFile);
               await thumbnailFile.FlushAsync();
               thumbnailFile.Dispose();
            }
         }
         catch (Exception ex) { throw; }
      }

   }
}