namespace ComicsShelf.Helpers
{
   public class BaseVM : Observables.ObservableObject
   {

      string _Title = string.Empty;
      public string Title
      {
         get { return this._Title; }
         set { this.SetProperty(ref this._Title, value); }
      }

      bool _IsBusy = false;
      public bool IsBusy
      {
         get { return this._IsBusy; }
         set { this.SetProperty(ref this._IsBusy, value); }
      }

      /*
      #region IsLoaded
      bool _IsLoaded = false;
      public bool IsLoaded
      {
         get { return this._IsLoaded; }
         set { this.SetProperty(ref this._IsLoaded, value); }
      }
      #endregion

      #region Events
      public delegate void NotifyHandler();
      protected event NotifyHandler Initialize;
      protected event NotifyHandler Finalize;
      public void InitializeAsync() { this.Initialize?.Invoke(); this.IsLoaded = true; }
      public void FinalizeAsync() { this.IsLoaded = false; this.Finalize?.Invoke(); }
      #endregion
      */

   }
}