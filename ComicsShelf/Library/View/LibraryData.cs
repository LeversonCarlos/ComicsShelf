namespace ComicsShelf.Library
{
   public class LibraryData : Helpers.Observables.ObservableObject
   {

      #region New
      internal Library Library { get; set; }
      internal LibraryData(Library _Library)
      {
         this.Library = _Library;
         this._LibraryID = this.Library.LibraryID;
         this._LibraryText = this.Library.Description;
         this._LibraryType = this.Library.Type;
      }
      #endregion

      #region LibraryID
      string _LibraryID;
      public string LibraryID
      {
         get { return this._LibraryID; }
         set
         {
            this.SetProperty(ref this._LibraryID, value);
            this.Library.LibraryID = value;
         }
      }
      #endregion

      #region LibraryText
      string _LibraryText;
      public string LibraryText
      {
         get { return this._LibraryText; }
         set
         {
            this.SetProperty(ref this._LibraryText, value);
            this.Library.Description = value;
         }
      }
      #endregion

      #region LibraryType
      TypeEnum _LibraryType;
      public TypeEnum LibraryType
      {
         get { return this._LibraryType; }
         set
         {
            this.SetProperty(ref this._LibraryType, value);

            this.Library.Type = value; 
         }
      }
      #endregion

      #region LibraryImage
      public string LibraryImage
      {
         get {
            var libraryImage = "icon_Folder_Black";
            if (this.LibraryType == TypeEnum.OneDrive)
            { libraryImage = "icon_OneDrive_Black"; }

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.UWP)
            { libraryImage = $"Assets/{libraryImage}"; }

            return $"{libraryImage}.png";
         }
      }
      #endregion

   }
}