using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   public class FolderDialog : IDisposable
   {

      private const string rSelectFolder = "Select folder";
      private const string rCurrentSelection = "Current Selection:";
      private const string rCommandOK = "OK";
      private const string rCommandCancel = "Cancel";

      #region New
      public FolderDialog(Context androidContext)
      {
         this._AndroidContext = androidContext;

         _SdcardDirectory = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

         try
         { _SdcardDirectory = new Java.IO.File(_SdcardDirectory).CanonicalPath; }
         catch (IOException) { }
      }
      #endregion

      #region Properties
      Context _AndroidContext;
      AlertDialog _dialog;
      AutoResetEvent _autoResetEvent;
      string _initialPath;
      string _resultPath;
      String _SdcardDirectory = "";
      int _padding10;
      #endregion

      #region GetDirectoryAsync
      public static async Task<string> GetDirectoryAsync(string initialPath)
      {
         try
         {
            using (var folderDialog = new FolderDialog(Xamarin.Forms.Forms.Context))
            {
               folderDialog._resultPath = string.Empty;

               if (string.IsNullOrEmpty(initialPath))
               { initialPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath; }
               folderDialog._initialPath = new Java.IO.File(initialPath).CanonicalPath;

               if (!folderDialog.CreateDialog()) { return folderDialog._resultPath; }
               folderDialog.CurrentPath = folderDialog._initialPath;
               if (!await folderDialog.AwaitSelection()) { return folderDialog._resultPath; }

               return folderDialog._resultPath; ;
            }
         }
         catch { return string.Empty; }
      }
      #endregion

      #region CreateDialog

      private bool CreateDialog()
      {
         try
         {
            this._padding10 = this.ConvertDpToPx(10);
            AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this._AndroidContext);
            dialogBuilder.SetTitle(rSelectFolder);
            dialogBuilder.SetView(this.CreateDialog_GetSelectionPanel());

            var initialAdapter = StringsArrayAdaper.GetAdaper(_AndroidContext, this._initialPath);
            //var initialList = StringsArrayAdaper.GetDirectories(this._initialPath);
            EventHandler<DialogClickEventArgs> onItemSelected = (object sender, DialogClickEventArgs args) => {
               string selectedItem = "" + this._dialog.ListView.Adapter.GetItem(args.Which);
               if (selectedItem.Equals(".."))
               {
                  this.CurrentPath = this.CurrentPath.Substring(0, this.CurrentPath.LastIndexOf("/"));
                  if ("".Equals(this.CurrentPath)) { this.CurrentPath = "/"; }
               }
               else { this.CurrentPath = selectedItem; }
               
            };
            dialogBuilder.SetSingleChoiceItems(initialAdapter, -1, onItemSelected);

            dialogBuilder.SetCancelable(true);
            dialogBuilder.SetPositiveButton(rCommandOK, (sender, args) => { this._resultPath = this.CurrentPath; this._autoResetEvent.Set(); });
            dialogBuilder.SetNegativeButton(rCommandCancel, (sender, args) => { });
            this._dialog = dialogBuilder.Create();
            this._dialog.CancelEvent += (sender, args) => { this._autoResetEvent.Set(); };
            this._dialog.DismissEvent += (sender, args) => { this._autoResetEvent.Set(); };

            return true;
         }
         catch (Exception ex) { throw; }
      }

      private LinearLayout CreateDialog_GetSelectionPanel()
      {
         try
         {
            LinearLayout selectionPanel = new LinearLayout(this._AndroidContext);
            selectionPanel.Orientation = Orientation.Vertical;
            selectionPanel.SetGravity(GravityFlags.Top);

            var label = new TextView(this._AndroidContext);
            label.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            label.SetPadding(this._padding10 * 2, this._padding10 / 2, this._padding10 * 2, this._padding10 / 2);
            label.SetTextColor(Color.Black);
            label.Gravity = GravityFlags.CenterVertical;
            label.Text = rCurrentSelection;
            label.SetTextSize(Android.Util.ComplexUnitType.Dip, 10);
            label.SetTypeface(null, TypefaceStyle.Normal);
            selectionPanel.AddView(label);

            _CurrentPathView = new TextView(this._AndroidContext);
            _CurrentPathView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            _CurrentPathView.SetPadding(this._padding10 * 2, this._padding10 / 2, this._padding10 * 2, this._padding10 / 2);
            _CurrentPathView.SetTextColor(Color.Black);
            _CurrentPathView.Gravity = GravityFlags.CenterVertical;
            _CurrentPathView.SetTextSize(Android.Util.ComplexUnitType.Dip, 10);
            _CurrentPathView.SetTypeface(null, TypefaceStyle.Bold);
            _CurrentPathView.Text = string.Empty;
            selectionPanel.AddView(_CurrentPathView);

            return selectionPanel;
         }
         catch { throw; }
      }

      #endregion

      #region SetInitialPath
      private bool SetInitialPath(string initialPath)
      {
         try
         {

            if (string.IsNullOrEmpty(initialPath))
            { initialPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath; }

            this._initialPath = new Java.IO.File(initialPath).CanonicalPath;
            this.CurrentPath = this._initialPath;
            this._resultPath = string.Empty;

            return true;
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region AwaitSelection
      private async Task<bool> AwaitSelection()
      {
         try
         {
            this._autoResetEvent = new AutoResetEvent(false);
            this._dialog.Show();
            await Task.Run(() => { _autoResetEvent.WaitOne(); });
            return (!string.IsNullOrEmpty(this._resultPath));
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region CurrentPath
      string _CurrentPath;
      TextView _CurrentPathView;
      public string CurrentPath
      {
         get { return _CurrentPath; }
         set
         {
            _CurrentPath = value;
            this._CurrentPathView.Text = value;        

            this._dialog.ListView.Adapter = null;
            this._dialog.ListView.Adapter = StringsArrayAdaper.GetAdaper(_AndroidContext, value);
         }
      }
      #endregion

      #region ConvertDpToPx
      private int ConvertDpToPx(int dp)
      {
         var r = this._AndroidContext.Resources;
         var metrics = r.DisplayMetrics;
         int px = (int)Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Dip, dp, metrics);
         return px;
      }
      #endregion

      #region Dispose
      public void Dispose()
      {
         throw new NotImplementedException();
      }
      #endregion

   }

   internal class StringsArrayAdaper : ArrayAdapter<String>
   {

      public StringsArrayAdaper(Context context, int resource, int textViewResourceId, IList<string> objects) : base(context, resource, textViewResourceId, objects)
      { }      

      public override View GetView(int position, View convertView, ViewGroup parent)
      {
        
         View v = base.GetView(position, convertView, parent);
         if (v is TextView)
         {
            TextView tv = (TextView)v;
            // tv.SetTextSize(Android.Util.ComplexUnitType.Dip, 20);
            // tv.SetPadding(20, 10, 20, 10);
            tv.LayoutParameters.Height = ViewGroup.LayoutParams.WrapContent;
            tv.Ellipsize = null;
         }
         return v;
      }

      public static List<String> GetDirectories(string path)
      {
         var dirArray = System.IO.Directory.GetDirectories(path);
         var dirList = new List<string>(dirArray);
         if (!"/".Equals(path)) { dirList.Insert(0, ".."); }
         return dirList;
      }

      public static ArrayAdapter<String> GetAdaper(Context androidContext, string path)
      {
         var dirList = GetDirectories(path);
         var adapter = new StringsArrayAdaper(androidContext, Android.Resource.Layout.SelectDialogItem, Android.Resource.Id.Text1, dirList);
         return adapter;
      }

   }

}