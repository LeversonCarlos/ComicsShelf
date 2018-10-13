using System;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
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
         this.LoadStarImages();
         this.LoadRatingButtons();
         this.RatingRedraw();
      }

      #region Rating
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
            if (this.Rating >= ratingButton.Stars) { ratingButton.Source = this.FullStar; }
            else { ratingButton.Source = this.EmptyStar; }
         }
      }
      #endregion

      #region StarImages
      private ImageSource FullStar;
      private ImageSource EmptyStar;
      private void LoadStarImages()
      {
         var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
         this.FullStar = ImageSource.FromResource("ComicsShelf.Helpers.Controls.RatingView.FullStar.png", executingAssembly);
         this.EmptyStar = ImageSource.FromResource("ComicsShelf.Helpers.Controls.RatingView.EmptyStar.png", executingAssembly);
      }
      #endregion

      #region RatingButtons
      private void LoadRatingButtons()
      {
         for (int stars = 1; stars <= 5; stars++)
         {
            var ratingButton = new RatingButton
            {
               Stars = stars,
               HeightRequest=this.HeightRequest, 
               RatingButtonTapped = RatingChanged
            };
            this.Children.Add(ratingButton);
         }
      }     
      #endregion

      #region RatingChanged
      private void RatingChanged(int stars)
      {      
         if (this.Rating == stars) { this.Rating = 0; }
         else { this.Rating = stars; }
      }
      #endregion

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
         tapGesture.Tapped += async (object sender, EventArgs e) => {
            await this.FadeTo(0.5, 100, Easing.SinOut);
            await this.FadeTo(1.0, 400, Easing.SinIn);
            this.RatingButtonTapped(this.Stars);
         };
         this.GestureRecognizers.Add(tapGesture);
      }
   }

}