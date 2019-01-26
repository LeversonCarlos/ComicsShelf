using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class EmptyLibraryView : StackLayout
   {

      public EmptyLibraryView()
      {
         this.VerticalOptions = LayoutOptions.CenterAndExpand;
         this.Padding = new Thickness(20, 0, 20, 60);

         this.Children.Add(new Label {
            Text = R.Strings.LIBRARY_EMPTY_MESSAGE_TITLE,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center, 
            VerticalTextAlignment = TextAlignment.Center,
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            FontAttributes = FontAttributes.Bold, 
            TextColor = Color.Accent
         });

         this.Children.Add(new Label
         {
            Text = R.Strings.HOME_LIBRARY_EMPTY_MESSAGE_INSTRUCTIONS,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center,
            VerticalTextAlignment = TextAlignment.Center,
            FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label)),
            TextColor = Color.Accent
         });

      }

   }
}