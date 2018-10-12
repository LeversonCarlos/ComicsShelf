using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class EmptyLibraryView : StackLayout
   {

      public EmptyLibraryView()
      {
         this.VerticalOptions = LayoutOptions.CenterAndExpand;
         this.Margin = new Thickness(10, 80);

         this.Children.Add(new Label {
            Text = R.Strings.HOME_NO_COMIC_FILE_FOUND_ON_DEVICE_TITLE,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            TextColor = Color.Black /* colorAccent */
         });

         this.Children.Add(new Label
         {
            Text = R.Strings.HOME_NO_COMIC_FILE_FOUND_ON_DEVICE_INSTRUCTIONS,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            TextColor = Color.DimGray /* colorAccent */
         });

      }

   }
}