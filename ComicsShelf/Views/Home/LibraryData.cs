using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Views.Home
{
   public class LibraryData : Folder.FolderData
   {

      public LibraryData(Helpers.Database.ComicFolder comicFolder) : base(comicFolder)
      {
         this.FileTappedCommand = new Command(async (item) => await this.FileTapped(item));
         this.FolderTappedCommand = new Command(async (item) => await this.FolderTapped(item));
         this.NotifyData = new Engine.BaseData();
         this.Sections = new Helpers.Observables.ObservableList<Folder.FolderData>();
         Helpers.Controls.Messaging.Subscribe<Engine.BaseData>(Helpers.Controls.Messaging.Keys.SearchEngine, this.OnNotifyDataChanged);
      }

      public Helpers.Observables.ObservableList<Folder.FolderData> Sections { get; set; }

      public Command FileTappedCommand { get; set; }
      private async Task FileTapped(object item)
      {
         try
         { await Helpers.NavVM.PushAsync<File.FileVM>((File.FileData)item); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      public Command FolderTappedCommand { get; set; }
      private async Task FolderTapped(object item)
      {
         try
         { await Helpers.NavVM.PushAsync<Folder.FolderVM<Folder.FolderData>>((Folder.FolderData)item); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      bool _IsFeaturedPage;
      public bool IsFeaturedPage
      {
         get { return this._IsFeaturedPage; }
         set { this.SetProperty(ref this._IsFeaturedPage, value); }
      }

      Engine.BaseData _NotifyData;
      public Engine.BaseData NotifyData
      {
         get { return this._NotifyData; }
         set { this.SetProperty(ref this._NotifyData, value); }
      }
      private void OnNotifyDataChanged(Engine.BaseData data)
      {
         this.NotifyData.Text = data.Text;
         this.NotifyData.Details = data.Details;
         this.NotifyData.Progress = data.Progress;
         this.NotifyData.IsRunning = data.IsRunning;
      }

   }
}