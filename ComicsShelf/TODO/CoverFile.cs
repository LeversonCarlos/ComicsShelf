using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   /*
   public class CoverFile : AbsoluteLayout
   {

      #region DataSource
      public static readonly BindableProperty DataSourceProperty =
         BindableProperty.Create(nameof(DataSource),
            typeof(File.FileData), typeof(CoverFile), null,
            BindingMode.TwoWay,
            propertyChanged: OnDataSourceChanged);
      public File.FileData DataSource
      {
         get { return (File.FileData)GetValue(DataSourceProperty); }
         set { SetValue(DataSourceProperty, value); }
      }
      #endregion

      #region OnDataSourceChanged
      private static async void OnDataSourceChanged(BindableObject bindable, object oldValue, object newValue)
      {
         try
         {
            var DATA = (File.FileData)newValue;

            var VIEW = (CoverFile)bindable;
            VIEW.Margin = new Thickness(6);


            var grid = new Grid
            {
               BackgroundColor = Color.FromHex("9FA8DA"),
               Padding = new Thickness(4),
               Children = {
                  new Image {
                     // Source = DATA.CoverPath,
                     HorizontalOptions = LayoutOptions.Center,
                     VerticalOptions = LayoutOptions.Start,
                     Aspect = Aspect.AspectFit,
                     HeightRequest = 200
                  }
               }
            };
            VIEW.Children.Add(grid);
            AbsoluteLayout.SetLayoutBounds(grid, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(grid, AbsoluteLayoutFlags.All);

            await Task.Run(() => ((Image)grid.Children[0]).Source = ImageSource.FromFile(DATA.CoverPath));

            var stackLayout = new StackLayout
            {
               VerticalOptions = LayoutOptions.FillAndExpand,
               Margin = new Thickness(0),
               Padding = new Thickness(0),
               Children = {
                  new ProgressBar {
                     Progress = DATA.ReadingPercent,
                     HorizontalOptions = LayoutOptions.Fill,
                     VerticalOptions = LayoutOptions.Start,
                     Opacity =0.75
                  },
                  new ContentView {
                     BackgroundColor = Color.FromHex("9FA8DA"),
                     Opacity=0.85, Padding = new Thickness(2),
                     HorizontalOptions = LayoutOptions.Fill,
                     VerticalOptions = LayoutOptions.EndAndExpand,
                     Content = new Label {
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.White,
                        HorizontalOptions = LayoutOptions.Center, 
                        LineBreakMode = LineBreakMode.TailTruncation, 
                        Text = DATA.SmallText
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