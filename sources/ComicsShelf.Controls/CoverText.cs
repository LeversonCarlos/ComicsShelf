using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverText : StackLayout
   {

      public CoverText()
      {
         Padding = new Thickness(2, 0);
         Spacing = 0;
         BackgroundColor = Color.Black;
         Opacity = 0.25;

         HeightRequest = 18;
         WidthRequest = CoverView.DefaultWidth;

         AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.PositionProportional);
         AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0, 0, -1, -1));

         var textLabel = new Label
         {
            FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
            LineBreakMode = LineBreakMode.MiddleTruncation,
            TextColor = Color.White
         };
         textLabel.SetBinding(Label.TextProperty, "Text");
         Children.Add(textLabel);

      }

      public static readonly BindableProperty TextProperty =
         BindableProperty.Create("Text", typeof(string), typeof(CoverText), "", propertyChanged: OnTextChanged);
      public string Text
      {
         get { return (string)GetValue(TextProperty); }
         set { SetValue(TextProperty, value); }
      }
      private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
      { 
         // (bindable as CoverText).FullTitleLabel.Text = (string)newValue; 
      }


   }
}
