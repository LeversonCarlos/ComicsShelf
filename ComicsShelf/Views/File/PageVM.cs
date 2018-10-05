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
         this.StatsOpacityTimer = new System.Timers.Timer
         {
            Enabled = false,
            Interval = 100,
            AutoReset = true
         };
         this.StatsOpacityTimer.Elapsed += this.StatsOpacityTimer_Elapsed;
         this.StatsOpacity = 1.0;
         this.Initialize += this.OnInitialize;
         this.Finalize += this.OnFinalize;
      }
      #endregion


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
               this.StatsOpacity = 1.0;
            }
            App.Database.Update(this.Data.ComicFile);
         }
      }
      #endregion

      #region StatsOpacity
       
      double _StatsOpacity;
      public double StatsOpacity
      {
         get { return this._StatsOpacity; }
         set
         {
            this.SetProperty(ref this._StatsOpacity, value);
            if (!this.StatsOpacityTimer.Enabled)
            { this.StatsOpacityTimer.Start(); }
         }
      }

      System.Timers.Timer StatsOpacityTimer;
      private void StatsOpacityTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
      {
         this.StatsOpacity -= 0.05;
         if (this.StatsOpacity <= 0)
         {
            this.StatsOpacity = 0;
            this.StatsOpacityTimer.Stop();
            this.StatsOpacityTimer.Enabled = false;
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
