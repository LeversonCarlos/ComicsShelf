namespace ComicsShelf.ViewModels
{
   public class FolderVM
   {

      public FolderVM(string path)
      {
         this.Path = path;
         this.Text = System.IO.Path.GetFileNameWithoutExtension(path);
      }

      public string Text { get; set; }
      public string Path { get; set; }

      public FolderVM[] Folders { get; set; }
      public ItemVM[] Items { get; set; }

      /*
      string _CoverPath;
      public string CoverPath
      {
         get => _CoverPath;
         set => SetProperty(ref _CoverPath, value);
      }

      ItemVM[] _Items;
      public ItemVM[] Items
      {
         get => _Items;
         set
         {
            Items = value;
            // CoverPath = Items?.Select(item => item.CoverPath).FirstOrDefault();
         }
      }
      */

   }
}
