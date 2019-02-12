using Xamarin.Forms.Xaml;

namespace ComicsShelf.Libraries.Service.OneDrive.FolderSelector
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SelectorPage : Helpers.Controls.BasePage
   {
      public SelectorPage()
      {
         InitializeComponent();
      }

      protected override bool OnBackButtonPressed()
      {
         // PREVENT THE BACK BUTON
         return true;
      }

   }
}