namespace ComicsShelf.ViewModels
{
   public class FolderVM
   {

      public FolderVM(string text)
      {
         Text = text;
         SizeFactor = 1;
      }

      public string Text { get; private set; }

      public double SizeFactor { get; set; }
      public double SizeHeight => Helpers.Cover.DefaultHeight * SizeFactor;
      public double SizeWidth => Helpers.Cover.DefaultWidth * SizeFactor;

      public ItemVM FirstItem { get; set; }

   }
}
