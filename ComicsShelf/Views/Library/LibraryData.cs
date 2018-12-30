using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace ComicsShelf.Views.Library
{
   public class LibraryData : Helpers.Observables.ObservableObject
   {

      #region New
      public LibraryData()
      {
         this.RefreshLibraries();
      }
      #endregion


      #region Libraries

      public Helpers.Observables.ObservableList<LibraryDataItem> Libraries { get; set; }

      private void Libraries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      {
         this.HasLibraries = this.Libraries.Count != 0;
         this.HasntLibraries = this.Libraries.Count == 0;
         this.HasntOneDriveLibrary = this.Libraries.Count(x => x.LibraryType == Helpers.Database.LibraryTypeEnum.OneDrive) == 0;
      }

      #endregion


      #region RefreshLibraries
      private void RefreshLibraries()
      {
         try
         {

            if (this.Libraries != null)
            { this.Libraries.CollectionChanged -= this.Libraries_CollectionChanged; }
            this.Libraries = new Helpers.Observables.ObservableList<LibraryDataItem>();
            this.Libraries.CollectionChanged += this.Libraries_CollectionChanged;

            var libraries = App.Database
               .Table<Helpers.Database.Library>()
               .Select(x => new LibraryDataItem(x))
               .AsEnumerable();
            libraries.ForEach(library => this.Libraries.Add(library));
         }
         catch { }
      }
      #endregion

      #region AddLibrary
      internal async Task AddLibrary(Helpers.Database.LibraryTypeEnum libraryType)
      {
         try
         {
            var library = await Engine.Library.AddNew(libraryType);
            if (library == null) { return; }

            this.Libraries.Add(new LibraryDataItem(library));

            if (this.Libraries.Count == 1)
            { await Helpers.NavVM.PushAsync<Views.Home.HomeVM>(true, App.HomeData); }
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

      #region RemoveLibrary
      internal async Task RemoveLibrary(LibraryDataItem library)
      {
         try
         {
            this.Libraries.Remove(library);
            await Engine.Library.Remove(library.Library);
            this.RefreshLibraries();
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion


      #region HasLibraries
      bool _HasLibraries;
      public bool HasLibraries
      {
         get { return this._HasLibraries; }
         set { this.SetProperty(ref this._HasLibraries, value); }
      }
      #endregion

      #region HasntLibraries
      bool _HasntLibraries;
      public bool HasntLibraries
      {
         get { return this._HasntLibraries; }
         set { this.SetProperty(ref this._HasntLibraries, value); }
      }
      #endregion

      #region HasntOneDriveLibrary
      bool _HasntOneDriveLibrary;
      public bool HasntOneDriveLibrary
      {
         get { return this._HasntOneDriveLibrary; }
         set { this.SetProperty(ref this._HasntOneDriveLibrary, value); }
      }
      #endregion

   }
}