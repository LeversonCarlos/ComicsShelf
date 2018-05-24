using System;

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
         this.Data.StatsOpacity = 1;
         this.Initialize += this.OnInitialize;
         this.Finalize += this.OnFinalize;
      }
      #endregion

      #region IsSwipeEnabled
      bool _IsSwipeEnabled;
      public bool IsSwipeEnabled
      {
         get { return this._IsSwipeEnabled; }
         set { this.SetProperty(ref this._IsSwipeEnabled, value); }
      }
      #endregion

      #region OnInitialize
      private void OnInitialize()
      {
         try
         {
            this.Data.ReadingDate = App.Database.GetDate();
            App.Database.Update(this.Data.PersistentData);
         }
         catch { }
      }
      #endregion

      #region OnFinalize
      private void OnFinalize()
      {
         try
         {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
         }
         catch { }
      }
      #endregion

   }
}
