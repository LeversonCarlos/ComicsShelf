namespace ComicsShelf.ViewModels
{
   public class PageSizeVM
   {

      public double Height { get; set; }
      public double Width { get; set; }

      public enum OrientationEnum { Landscape, Portrait }
      public OrientationEnum Orientation => Width > Height ? OrientationEnum.Landscape : OrientationEnum.Portrait;

   }
}
