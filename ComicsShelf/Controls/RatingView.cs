using System;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{

   public class RatingView : StackLayout
   {

      public RatingView()
      {
         this.Orientation = StackOrientation.Horizontal;
         if (Device.Idiom == TargetIdiom.Phone)
         { this.HeightRequest = 50; }
         else if (Device.Idiom == TargetIdiom.Tablet)
         { this.HeightRequest = 75; }
         else { this.HeightRequest = 90; }
         this.LoadRatingButtons();
         this.RatingRedraw();
      }

      public static readonly BindableProperty RatingProperty =
         BindableProperty.Create("Rating", typeof(int), typeof(RatingView), 0,
         propertyChanged: OnRatingChanged, defaultBindingMode: BindingMode.TwoWay);
      public int Rating
      {
         get { return (int)GetValue(RatingProperty); }
         set { SetValue(RatingProperty, value); }
      }
      private static void OnRatingChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as RatingView).RatingRedraw(); }

      private void RatingRedraw()
      {
         foreach (RatingButton ratingButton in this.Children)
         {
            if (this.Rating >= ratingButton.Stars) { ratingButton.Source = "icon_FullStar.png"; }
            else { ratingButton.Source = "icon_EmptyStar.png"; }
         }
      }

      private void LoadRatingButtons()
      {
         for (int stars = 1; stars <= 5; stars++)
         {
            var ratingButton = new RatingButton
            {
               Stars = stars,
               HeightRequest = this.HeightRequest,
               RatingButtonTapped = (int val) =>
               {
                  if (this.Rating == val) { this.Rating = 0; }
                  else { this.Rating = val; }
               }
            };
            this.Children.Add(ratingButton);
         }
      }

   }

   public delegate void RatingButtonTappedHandler(int stars);
   public class RatingButton : Xamarin.Forms.Image
   {
      public int Stars { get; set; }
      public RatingButtonTappedHandler RatingButtonTapped;
      public RatingButton()
      {
         this.Aspect = Aspect.AspectFit;
         var tapGesture = new TapGestureRecognizer();
         tapGesture.Tapped += async (object sender, EventArgs e) =>
         {
            await this.ScaleTo(0.85, 150, Easing.SpringIn);
            await this.ScaleTo(1.00, 100, Easing.SpringOut);
            this.RatingButtonTapped(this.Stars);
         };
         this.GestureRecognizers.Add(tapGesture);
      }
   }

}