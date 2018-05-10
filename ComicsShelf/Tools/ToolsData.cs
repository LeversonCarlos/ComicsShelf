namespace ComicsShelf.Tools
{
   public class ToolsData : Engine.StepData
   {

      #region LibraryPath
      string _LibraryPath;
      public string LibraryPath
      {
         get { return this._LibraryPath; }
         set { this.SetProperty(ref this._LibraryPath, value); }
      }
      #endregion

   }
}