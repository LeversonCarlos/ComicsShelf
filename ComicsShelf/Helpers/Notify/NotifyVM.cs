namespace ComicsShelf.Notify
{
   public class NotifyVM : Helpers.Observables.ObservableObject
   {

      #region New
      private readonly string NotifyKey;
      public NotifyVM(string key)
      {
         this.NotifyKey = key;
         Subscribe(this.NotifyKey, val =>
         {
            this.Text = val.Text;
            this.Details = val.Details;
            this.Progress = val.Progress;
            this.IsRunning = val.IsRunning;
         });
      }
      #endregion

      #region Text
      string _Text;
      public string Text
      {
         get { return this._Text; }
         set { this.SetProperty(ref this._Text, value); }
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

      #region Progress
      double _Progress = 0;
      public double Progress
      {
         get { return this._Progress; }
         set { this.SetProperty(ref this._Progress, value); }
      }
      #endregion

      #region IsRunning
      bool _IsRunning = false;
      public bool IsRunning
      {
         get { return this._IsRunning; }
         set { this.SetProperty(ref this._IsRunning, value); }
      }
      #endregion

      #region Send

      public void Send(string text)
      {
         this.Text = text;
         this.Details = string.Empty;
         this.Progress = 0;
         this.IsRunning = true;
         Send(this.NotifyKey, this);
      }

      public void Send(string details, double progress)
      {
         this.Details = details;
         this.Progress = progress;
         Send(this.NotifyKey, this);
      }

      public void Send(bool isRunning)
      {
         this.IsRunning = isRunning;
         Send(this.NotifyKey, this);
      }

      public static void Send(string key, NotifyVM value)
      { Messaging.Send(key, value); }

      public static void Subscribe(string key, System.Action<NotifyVM> callback)
      { Messaging.Subscribe<NotifyVM>(key, callback); }

      #endregion

   }
}
