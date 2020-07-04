using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class RatingView : StackLayout
   {

      public RatingView()
      {
         Orientation = StackOrientation.Horizontal;
         Padding = new Thickness(10, 0);
         Spacing = 30;

         _ThumbsUpButton = new ImageButton
         {
            BackgroundColor = Color.Transparent,
            Command = new Command(() => this.SetRating(1))
         };
         Children.Add(_ThumbsUpButton);

         _ThumbsDownButton = new ImageButton
         {
            BackgroundColor = Color.Transparent,
            Command = new Command(() => this.SetRating(-1))
         };
         Children.Add(_ThumbsDownButton);
      }

      readonly ImageButton _ThumbsUpButton;
      readonly ImageButton _ThumbsDownButton;

      void SetRating(short rating)
      {
         if (this.Rating.HasValue && this.Rating.Value == rating) { this.Rating = null; }
         else { this.Rating = rating; }
      }

      short? _Rating;
      public static readonly BindableProperty RatingProperty =
         BindableProperty.Create("Rating", typeof(short?), typeof(RatingView), null, defaultBindingMode: BindingMode.TwoWay,
         propertyChanged: OnRatingChanged);
      public short? Rating
      {
         get { return (short?)GetValue(RatingProperty); }
         set { SetValue(RatingProperty, value); }
      }
      private static void OnRatingChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var ratingView = (bindable as RatingView);
         var value = (short?)newValue;
         ratingView._Rating = value;
         ratingView._ThumbsUpButton.Source = (value.HasValue && value.Value == 1 ? ratingView._ThumbsUpSelectedImageSource : ratingView._ThumbsUpUnselectedImageSource);
         ratingView._ThumbsDownButton.Source = (value.HasValue && value.Value == -1 ? ratingView._ThumbsDownSelectedImageSource : ratingView._ThumbsDownUnselectedImageSource);
      }


      ImageSource _ThumbsUpSelectedImageSource { get; set; }
      public static readonly BindableProperty ThumbsUpSelectedImageSourceProperty =
         BindableProperty.Create("ThumbsUpSelectedImageSource", typeof(ImageSource), typeof(RatingView), null,
         propertyChanged: OnThumbsUpSelectedImageSourceChanged);
      public ImageSource ThumbsUpSelectedImageSource
      {
         get { return (ImageSource)GetValue(ThumbsUpSelectedImageSourceProperty); }
         set { SetValue(ThumbsUpSelectedImageSourceProperty, value); }
      }
      private static void OnThumbsUpSelectedImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as RatingView)._ThumbsUpSelectedImageSource = (ImageSource)newValue; }

      ImageSource _ThumbsUpUnselectedImageSource { get; set; }
      public static readonly BindableProperty ThumbsUpUnselectedImageSourceProperty =
         BindableProperty.Create("ThumbsUpUnselectedImageSource", typeof(ImageSource), typeof(RatingView), null,
         propertyChanged: OnThumbsUpUnselectedImageSourceChanged);
      public ImageSource ThumbsUpUnselectedImageSource
      {
         get { return (ImageSource)GetValue(ThumbsUpUnselectedImageSourceProperty); }
         set { SetValue(ThumbsUpUnselectedImageSourceProperty, value); }
      }
      private static void OnThumbsUpUnselectedImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var ratingView = (bindable as RatingView);
         ratingView._ThumbsUpUnselectedImageSource = (ImageSource)newValue;
         if (ratingView._ThumbsUpButton.Source == null) { ratingView._ThumbsUpButton.Source = (ImageSource)newValue; }
      }

      ImageSource _ThumbsDownSelectedImageSource { get; set; }
      public static readonly BindableProperty ThumbsDownSelectedImageSourceProperty =
         BindableProperty.Create("ThumbsDownSelectedImageSource", typeof(ImageSource), typeof(RatingView), null,
         propertyChanged: OnThumbsDownSelectedImageSourceChanged);
      public ImageSource ThumbsDownSelectedImageSource
      {
         get { return (ImageSource)GetValue(ThumbsDownSelectedImageSourceProperty); }
         set { SetValue(ThumbsDownSelectedImageSourceProperty, value); }
      }
      private static void OnThumbsDownSelectedImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as RatingView)._ThumbsDownSelectedImageSource = (ImageSource)newValue; }

      ImageSource _ThumbsDownUnselectedImageSource { get; set; }
      public static readonly BindableProperty ThumbsDownUnselectedImageSourceProperty =
         BindableProperty.Create("ThumbsDownUnselectedImageSource", typeof(ImageSource), typeof(RatingView), null,
         propertyChanged: OnThumbsDownUnselectedImageSourceChanged);
      public ImageSource ThumbsDownUnselectedImageSource
      {
         get { return (ImageSource)GetValue(ThumbsDownUnselectedImageSourceProperty); }
         set { SetValue(ThumbsDownUnselectedImageSourceProperty, value); }
      }
      private static void OnThumbsDownUnselectedImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var ratingView = (bindable as RatingView);
         ratingView._ThumbsDownUnselectedImageSource = (ImageSource)newValue;
         if (ratingView._ThumbsDownButton.Source == null) { ratingView._ThumbsDownButton.Source = (ImageSource)newValue; }
      }

   }
}
