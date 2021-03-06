﻿using ComicsShelf.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Screens.Reading
{

   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class Page : ContentPage
   {

      public Page()
      {
         InitializeComponent();
         ReadingExtentions.Instance.IsBusy = true;
         BindingContext = ReadingExtentions.Instance;
      }

      ReadingVM Context => BindingContext as ReadingVM;

      protected override void OnAppearing()
      {
         base.OnAppearing();
         Context?.OnAppearing();
      }

      protected override void OnSizeAllocated(double width, double height)
      {
         if (Context != null)
            Context.ScreenSize = new PageSizeVM { Width = width, Height = height };
         base.OnSizeAllocated(width, height);
      }

      protected override void OnDisappearing()
      {
         Context?.OnDisappearing();
         base.OnDisappearing();
      }

      private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
      {
         var scrollView = (sender as ScrollView);
         var scrollStart = scrollView.ScrollX == 0;
         var scrollEnd = (scrollView.Bounds.Width + scrollView.ScrollX) > scrollView.ContentSize.Width;
         Context.ScrollComplete = scrollStart || scrollEnd;
      }


   }
}