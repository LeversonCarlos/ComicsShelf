namespace ComicsShelf.File
{
   public class FileVM : Helpers.ViewModels.DataVM<FileData>
   {

      #region New
      public FileVM(FileData args)
      {
         this.Title = args.Text;
         this.ViewType = typeof(FilePage);
         this.Data = args;
         this.Initialize += this.OnInitialize;
      }
      #endregion

      private void OnInitialize()
      {
         try
         {
            this.Data.Data.ReadingDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            App.Settings.Database.Update(this.Data.Data);
         }
         catch { }
      }


   }
}