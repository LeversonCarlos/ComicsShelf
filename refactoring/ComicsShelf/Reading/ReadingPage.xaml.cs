using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Reading
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ReadingPage : ContentPage
   {
      public ReadingPage()
      {
         InitializeComponent();
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         if (!Xamarin.Essentials.OrientationSensor.IsMonitoring)
         { Xamarin.Essentials.OrientationSensor.Start(Xamarin.Essentials.SensorSpeed.Default); }
      }

      protected override void OnDisappearing()
      {
         try
         {
            base.OnDisappearing();
            Xamarin.Essentials.OrientationSensor.Stop();
            if (this.BindingContext != null)
            {
               var libraryID = (this.BindingContext as ReadingVM).ComicFile.ComicFile.LibraryKey;
               Services.LibraryService.UpdateLibrary(libraryID);
            }
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
         finally { GC.Collect(); }
      }

   }
}