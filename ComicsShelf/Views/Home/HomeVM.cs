using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Views.Home
{
   public class HomeVM : Folder.FolderVM<HomeData>
   {

      #region New
      public HomeVM() : this(new HomeData { FullText = R.Strings.AppTitle }) { }
      public HomeVM(HomeData args): base(args)
      {
         this.Title = args.FullText;
         this.ViewType = typeof(HomeView);

         this.Data = args;
         this.OpenLibraryCommand = new Command(async (item) => await this.OpenLibrary(item));

         MessagingCenter.Subscribe<Engine.BaseData>(this, Engine.BaseData.KEY, (data) =>
         {
            this.Data.EngineData.Text = data.Text;
            this.Data.EngineData.Details = data.Details;
            this.Data.EngineData.Progress = data.Progress;
            this.Data.EngineData.IsRunning = data.IsRunning;
         });

         this.Initialize += () =>
         {
            /* if (!this.Data.EngineData.IsRunning) { Engine.Statistics.Execute(); } */
         };
      }
      #endregion


      #region OpenLibrary
      public Command OpenLibraryCommand { get; set; }
      private async Task OpenLibrary(object item)
      {
         try
         { await Helpers.NavVM.PushAsync<Views.Library.LibraryVM>(false); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

   }
}