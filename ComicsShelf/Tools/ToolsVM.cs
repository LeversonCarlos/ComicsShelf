using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Tools
{
   public class ToolsVM : Helpers.ViewModels.DataVM<ToolsData>
   {

      #region New
      public ToolsVM()
      {
         this.Title = R.Strings.TOOLS_PAGE_TITLE; ;
         this.ViewType = typeof(ToolsPage);
         this.Data = new ToolsData { LibraryPath = App.Settings.Paths.LibraryPath };
         this.LibraryTappedCommand = new Command(async (item) => await this.LibraryTapped(item));
         this.LinkTappedCommand = new Command(async (item) => await this.LinkTapped(item));
         MessagingCenter.Subscribe<Engine.StepData>(this, Engine.StepData.KEY, (data) =>
         {
            this.Data.Text = data.Text;
            this.Data.Details = data.Details;
            this.Data.Progress = data.Progress;
            this.Data.IsRunning = data.IsRunning;
         });
      }
      #endregion

      #region LibraryTapped
      public Command LibraryTappedCommand { get; set; }
      private async Task LibraryTapped(object item)
      {
         try
         {
            if (await Engine.Library.Execute())
            { await Helpers.ViewModels.NavVM.PopAsync(); }
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region LinkTapped
      public Command LinkTappedCommand { get; set; }
      private async Task LinkTapped(object item)
      {
         try
         {
            Device.OpenUri(new Uri("http://www.comicbookplus.com"));
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

   }
}