using ComicsShelf.ComicFiles;
using System;

namespace ComicsShelf.Reading
{
   public class ReadingVM : Helpers.Observables.BaseVM
   {

      public ComicFileVM ComicFile { get; set; }
      public ReadingVM(ComicFileVM comicFile)
      {
         this.ComicFile = comicFile;
         this._ReadingPage = this.ComicFile.ReadingPage;
      }

      short _ReadingPage;
      public short ReadingPage
      {
         get { return this._ReadingPage; }
         set
         {
            this.SetProperty(ref this._ReadingPage, value);
            this.ComicFile.ReadingPage = value;
            this.ComicFile.ReadingDate = DateTime.Now;
            var totalPagesIncludingTheCover = (double)(this.ComicFile.ComicFile.TotalPages + 0);
            this.ComicFile.ReadingPercent = Math.Round(((double)value / totalPagesIncludingTheCover * (double)100), 0) / 100;
            Helpers.AppCenter.TrackEvent("ReadingPage.OnPageSplit", $"Page:{value}");
            if (this.ComicFile.ReadingPercent > (double)1)
            {
               this.ComicFile.Readed = true;
               this.ComicFile.ReadingPage = 0;
               Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => await App.Navigation().PopAsync());
            }
         }
      }

      ComicPageSize _PageSize;
      public ComicPageSize PageSize
      {
         get { return this._PageSize; }
         set { this.SetProperty(ref this._PageSize, value); }
      }

   }
}
