using Xamarin.Forms;

namespace ComicsShelf
{
   internal class InitialPage : ContentPage
   {

      public InitialPage()
      {
         Title = R.Strings.AppTitle;
         Content = new Label
         {
            Text = R.Strings.BASE_WAIT_MESSAGE,
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            VerticalOptions = LayoutOptions.CenterAndExpand
         };
      }

   }
}