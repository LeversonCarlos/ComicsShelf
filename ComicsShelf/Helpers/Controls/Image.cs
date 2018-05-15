namespace ComicsShelf.Helpers.Controls
{
   public class Image : FFImageLoading.Forms.CachedImage
   {

      public Image()
      {
         this.RetryCount = 10;
         this.RetryDelay = 100;
         this.Error += Image_Error;
         this.Success += Image_Success;
      }

      private void Image_Success(object sender, FFImageLoading.Forms.CachedImageEvents.SuccessEventArgs e)
      {
         System.GC.Collect();
      }    

      private async void Image_Error(object sender, FFImageLoading.Forms.CachedImageEvents.ErrorEventArgs e)
      {
         await App.Message.Show(e.ToString());
      }
   }
}