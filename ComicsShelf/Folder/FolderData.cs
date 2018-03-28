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

      #region CoverPath
      string _CoverPath;
      public string CoverPath
      {
         get { return this._CoverPath; }
         set { this.SetProperty(ref this._CoverPath, value); }
      }
      #endregion

      #region Children
      public Helpers.Observables.ObservableList<Folder.FolderData> Folders { get; set; }
      public Helpers.Observables.ObservableList<File.FileData> Files { get; set; }
      public Helpers.Observables.ObservableList<File.FileData> RecentFiles { get; set; }
      #endregion

   }
}