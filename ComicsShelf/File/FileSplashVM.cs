using System;

namespace ComicsShelf.File
{
   public class FileSplashVM : Helpers.ViewModels.DataVM<FileData>
   {

      #region New
      public FileSplashVM(FileData args)
      {
         this.Title = args.Text;
         this.ViewType = typeof(FileSplash);
         this.Data = args;
         this.Initialize += this.OnInitialize;
      }
      #endregion

      #region OnInitialize
      private void OnInitialize()
      {
         try
         {
            // App.Settings.Database.Update(this.Data.PersistentData);
         }
         catch { }
      }
      #endregion

      #region Details
      string _Details;
      public string Details
      {
         get { return this._Details; }
         set { this.SetProperty(ref this._Details, value); }
      }
      #endregion

   }
}