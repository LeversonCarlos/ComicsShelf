using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverImage : Grid
   {

      public CoverImage()
      {

         Image = new Image { };
         // image.SetBinding(Image.SourceProperty, "ImageSource");
         Children.Add(Image);

         AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.All);
         AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0, 0, 1, 1));

      }

      readonly Image Image;
      public static readonly BindableProperty ImageSourceProperty =
         BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(CoverImage), null,
         propertyChanged: OnImageSourceChanged);
      public ImageSource ImageSource
      {
         get { return (ImageSource)GetValue(ImageSourceProperty); }
         set { SetValue(ImageSourceProperty, value); }
      }
      private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverImage).Image.Source = (ImageSource)newValue; }

   }
}
