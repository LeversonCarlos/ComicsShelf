using System;

namespace ComicsShelf.Engine
{
   internal abstract class BaseEngine : IDisposable
   {
      protected Helpers.iFileSystem FileSystem { get; set; }
      protected BaseData Data { get; set; }

      public BaseEngine()
      {
         this.FileSystem = Helpers.FileSystem.Get();
         this.Data = new BaseData
         {
            Text = string.Empty,
            Details = string.Empty,
            IsRunning = false
         };
         this.Notify();
      }

      protected void Notify(string text)
      {
         this.Data.Text = text;
         this.Data.Details = string.Empty;
         this.Data.Progress = 0;
         this.Data.IsRunning = true;
         this.Notify();
      }

      protected void Notify(string details, double progress)
      {
         this.Data.Details = details;
         this.Data.Progress = progress;
         this.Notify();
      }

      protected void Notify()
      { Helpers.Controls.Messaging.Send(Helpers.Controls.Messaging.Keys.SearchEngine, this.Data); }

      public void Dispose()
      {
         this.Data.IsRunning = false;
         this.Notify();
         this.Data = null;
         this.FileSystem = null;
      }

   }
}