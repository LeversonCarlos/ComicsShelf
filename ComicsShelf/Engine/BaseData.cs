namespace ComicsShelf.Engine
{
   public class BaseData : Helpers.Observables.ObservableObject
   {
      internal const string KEY = "Engine";

      #region Text
      string _Text;
      public string Text
      {
         get { return this._Text; }
         set { this.SetProperty(ref this._Text, value); }
      }
      #endregion

      #region Details
      string _Details;
      public string Details
      {
         get { return this._Details; }
         set { this.SetProperty(ref this._Details, value); }
      }
      #endregion

      #region Progress
      double _Progress = 0;
      public double Progress
      {
         get { return this._Progress; }
         set { this.SetProperty(ref this._Progress, value); }
      }
      #endregion

      #region IsRunning
      bool _IsRunning = false;
      public bool IsRunning
      {
         get { return this._IsRunning; }
         set { this.SetProperty(ref this._IsRunning, value); }
      }
      #endregion

   }
}
