using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.ComicFiles
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ReadingView : ContentPage
   {
      public ReadingView()
      {
         InitializeComponent();
      }
   }
}