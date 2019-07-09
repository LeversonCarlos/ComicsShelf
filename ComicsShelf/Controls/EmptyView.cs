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

         var titleLabel = new Label
         {
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.Default,
            Text = R.Strings.LIBRARY_EMPTY_MESSAGE_TITLE
         };
         this.Children.Add(titleLabel);

         var messageLabel = new Label
         {
            TextColor = Color.Default,
            Text = R.Strings.HOME_LIBRARY_EMPTY_MESSAGE_INSTRUCTIONS
         };
         this.Children.Add(messageLabel);

      }

   }
}
