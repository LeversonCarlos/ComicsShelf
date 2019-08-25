using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class SplashMainCoverGrid : Grid
   {

      private readonly ColumnDefinition Column1;
      private readonly ColumnDefinition Column2;
      private readonly ColumnDefinition Column3;
      private readonly RowDefinition Row1;
      private readonly RowDefinition Row2;
      private readonly RowDefinition Row3;

      public SplashMainCoverGrid()
      {
         this.VerticalOptions = LayoutOptions.FillAndExpand;
         this.HorizontalOptions = LayoutOptions.FillAndExpand;

         this.Column1 = new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) };
         this.Column2 = new ColumnDefinition { Width = new GridLength(80, GridUnitType.Star) };
         this.Column3 = new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) };
         this.ColumnDefinitions.Add(this.Column1);
         this.ColumnDefinitions.Add(this.Column2);
         this.ColumnDefinitions.Add(this.Column3);

         this.Row1 = new RowDefinition { Height = new GridLength(15, GridUnitType.Star) };
         this.Row2 = new RowDefinition { Height = new GridLength(70, GridUnitType.Star) };
         this.Row3 = new RowDefinition { Height = new GridLength(15, GridUnitType.Star) };
         this.RowDefinitions.Add(this.Row1);
         this.RowDefinitions.Add(this.Row2);
         this.RowDefinitions.Add(this.Row3);

         Xamarin.Essentials.OrientationSensor.ReadingChanged +=
            (object sender, Xamarin.Essentials.OrientationSensorChangedEventArgs e) => { this.OnOrientationChanged(); };
         /*
         Xamarin.Essentials.DeviceDisplay.MainDisplayInfoChanged +=
            (object sender, Xamarin.Essentials.DisplayInfoChangedEventArgs e) => { this.OnOrientationChanged(); };
         */
      }

      private void OnOrientationChanged()
      {
         try
         {
            var displayInfo = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;

            if (displayInfo.Orientation == Xamarin.Essentials.DisplayOrientation.Portrait)
            {
               this.Column1.Width = new GridLength(10, GridUnitType.Star);
               this.Column2.Width = new GridLength(80, GridUnitType.Star);
               this.Column3.Width = new GridLength(10, GridUnitType.Star);
               this.Row1.Height = new GridLength(15, GridUnitType.Star);
               this.Row2.Height = new GridLength(70, GridUnitType.Star);
               this.Row3.Height = new GridLength(15, GridUnitType.Star);
            }

            if (displayInfo.Orientation == Xamarin.Essentials.DisplayOrientation.Landscape)
            {
               this.Column1.Width = new GridLength(15, GridUnitType.Star);
               this.Column2.Width = new GridLength(70, GridUnitType.Star);
               this.Column3.Width = new GridLength(15, GridUnitType.Star);
               if (Device.Idiom == TargetIdiom.Phone)
               {
                  this.Row1.Height = new GridLength(1, GridUnitType.Star);
                  this.Row2.Height = new GridLength(98, GridUnitType.Star);
                  this.Row3.Height = new GridLength(1, GridUnitType.Star);
               }
               else
               {
                  this.Row1.Height = new GridLength(10, GridUnitType.Star);
                  this.Row2.Height = new GridLength(80, GridUnitType.Star);
                  this.Row3.Height = new GridLength(10, GridUnitType.Star);
               }
            }

         }
         catch { }
      }

   }
}
