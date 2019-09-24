using System;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<System.Drawing.Size> GetPageSize(string pagePath)
      {
         try
         {
            using (var bitmap = await Android.Graphics.BitmapFactory.DecodeFileAsync(pagePath))
            {
               return new System.Drawing.Size(bitmap.Width, bitmap.Height);
            }
         }
         catch (Exception) { throw; }
         finally { GC.Collect(); }
      }

   }
}