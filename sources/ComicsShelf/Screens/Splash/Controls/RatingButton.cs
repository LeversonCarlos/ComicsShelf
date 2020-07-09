using Xamarin.Forms;

namespace ComicsShelf.Screens.Splash
{
   public class RatingButton : ImageButton
   {

      public RatingButton()
      {
         BackgroundColor = Color.Transparent;
         Command = new Command(() => CommandHandler());
      }


      void CommandHandler() =>
         Rating = SelectedRating;


      public static readonly BindableProperty RatingProperty =
         BindableProperty.Create("Rating", typeof(short?), typeof(RatingButton), null, defaultBindingMode: BindingMode.TwoWay,
         propertyChanged: OnRatingChanged);
      public short? Rating
      {
         get { return (short?)GetValue(RatingProperty); }
         set { SetValue(RatingProperty, value); }
      }
      private static void OnRatingChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var ratingView = (bindable as RatingButton);
         var value = (short?)newValue;
         ratingView.Source = (value.HasValue && value.Value == ratingView.SelectedRating ? ratingView.SelectedImageSource : ratingView.UnselectedImageSource);
      }


      public static readonly BindableProperty SelectedRatingProperty =
         BindableProperty.Create("SelectedRating", typeof(short), typeof(RatingButton), null);
      public short SelectedRating
      {
         get { return (short)GetValue(SelectedRatingProperty); }
         set { SetValue(SelectedRatingProperty, value); }
      }


      // ImageSource _SelectedImageSource { get; set; }
      public static readonly BindableProperty SelectedImageSourceProperty =
         BindableProperty.Create("SelectedImageSource", typeof(ImageSource), typeof(RatingButton), null);
      public ImageSource SelectedImageSource
      {
         get { return (ImageSource)GetValue(SelectedImageSourceProperty); }
         set { SetValue(SelectedImageSourceProperty, value); }
      }
      /*
      private static void OnSelectedImageSourceChanged(BindableObject bindable, object oldValue, object newValue) =>
         (bindable as RatingButton)._SelectedImageSource = (ImageSource)newValue;
      */


      public static readonly BindableProperty UnselectedImageSourceProperty =
         BindableProperty.Create("UnselectedImageSource", typeof(ImageSource), typeof(RatingButton), null,
         propertyChanged: OnUnselectedImageSourceChanged);
      public ImageSource UnselectedImageSource
      {
         get { return (ImageSource)GetValue(UnselectedImageSourceProperty); }
         set { SetValue(UnselectedImageSourceProperty, value); }
      }
      private static void OnUnselectedImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var ratingView = (bindable as RatingButton);
         if (ratingView.Source == null) ratingView.Source = (ImageSource)newValue;
      }

   }
}
