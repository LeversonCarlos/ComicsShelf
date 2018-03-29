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
            Text = "Loading...",
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            VerticalOptions = LayoutOptions.CenterAndExpand
         };
      }

      /*
      protected override void OnAppearing()
      {
         try
         {
            base.OnAppearing();

            // INSTANCE 
            var vm = new Startup.StartupVM();
            var view = Activator.CreateInstance(vm.ViewType) as Page;
            view.BindingContext = vm;

            // NAVIGATION
            var mainPage = Application.Current.MainPage as Page;
            var navigation = mainPage.Navigation;
            navigation.PushModalAsync(view);

         }
         catch (Exception ex) { throw; }
      }
      */

   }
}