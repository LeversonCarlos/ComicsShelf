using ComicsShelf.ViewModels;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Reading
{
   public class ReadingScroll : ScrollView
   {

      public static readonly BindableProperty ImageSizeProperty =
         BindableProperty.Create(nameof(ImageSize), typeof(PageSizeVM), typeof(ReadingScroll), PageSizeVM.Zero,
         propertyChanged: OnImageSizeChanged);
      public PageSizeVM ImageSize
      {
         get => (PageSizeVM)GetValue(ImageSizeProperty);
         set => SetValue(ImageSizeProperty, value);
      }
      static void OnImageSizeChanged(BindableObject bindable, object oldValue, object newValue) =>
         (bindable as ReadingScroll).OnChange();

      public static readonly BindableProperty ReadingPageProperty =
         BindableProperty.Create(nameof(ReadingPage), typeof(short?), typeof(ReadingScroll), null,
         propertyChanged: OnReadingPageChanging);
      public short? ReadingPage
      {
         get => (short?)GetValue(ReadingPageProperty);
         set => SetValue(ReadingPageProperty, value);
      }
      static void OnReadingPageChanging(BindableObject bindable, object oldValue, object newValue) =>
         (bindable as ReadingScroll).OnChange(oldValue as short?, newValue as short?);

      void OnChange() => OnChange(ReadingPage, ReadingPage);
      void OnChange(short? oldPage, short? newPage)
      {
         if (newPage >= oldPage)
            ScrollToAsync(0, 0, false);
         else
         {
            if (ImageSize.Orientation == PageSizeVM.OrientationEnum.Portrait)
               ScrollToAsync(0, 0, false);
            else
               ScrollToAsync(Content.Width, Content.Height, false);
         }
      }

   }
}
