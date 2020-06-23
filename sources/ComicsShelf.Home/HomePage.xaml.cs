using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Home
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class HomePage : ContentPage
   {
      readonly Timer CoverSliderTimer;

      public HomePage()
      {
         InitializeComponent();
         CoverSliderTimer = new Timer(5000);
         CoverSliderTimer.Elapsed += this.CoverSliderTimerElapsed;
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         CoverSliderTimer.Start();
      }

      protected override void OnDisappearing()
      {
         CoverSliderTimer.Stop();
         base.OnDisappearing();
      }

      private void CoverSliderTimerElapsed(object sender, ElapsedEventArgs e) =>
         Notifyers.Notify.CoverSliderTimer();

   }
}