using System;
using System.IO;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      #region Thumbnail
      public async Task Thumbnail(Stream imageStream, string imagePath)
      {
         try
         {
            if (System.IO.File.Exists(imagePath)) { return; }
            using (MemoryStream thumbnailStream = await this.Thumbnail_GetThumbnail(imageStream))
            {
               await this.Thumbnail_SaveThumbnail(thumbnailStream, imagePath);
            }
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region Thumbnail_GetThumbnail
      private async Task<MemoryStream> Thumbnail_GetThumbnail(Stream imageStream)
      {
         try
         {
            MemoryStream thumbnailStream = new MemoryStream();
            using (var originalImage = await Android.Graphics.BitmapFactory.DecodeStreamAsync(imageStream))
            {

               // DEFINE SIZE
               double imageHeight = 450; double imageWidth = 150;
               double scaleFactor = (double)imageHeight / (double)originalImage.Height;
               imageHeight = originalImage.Height * scaleFactor;
               imageWidth = originalImage.Width * scaleFactor;

               // THUMBNAIL
               using (var thumbnailImage = Android.Graphics.Bitmap.CreateScaledBitmap(originalImage, (int)imageWidth, (int)imageHeight, false))
               {
                  await thumbnailImage.CompressAsync(Android.Graphics.Bitmap.CompressFormat.Jpeg, 70, thumbnailStream);
                  thumbnailStream.Position = 0;
               }

            }
            return thumbnailStream;
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region Thumbnail_SaveThumbnail
      // private static object saveFileLocker = new object();
      private async Task Thumbnail_SaveThumbnail(Stream thumbnailStream, string imagePath)
      {
         try
         {
            //lock (saveFileLocker)
            {
               FileStream thumbnailFile = null;
               await Task.Run(() => thumbnailFile = System.IO.File.Open(imagePath, FileMode.OpenOrCreate));
               await thumbnailStream.CopyToAsync(thumbnailFile);
               await thumbnailFile.FlushAsync();
               thumbnailFile.Dispose();
            }
         }
         catch (Exception ex) { throw; }
      }
      #endregion

   }
}