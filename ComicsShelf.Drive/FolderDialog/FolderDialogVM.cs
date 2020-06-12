using ComicsShelf.Base.Observables;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Drive.FolderDialog
{
   internal class FolderDialogVM : BaseVM
   {
      readonly TaskCompletionSource<FolderVM> tcs;
      ObservableList<FolderVM> Data { get; set; }

      public FolderDialogVM()
      {
         this.Title = "select_the_desired_folder";
         this.Data = new ObservableList<FolderVM>();
         this.tcs = new TaskCompletionSource<FolderVM>();
         this.ItemSelectCommand = new Command(async item => await this.ItemSelect(item));
         this.ConfirmCommand = new Command(async () => await this.Confirm());
         this.CancelCommand = new Command(async () => await this.Cancel());
      }

      FolderVM _CurrentItem;
      public FolderVM CurrentItem
      {
         get { return this._CurrentItem; }
         set { this.SetProperty(ref this._CurrentItem, value); }
      }

      public EventHandler<FolderVM> OnItemSelected;
      public Command ItemSelectCommand { get; set; }
      internal async Task ItemSelect(object item)
      {
         this.OnItemSelected?.Invoke(this, item as FolderVM);
         await Task.CompletedTask;
      }

      public Command ConfirmCommand { get; set; }
      async Task Confirm()
      {
         await this.ClosePage();
         tcs.SetResult(this.CurrentItem);
      }

      public Command CancelCommand { get; set; }
      async Task Cancel()
      {
         await this.ClosePage();
         tcs.SetResult(null);
      }

      public async Task<FolderVM> OpenPage()
      {
         var view = new FolderDialogPage { BindingContext = this };
         await Base.Helpers.App.Navigation.PushModalAsync(view, true);
         return await this.tcs.Task;
      }

      async Task ClosePage()
      {
         this.IsBusy = true;
         await Base.Helpers.App.Navigation.PopModalAsync(true);
         this.IsBusy = false;
      }

   }
}
