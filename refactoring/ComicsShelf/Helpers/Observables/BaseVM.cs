namespace ComicsShelf.Helpers.Observables
{
   public class BaseVM : ObservableObject
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

   }
}