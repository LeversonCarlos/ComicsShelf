using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Views.Home
{
   public class LibraryData : Folder.FolderData
   {
      public Helpers.Observables.ObservableList<Folder.FolderData> Sections { get; set; }


      public LibraryData(Helpers.Database.ComicFolder comicFolder) : base(comicFolder)
      {
         this.Sections = new Helpers.Observables.ObservableList<Folder.FolderData>();

         // COMMANDS
         this.FileTappedCommand = new Command(async (item) => await this.FileTapped(item));
         this.FolderTappedCommand = new Command(async (item) => await this.FolderTapped(item));

         // FEATURED SECTIONS
         var readingFiles = new Helpers.Database.ComicFolder { Key = "FEATURED_READING_FILES", Text = R.Strings.HOME_READING_FILES_SECTION_TITLE };
         var readingFilesData = new Views.Folder.FolderData(readingFiles) { Files = new Helpers.Observables.ObservableList<File.FileData>() };
         this.Sections.Add(readingFilesData);
         var recentFiles = new Helpers.Database.ComicFolder { Key = "FEATURED_RECENT_FILES", Text = R.Strings.HOME_RECENT_FILES_SECTION_TITLE };
         var recentFilesData = new Views.Folder.FolderData(recentFiles) { Files = new Helpers.Observables.ObservableList<File.FileData>() };
         this.Sections.Add(recentFilesData);

         // NOTIFICATION
         this.NotifyData = new Engine.BaseData();
         if (!string.IsNullOrEmpty(comicFolder.LibraryPath))
         { Helpers.Controls.Messaging.Subscribe<Engine.BaseData>(comicFolder.LibraryPath, Helpers.Controls.Messaging.Keys.SearchEngine, this.OnNotifyDataChanged); }
      }


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


      public Folder.FolderData RecentFiles
      { get { return this.FeaturedSection("FEATURED_RECENT_FILES"); } }

      public Folder.FolderData ReadingFiles
      { get { return this.FeaturedSection("FEATURED_READING_FILES"); } }

      private Folder.FolderData FeaturedSection(string featuredSectionName)
      {
         var featuredSection = this.Sections.Where(x => x.ComicFolder.Key == featuredSectionName).FirstOrDefault();
         return featuredSection;
         // if (featuredSection == null) { return new Helpers.Observables.ObservableList<File.FileData>(); }
         // return featuredSection.Files;
      }


      bool _IsEmptyPage;
      public bool IsEmptyPage
      {
         get { return this._IsEmptyPage; }
         set { this.SetProperty(ref this._IsEmptyPage, value); }
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