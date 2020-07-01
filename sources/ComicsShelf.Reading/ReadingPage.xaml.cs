using ComicsShelf.ViewModels;
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
         BindingContext = ReadingExtentions.Instance;
      }

      ReadingVM Context => BindingContext as ReadingVM;

      protected override void OnAppearing()
      {
         Context?.OnAppearing();
         base.OnAppearing();
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

   }
}