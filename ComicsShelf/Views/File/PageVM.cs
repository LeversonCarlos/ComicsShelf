using System;
using System.Linq;
using Xamarin.Forms.Internals;

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
            var wasReaded = this.Data.Readed;
            if (this.Data.Pages != null && this.Data.Pages.Count != 0)
            {
               this.Data.ReadingPercent = ((double)value / (double)this.Data.Pages.Count);
               this.Data.ReadingDate = App.Database.GetDate();
               this.Data.Readed = (value == (this.Data.Pages.Count - 1));
               if (this.Data.Readed) {
                  this.Data.ReadingPage = 0;
                  this.Data.ReadingPercent = 1;
               }
            }
            App.Database.Update(this.Data.ComicFile);
            if (this.Data.Readed && !wasReaded) {
               Xamarin.Forms.Device.BeginInvokeOnMainThread(async() => await Helpers.NavVM.PopAsync());
            }
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
         {
            this.Data.Pages.Where(page => page.IsVisible).ForEach(page => page.IsVisible = false);
            this.Data.Pages.Clear();
            GC.Collect();
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

   }
}
