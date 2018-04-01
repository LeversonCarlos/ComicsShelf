namespace ComicsShelf.File
{
   public class FileReadVM : Helpers.ViewModels.DataVM<FileData>
   {

      #region New
      public FileReadVM(FileData args)
      {
         this.Title = args.Text;
         this.ViewType = typeof(FileReadPage);
         this.Data = args;
         this.Initialize += this.OnInitialize;
      }
      #endregion

      #region OnInitialize
      private void OnInitialize()
      {
         try
         {
            this.Data.ReadingDate = App.Settings.GetDatabaseDate();
            App.Settings.Database.Update(this.Data.PersistentData);
         }
         catch { }
      }
      #endregion

   }
}
