﻿using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{

   public class NavPage : NavigationPage
   {
      public NavPage() : base(new InitialPage())
      {
         this.BarTextColor = Color.White;
         this.BarBackgroundColor = Colors.Primary;
      }
   }

   public class InitialPage : ContentPage
   {
      public InitialPage()
      {
         this.Title = "Comics Shelf";
         this.Content = new ActivityIndicator
         {
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            VerticalOptions = LayoutOptions.StartAndExpand
         };
      }
   }

}