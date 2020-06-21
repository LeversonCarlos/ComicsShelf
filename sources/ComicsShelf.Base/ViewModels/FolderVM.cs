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

      public ItemVM[] Items { get; set; }
      public FolderVM[] Folders { get; set; }

   }
}
