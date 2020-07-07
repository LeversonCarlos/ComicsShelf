using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Screens.Shells
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ShellPage : Xamarin.Forms.Shell
   {
      public ShellPage()
      {
         InitializeComponent();
      }
   }
}