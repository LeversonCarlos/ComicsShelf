using ComicsShelf.Helpers.Observables;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.FolderDialog
{
   internal class FolderDialogVM : BaseVM
   {

      private readonly TaskCompletionSource<SelectorItem> tcs;
      public FolderDialogVM()
      {
         this.Title = R.Strings.FOLDER_DIALOG_TITLE;
         this.Data = new ObservableList<SelectorItem>();
         this.ConfirmCommand = new Command(async () => await this.Confirm());
         this.CancelCommand = new Command(async () => await this.Cancel());
         this.ItemSelectCommand = new Command(async (item) => await this.ItemSelect(item));
         this.tcs = new TaskCompletionSource<SelectorItem>();
      }

      public ObservableList<SelectorItem> Data { get; private set; }
      public SelectorItem SelectedItem { get; set; }

      SelectorItem _CurrentItem;
      public SelectorItem CurrentItem
      {
         get { return this._CurrentItem; }
         set { this.SetProperty(ref this._CurrentItem, value); }
      }

      public EventHandler<SelectorItem> OnItemSelected;
      public Command ItemSelectCommand { get; set; }
      internal async Task ItemSelect(object item)
      {
         this.OnItemSelected?.Invoke(this, this.SelectedItem);
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

      public async Task<SelectorItem> OpenPage()
      {
         var view = new FolderDialogPage { BindingContext = this };
         await App.Navigation().PushModalAsync(view, true);
         return await this.tcs.Task;
      }

      async Task ClosePage()
      {
         this.IsBusy = true;
         await App.Navigation().PopModalAsync(true);
         this.IsBusy = false;
      }

   }
}
