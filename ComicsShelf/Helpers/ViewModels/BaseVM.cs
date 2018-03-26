using System;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers.ViewModels
{
   public class BaseVM : Observables.ObservableObject, IDisposable
   {

      #region Title
      string _Title = string.Empty;
      public string Title
      {
         get { return this._Title; }
         set { this.SetProperty(ref this._Title, value); }
      }
      #endregion

      #region IsBusy
      bool _IsBusy = false;
      public bool IsBusy
      {
         get { return this._IsBusy; }
         set { this.SetProperty(ref this._IsBusy, value); }
      }
      #endregion

      #region Events
      public delegate void NotifyHandler();
      protected event NotifyHandler Initialize;
      protected event NotifyHandler Finalize;
      public void InitializeAsync() { this.Initialize?.Invoke(); }
      public void FinalizeAsync() { this.Finalize?.Invoke(); }
      #endregion

      #region Dispose
      public virtual void Dispose()
      {
         /* throw new NotImplementedException(); */
      }
      #endregion

   }
}