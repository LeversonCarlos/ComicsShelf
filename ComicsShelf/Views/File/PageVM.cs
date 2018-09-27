using System;

namespace ComicsShelf.Views.File
{
   public class PageVM : Helpers.DataVM<FileData>
   {

      #region New
      public PageVM(FileData args)
      {
         this.Title = args.FullText;
         this.ViewType = typeof(PageView);
         this.Data = args;
         this._ReadingPage = args.ReadingPage;
         // this.Data.StatsOpacity = 1;
         this.Initialize += this.OnInitialize;
         this.Finalize += this.OnFinalize;
      }
      #endregion


      /*
      #region IsSwipeEnabled
      bool _IsSwipeEnabled;
      public bool IsSwipeEnabled
      {
         get { return this._IsSwipeEnabled; }
         set { this.SetProperty(ref this._IsSwipeEnabled, value); }
      }
      #endregion
      */

      #region ReadingPage
      short _ReadingPage;
      public short ReadingPage
      {
         get { return this._ReadingPage; }
         set
         {
            this.SetProperty(ref this._ReadingPage, value);

            this.Data.ReadingPage = value;
            if (this.Data.Pages != null && this.Data.Pages.Count != 0)
            {
               this.Data.ReadingPercent = ((double)value / (double)this.Data.Pages.Count);
               this.Data.ReadingDate = App.Database.GetDate();
               this.Data.Readed = (value == (this.Data.Pages.Count - 1));
               // this.StatsOpacity = 1.0;
            }
            App.Database.Update(this.Data.ComicFile);
         }
      }
      #endregion


      #region OnInitialize
      private void OnInitialize()
      {
         try
         {
            this.Data.ReadingDate = App.Database.GetDate();
            App.Database.Update(this.Data.ComicFile);
         }
         catch { }
      }
      #endregion

      #region OnFinalize
      private async void OnFinalize()
      {
         try
         { GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

   }
}
