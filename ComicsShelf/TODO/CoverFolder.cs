using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   /*
   public class CoverFolder : AbsoluteLayout
   {

      #region DataSource
      public static readonly BindableProperty DataSourceProperty =
         BindableProperty.Create(nameof(DataSource),
            typeof(Folder.FolderData), typeof(CoverFolder), null,
            BindingMode.TwoWay,
            propertyChanged: OnDataSourceChanged);
      public Folder.FolderData DataSource
      {
         get { return (Folder.FolderData)GetValue(DataSourceProperty); }
         set { SetValue(DataSourceProperty, value); }
      }
      #endregion

      #region OnDataSourceChanged
      private static void OnDataSourceChanged(BindableObject bindable, object oldValue, object newValue)
      {
         try
         {
            var DATA = (Folder.FolderData)newValue;

            var VIEW = (CoverFolder)bindable;
            VIEW.Margin = new Thickness(6);

            var grid = new Grid
            {
               BackgroundColor = Color.FromHex("E8EAF6"),
               Padding = new Thickness(4),
               Children = {
                  new Image {
                     Source = DATA.CoverPath,
                     HorizontalOptions = LayoutOptions.FillAndExpand,
                     VerticalOptions = LayoutOptions.Start,
                     Aspect = Aspect.AspectFill,
                     HeightRequest = 150
                  }
               }
            };
            VIEW.Children.Add(grid);
            AbsoluteLayout.SetLayoutBounds(grid, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(grid, AbsoluteLayoutFlags.All);

            var stackLayout = new StackLayout
            {
               VerticalOptions = LayoutOptions.FillAndExpand,
               Margin = new Thickness(0),
               Padding = new Thickness(0),
               Children = {
                  new ContentView {
                     BackgroundColor = Color.FromHex("E8EAF6"),
                     Opacity=0.85, Padding = new Thickness(2),
                     HorizontalOptions = LayoutOptions.Fill,
                     VerticalOptions = LayoutOptions.EndAndExpand,
                     Content = new Label {
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.Black,
                        HorizontalOptions = LayoutOptions.Center,
                        LineBreakMode = LineBreakMode.TailTruncation,
                        Text = DATA.Text
                     }
                  }
               }
            };
            VIEW.Children.Add(stackLayout);
            AbsoluteLayout.SetLayoutBounds(stackLayout, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(stackLayout, AbsoluteLayoutFlags.All);

         }
         catch { }
      }
      #endregion

   }
   */
}