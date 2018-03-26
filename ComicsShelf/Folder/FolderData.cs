namespace ComicsShelf.Folder
{
   public class FolderData : Helpers.Observables.ObservableObject
   {

      #region New
      public FolderData()
      {
         this.Folders = new Helpers.Observables.ObservableList<FolderData>();
         this.Files = new Helpers.Observables.ObservableList<File.FileData>();
      }
      #endregion

      #region Text
      string _Text;
      public string Text
      {
         get { return this._Text; }
         set { this.SetProperty(ref this._Text, value); }
      }
      #endregion

      #region Children
      public Helpers.Observables.ObservableList<Folder.FolderData> Folders { get; set; }
      public Helpers.Observables.ObservableList<File.FileData> Files { get; set; }
      #endregion

   }
}