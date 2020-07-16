using System;
using System.IO;
using System.Threading.Tasks;

namespace ComicsShelfStore.Droid
{
   partial class FileSystem
   {

      public async Task<bool> SaveThumbnail(Stream imageStream, string imagePath)
      {
         try
         {

            // LOAD IMAGE
            using (var originalBitmap = await Android.Graphics.BitmapFactory.DecodeStreamAsync(imageStream))
            {
               if (originalBitmap == null) { return false; }

               // DEFINE SIZE
               double imageHeight = 300; double imageWidth = 100;
               double scaleFactor = (double)imageHeight / (double)originalBitmap.Height;
               imageHeight = originalBitmap.Height * scaleFactor;
               imageWidth = originalBitmap.Width * scaleFactor;

               // INITIALIZE THUMBNAIL STREAM
               using (var thumbnailFileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
               {

                  // SCALE BITMAP
                  using (var thumbnailBitmap = Android.Graphics.Bitmap.CreateScaledBitmap(originalBitmap, (int)imageWidth, (int)imageHeight, false))
                  {
                     await thumbnailBitmap.CompressAsync(Android.Graphics.Bitmap.CompressFormat.Jpeg, 70, thumbnailFileStream);
                     await thumbnailFileStream.FlushAsync();

                     return true;
                  }

               }

            }
         }
         catch (Exception) { throw; }
      }

   }
}