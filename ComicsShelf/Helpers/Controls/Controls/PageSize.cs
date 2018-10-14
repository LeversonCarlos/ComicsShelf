namespace ComicsShelf.Helpers.Controls
{
   public class PageSize 
   {

      public PageSize(double width, double height)
      {
         this.Width = width;
         this.Height = height;
         this.Orientation = (width > height ? OrientationEnum.Landscape : OrientationEnum.Portrait);
      }

      public double Height { get; set; }
      public double Width { get; set; }

      public enum OrientationEnum { Landscape, Portrait }
      public OrientationEnum Orientation { get; set; }

      public static PageSize Zero
      { get { return new PageSize(0, 0); } }
      public bool IsZero()
      { return this.Height == 0 && this.Width == 0; }


   }
}