using System;
using System.Threading.Tasks;

namespace ComicsShelf.ViewModels
{
   public partial class BaseVM : ObservableObject, IDisposable
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

      public virtual Task OnAppearing() => Task.CompletedTask;
      public virtual Task OnDisappearing() => Task.CompletedTask;

   }
}