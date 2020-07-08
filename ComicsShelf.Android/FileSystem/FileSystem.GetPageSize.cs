using System;
using System.Drawing;
using System.Threading.Tasks;

namespace ComicsShelfStore.Droid
{
   partial class FileSystem
   {

      public async Task<Size> GetPageSize(string pagePath)
      {
         try
         {
            using (var bitmap = await Android.Graphics.BitmapFactory.DecodeFileAsync(pagePath))
            {
               return new Size(bitmap.Width, bitmap.Height);
            }
         }
         catch (Exception ex) { throw new Exception($"Error identifying the image size for file. {ex.Message}.", new Exception(pagePath)); }
      }

   }
}