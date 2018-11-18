using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class EmptyLibraryView : StackLayout
   {

      public EmptyLibraryView()
      {
         this.VerticalOptions = LayoutOptions.FillAndExpand;
         this.Margin = new Thickness(10, 80);

         this.Children.Add(new Label {
            Text = R.Strings.HOME_NO_COMIC_FILE_FOUND_ON_DEVICE_TITLE,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            FontAttributes = FontAttributes.Bold, 
            TextColor = Colors.Accent
         });

         this.Children.Add(new Label
         {
            Text = R.Strings.HOME_NO_COMIC_FILE_FOUND_ON_DEVICE_INSTRUCTIONS,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label)),
            TextColor = Colors.Accent
         });

      }

   }
}