using System;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{

   public class RatingView : StackLayout
   {

      public RatingView()
      {
         this.Orientation = StackOrientation.Horizontal;
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
      {
         try
         {
            var VIEW = bindable as RatingView;
            VIEW.RatingRedraw();
         }
         catch { }
      }
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
         this.FullStar = ImageSource.FromResource("ComicsShelf.Helpers.Controls.RatingView.FullStar.png", System.Reflection.Assembly.GetExecutingAssembly());
         this.EmptyStar = ImageSource.FromResource("ComicsShelf.Helpers.Controls.RatingView.EmptyStar.png", System.Reflection.Assembly.GetExecutingAssembly());
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
         tapGesture.Tapped += (object sender, EventArgs e) => { this.RatingButtonTapped(this.Stars); };
         this.GestureRecognizers.Add(tapGesture);
      }
   }

}