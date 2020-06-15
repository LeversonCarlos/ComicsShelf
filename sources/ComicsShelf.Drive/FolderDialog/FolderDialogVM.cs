using ComicsShelf.Observables;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Drive.FolderDialog
{
   internal class FolderDialogVM : BaseVM
   {
      readonly TaskCompletionSource<ItemVM> tcs;
      public ObservableList<ItemVM> Data { get; private set; }

      public FolderDialogVM()
      {
         this.Title = "select_the_desired_folder";
         this.Data = new ObservableList<ItemVM>();
         this.tcs = new TaskCompletionSource<ItemVM>();
         this.ItemSelectCommand = new Command(async item => await this.ItemSelect(item));
         this.ConfirmCommand = new Command(async () => await this.Confirm());
         this.CancelCommand = new Command(async () => await this.Cancel());
      }

      ItemVM _CurrentItem;
      public ItemVM CurrentItem
      {
         get { return this._CurrentItem; }
         set { this.SetProperty(ref this._CurrentItem, value); }
      }

      public EventHandler<ItemVM> OnItemSelected;
      public Command ItemSelectCommand { get; set; }
      internal async Task ItemSelect(object item)
      {
         this.OnItemSelected?.Invoke(this, item as ItemVM);
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

      public async Task<ItemVM> OpenPage()
      {
         var view = new FolderDialogPage { BindingContext = this };
         await Helpers.App.Navigation.PushModalAsync(view, true);
         return await this.tcs.Task;
      }

      async Task ClosePage()
      {
         this.IsBusy = true;
         await Helpers.App.Navigation.PopModalAsync(true);
         this.IsBusy = false;
      }

   }
}
