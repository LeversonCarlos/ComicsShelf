using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverView : AbsoluteLayout
   {

      public CoverView()
      {
         this.Margin = 0;
         this.Padding = 0;
         this.VerticalOptions = LayoutOptions.FillAndExpand;
         // this.BackgroundColor = Color.White;

         this.Image = new Image
         {
            VerticalOptions = LayoutOptions.Start,
            Aspect = Aspect.AspectFill
         };
         // this.Children.Add(this.Image);
         var imageContainer = new Grid { Children = { this.Image } };
         this.Children.Add(imageContainer);
         AbsoluteLayout.SetLayoutBounds(imageContainer, new Rectangle(0, 0, 1, 1));
         AbsoluteLayout.SetLayoutFlags(imageContainer, AbsoluteLayoutFlags.All);

         this.ProgressBar = new ProgressBar
         {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.EndAndExpand,
            HeightRequest = 5,
            Opacity = 0.75
         };
         // this.Children.Add(this.ProgressBar);
         var overlayContainer = new StackLayout
         {
            VerticalOptions = LayoutOptions.FillAndExpand,
            Children = { this.ProgressBar },
            InputTransparent = true
         };
         this.Children.Add(overlayContainer);
         AbsoluteLayout.SetLayoutBounds(overlayContainer, new Rectangle(0, 0, 1, 1));
         AbsoluteLayout.SetLayoutFlags(overlayContainer, AbsoluteLayoutFlags.All);

         AbsoluteLayout.SetLayoutBounds(this.ProgressBar, new Rectangle(1, 1, 1, 1));
         AbsoluteLayout.SetLayoutFlags(this.ProgressBar, AbsoluteLayoutFlags.YProportional);

      }


      Image Image { get; set; }
      public static readonly BindableProperty ImageSourceProperty =
         BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(CoverView), null,
         propertyChanged: OnImageSourceChanged);
      public ImageSource ImageSource
      {
         get { return (ImageSource)GetValue(ImageSourceProperty); }
         set { SetValue(ImageSourceProperty, value); }
      }
      private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverView).Image.Source = (ImageSource)newValue; }


      ProgressBar ProgressBar { get; set; }
      public static readonly BindableProperty ProgressProperty =
         BindableProperty.Create("Progress", typeof(double), typeof(CoverView), (double)0,
         propertyChanged: OnProgressChanged);
      public double Progress
      {
         get { return (double)GetValue(ProgressProperty); }
         set { SetValue(ProgressProperty, value); }
      }
      private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverView).ProgressBar.Progress = (double)newValue; }

   }
}