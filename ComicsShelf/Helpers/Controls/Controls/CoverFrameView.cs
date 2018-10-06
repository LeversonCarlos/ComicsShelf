using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class CoverFrameView : Frame
   {

      // CoverItemFrame
      // CoverItemLayout
      // CoverItemImageGrid
      // CoverItemImage
      // CoverItemTitle

      #region New
      public CoverFrameView()
      {
         this.Margin = 4;
         this.Padding = 0;
         this.HasShadow = true;
         this.BackgroundColor = Color.White;
         this.BorderColor = (Color)Application.Current.Resources["colorGray"];
         this.CornerRadius = 0;

         this.Image = new Image
         {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
            Aspect = Aspect.AspectFit
         };
         this.ImageGrid = new Grid { Padding = 4, Children = { this.Image } };

         this.Label = new Label
         {
            HorizontalOptions = LayoutOptions.Center,
            LineBreakMode = LineBreakMode.MiddleTruncation,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(4, 0)
         };

         this.ProgressBar = new ProgressBar
         {
            HorizontalOptions = LayoutOptions.Fill,
            HeightRequest = 5, 
         };

         this.Content = new StackLayout
         {
            Margin = 0,
            Padding = 0,
            Spacing = 0,
            Children = { this.ImageGrid, this.Label, this.ProgressBar }
         };

      }
      #endregion

      #region ImageHeight
      Grid ImageGrid { get; set; }
      public static readonly BindableProperty ImageHeightProperty =
         BindableProperty.Create("ImageHeight", typeof(double), typeof(CoverFrameView), (double)0,
         propertyChanged: OnImageHeightChanged, defaultBindingMode: BindingMode.TwoWay);
      public double ImageHeight
      {
         get { return (double)GetValue(ImageHeightProperty); }
         set { SetValue(ImageHeightProperty, value); }
      }
      private static void OnImageHeightChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverFrameView).ImageGrid.HeightRequest = (double)newValue; }
      #endregion

      #region ImageSource
      Image Image { get; set; }
      public static readonly BindableProperty ImageSourceProperty =
         BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(CoverFrameView), null,
         propertyChanged: OnImageSourceChanged, defaultBindingMode: BindingMode.TwoWay);
      public ImageSource ImageSource
      {
         get { return (ImageSource)GetValue(ImageSourceProperty); }
         set { SetValue(ImageSourceProperty, value); }
      }
      private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverFrameView).Image.Source = (ImageSource)newValue; }
      #endregion

      #region Text
      Label Label { get; set; }
      public static readonly BindableProperty TextProperty =
         BindableProperty.Create("Text", typeof(string), typeof(CoverFrameView), string.Empty,
         propertyChanged: OnTextChanged, defaultBindingMode: BindingMode.TwoWay);
      public string Text
      {
         get { return (string)GetValue(TextProperty); }
         set { SetValue(TextProperty, value); }
      }
      private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverFrameView).Label.Text = (string)newValue; }
      #endregion

      #region Progress
      ProgressBar ProgressBar { get; set; }
      public static readonly BindableProperty ProgressProperty =
         BindableProperty.Create("Progress", typeof(double), typeof(CoverFrameView), (double)0,
         propertyChanged: OnProgressChanged, defaultBindingMode: BindingMode.TwoWay);
      public double Progress
      {
         get { return (double)GetValue(ProgressProperty); }
         set { SetValue(ProgressProperty, value); }
      }
      private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverFrameView).ProgressBar.Progress = (double)newValue; }
      #endregion

   }
}