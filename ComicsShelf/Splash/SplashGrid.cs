using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class SplashMainCoverGrid : Grid
   {

      private readonly ColumnDefinition Column1;
      private readonly ColumnDefinition Column2;
      private readonly ColumnDefinition Column3;
      private readonly RowDefinition Row1;
      private readonly RowDefinition Row2;
      private readonly RowDefinition Row3;

      public SplashMainCoverGrid()
      {
         this.VerticalOptions = LayoutOptions.FillAndExpand;
         this.HorizontalOptions = LayoutOptions.FillAndExpand;

         this.Column1 = new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) };
         this.Column2 = new ColumnDefinition { Width = new GridLength(80, GridUnitType.Star) };
         this.Column3 = new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) };
         this.ColumnDefinitions.Add(this.Column1);
         this.ColumnDefinitions.Add(this.Column2);
         this.ColumnDefinitions.Add(this.Column3);

         this.Row1 = new RowDefinition { Height = new GridLength(15, GridUnitType.Star) };
         this.Row2 = new RowDefinition { Height = new GridLength(70, GridUnitType.Star) };
         this.Row3 = new RowDefinition { Height = new GridLength(15, GridUnitType.Star) };
         this.RowDefinitions.Add(this.Row1);
         this.RowDefinitions.Add(this.Row2);
         this.RowDefinitions.Add(this.Row3);

      }

      #region PageSize
      public static readonly BindableProperty PageSizeProperty =
         BindableProperty.Create("PageSize", typeof(ComicFiles.ComicPageSize), typeof(SplashMainCoverGrid), ComicFiles.ComicPageSize.Zero,
         propertyChanged: OnPageSizeChanged, defaultBindingMode: BindingMode.TwoWay);
      public ComicFiles.ComicPageSize PageSize
      {
         get { return (ComicFiles.ComicPageSize)GetValue(PageSizeProperty); }
         set { SetValue(PageSizeProperty, value); }
      }
      private static void OnPageSizeChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as SplashMainCoverGrid).OnOrientationChanged(newValue as ComicFiles.ComicPageSize); }
      #endregion

      private void OnOrientationChanged(ComicFiles.ComicPageSize pageSize)
      {
         try
         {

            if (pageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Portrait)
            {
               this.Column1.Width = new GridLength(10, GridUnitType.Star);
               this.Column2.Width = new GridLength(80, GridUnitType.Star);
               this.Column3.Width = new GridLength(10, GridUnitType.Star);
               this.Row1.Height = new GridLength(15, GridUnitType.Star);
               this.Row2.Height = new GridLength(70, GridUnitType.Star);
               this.Row3.Height = new GridLength(15, GridUnitType.Star);
            }

            if (pageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Landscape)
            {
               this.Column1.Width = new GridLength(15, GridUnitType.Star);
               this.Column2.Width = new GridLength(70, GridUnitType.Star);
               this.Column3.Width = new GridLength(15, GridUnitType.Star);
               if (Device.Idiom == TargetIdiom.Phone)
               {
                  this.Row1.Height = new GridLength(1, GridUnitType.Star);
                  this.Row2.Height = new GridLength(98, GridUnitType.Star);
                  this.Row3.Height = new GridLength(1, GridUnitType.Star);
               }
               else
               {
                  this.Row1.Height = new GridLength(10, GridUnitType.Star);
                  this.Row2.Height = new GridLength(80, GridUnitType.Star);
                  this.Row3.Height = new GridLength(10, GridUnitType.Star);
               }
            }

         }
         catch { }
      }

   }
}
