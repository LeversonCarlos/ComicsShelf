namespace ComicsShelf.ViewModels
{
   public class FolderVM
   {

      public FolderVM(string text)
      {
         Text = text;
         // ItemIDs = new List<string>();
      }

      public string Text { get; private set; }

      public ItemVM FirstItem { get; set; }

      // public IList<string> ItemIDs { get; private set; }

   }
}
