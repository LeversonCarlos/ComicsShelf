using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class EmptyView : FlexLayout
   {

      public EmptyView()
      {
         this.VerticalOptions = LayoutOptions.FillAndExpand;
         this.Direction = FlexDirection.Column;
         this.JustifyContent = FlexJustify.Center;
         this.AlignItems = FlexAlignItems.Center;
         if (Device.Idiom != TargetIdiom.Phone)
         { this.Padding = new Thickness(80, 0); }

         var titleLabel = new Label
         {
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = Helpers.Colors.Accent,
            Text = R.Strings.LIBRARY_EMPTY_MESSAGE_TITLE
         };
         this.Children.Add(titleLabel);

         var messageLabel = new Label
         {
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = Helpers.Colors.Accent,
            Text = R.Strings.LIBRARY_EMPTY_MESSAGE_INSTRUCTIONS
         };
         this.Children.Add(messageLabel);

      }

   }
}
