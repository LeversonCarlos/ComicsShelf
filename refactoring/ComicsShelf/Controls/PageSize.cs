namespace ComicsShelf.ComicFiles
{
   public class ComicPageSize
   {
      public const string PageSizeChanged = "PageSizeChanged";

      public ComicPageSize(double width, double height)
      {
         this.Width = width;
         this.Height = height;
         this.Orientation = (width > height ? OrientationEnum.Landscape : OrientationEnum.Portrait);
      }

      public double Height { get; set; }
      public double Width { get; set; }

      public enum OrientationEnum { Landscape, Portrait }
      public OrientationEnum Orientation { get; set; }

      public static ComicPageSize Zero
      { get { return new ComicPageSize(0, 0); } }
      public bool IsZero()
      { return this.Height == 0 && this.Width == 0; }

   }

}
