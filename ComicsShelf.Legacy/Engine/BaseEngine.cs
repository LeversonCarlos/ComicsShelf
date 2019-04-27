using System;

namespace ComicsShelf.Engine
{
   internal abstract class BaseEngine : IDisposable
   {
      protected BaseData Data { get; set; }
      private string MessagePrefix { get; set; }

      public BaseEngine() : this("") { }
      public BaseEngine(string messagePrefix)
      {
         this.MessagePrefix = messagePrefix;
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
      { Helpers.Controls.Messaging.Send(this.MessagePrefix, Helpers.Controls.Messaging.Keys.SearchEngine, this.Data); }

      public void Dispose()
      {
         this.Data.IsRunning = false;
         this.Notify();
         this.Data = null;
      }

   }
}