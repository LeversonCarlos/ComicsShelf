namespace ComicsShelf.ComicFiles
{

   public class ComicPageVM : Helpers.BaseVM
   {

      public ComicPageVM() { }

      public short Index { get; set; }
      public string Text { get; set; }
      public string Path { get; set; }
      public ComicPageSize PageSize { get; set; }

      bool _IsVisible;
      public bool IsVisible
      {
         get { return this._IsVisible; }
         set { this.SetProperty(ref this._IsVisible, value); }
      }

   }

   public class ComicPageSize
   {

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
