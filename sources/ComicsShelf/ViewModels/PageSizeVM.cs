namespace ComicsShelf.ViewModels
{
   public class PageSizeVM
   {

      public double Height { get; set; }
      public double Width { get; set; }

      public enum OrientationEnum { Landscape, Portrait }
      public OrientationEnum Orientation => Width > Height ? OrientationEnum.Landscape : OrientationEnum.Portrait;

      public static PageSizeVM Zero => new PageSizeVM { Height = 0, Width = 0 };
      public bool IsZero() => this.Height == 0 && this.Width == 0;

   }
}
