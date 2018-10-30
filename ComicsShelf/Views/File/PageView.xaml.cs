using Xamarin.Forms.Xaml;

namespace ComicsShelf.Views.File
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class PageView : Helpers.Controls.ReadingPage
   {
      public PageView()
      {
         InitializeComponent();
      }
   }
}