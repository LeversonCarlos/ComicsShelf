using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverImage : Grid
   {

      public CoverImage()
      {

         var image = new Image { };
         image.SetBinding(Image.SourceProperty, "ImageSource");
         this.Children.Add(image);

         AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.All);
         AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0, 0, 1, 1));

      }

   }
}
