using System;

namespace ComicsShelf.Reading
{
   partial class ReadingVM
   {

      short _ReadingPage;
      public short ReadingPage
      {
         get => _ReadingPage;
         set
         {
            SetProperty(ref _ReadingPage, value);
            OnReadingPage(value);
         }
      }

      void OnReadingPage(short value)
      {
         try
         {

            Item.ReadingPage = value;
            Item.ReadingDate = DateTime.UtcNow;
            Item.ReadingPercent = (double)value / (double)(PagesList.Count - 1);

            foreach (var page in PagesList)
               page.IsVisible = page.Index >= (value - 1) && page.Index <= (value + 1);

            ScrollComplete = false;
            IsSwipeEnabled = GetSwipeEnabled();

            if (Item.ReadingPercent >= (double)1)
            {
               Item.Readed = true;
               Item.ReadingPage = 0;
               Helpers.Modal.Pop();
            }

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

   }
}
