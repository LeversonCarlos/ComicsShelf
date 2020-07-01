using System.IO;
using Xamarin.Forms;

namespace ComicsShelf.Reading
{
   public class ReadingImage : Image
   {

      public static readonly BindableProperty ImagePathProperty =
         BindableProperty.Create("ImagePath", typeof(string), typeof(ReadingImage), string.Empty,
         propertyChanged: OnImagePathChanged);
      public string ImagePath
      {
         get { return (string)GetValue(ImagePathProperty); }
         set { SetValue(ImagePathProperty, value); }
      }
      static void OnImagePathChanged(BindableObject bindable, object oldValue, object newValue) =>
         (bindable as ReadingImage).OnImageRefresh();

      public static readonly BindableProperty ImageLoadedProperty =
         BindableProperty.Create("ImageLoaded", typeof(bool), typeof(ReadingImage), false,
         propertyChanged: OnImageLoadedChanged);
      public bool ImageLoaded
      {
         get { return (bool)GetValue(ImageLoadedProperty); }
         set { SetValue(ImageLoadedProperty, value); }
      }
      static void OnImageLoadedChanged(BindableObject bindable, object oldValue, object newValue) =>
         (bindable as ReadingImage).OnImageRefresh();

      void OnImageRefresh()
      {
         if (!this.ImageLoaded && this.Source != null)
            this.Source = null;
         if (this.ImageLoaded && this.Source == null && !string.IsNullOrEmpty(this.ImagePath))
            this.Source = ImageSource.FromStream(() => new MemoryStream(File.ReadAllBytes(this.ImagePath)));
      }

   }
}
