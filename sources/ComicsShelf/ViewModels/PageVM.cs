namespace ComicsShelf.ViewModels
{
   public class PageVM : ObservableObject
   {

      public short Index { get; set; }
      public string Text { get; set; }
      public string Path { get; set; }
      public PageSizeVM PageSize { get; set; }

      bool _IsVisible;
      public bool IsVisible
      {
         get { return _IsVisible; }
         set { this.SetProperty(ref _IsVisible, value); }
      }

   }
}
