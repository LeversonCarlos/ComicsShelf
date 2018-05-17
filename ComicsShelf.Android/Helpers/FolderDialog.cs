using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   public class FolderDialog : IDisposable
   {

      private const string rSelectFolder = "Select folder";
      private const string rCommandOK = "OK";
      private const string rCommandCancel = "Cancel";

      #region New
      public FolderDialog(Context androidContext)
      {
         this._androidContext = androidContext;
      }
      #endregion

      #region Properties
      Context _androidContext;
      AlertDialog _dialog;
      TextView _currentPathView;
      ListView _directoryListView;
      AutoResetEvent _autoResetEvent;
      #endregion

      #region GetDirectoryAsync
      public static async Task<string> GetDirectoryAsync(string initialPath)
      {
         try
         {
            using (var folderDialog = new FolderDialog(Xamarin.Forms.Forms.Context))
            {

               if (string.IsNullOrEmpty(initialPath))
               { initialPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath; }
               initialPath = new Java.IO.File(initialPath).CanonicalPath;

               if (!folderDialog.CreateDialog()) { return string.Empty; }
               folderDialog.CurrentPath = initialPath;
               if (!await folderDialog.AwaitSelection()) { return string.Empty; }

               return folderDialog.ResultPath;
            }
         }
         catch (Exception ex) { return string.Empty; }
      }
      #endregion

      #region CreateDialog
      private bool CreateDialog()
      {
         try
         {

            // INITIALIZE
            var viewPadding = this.ConvertDpToPx(10);
            AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(_androidContext);
            dialogBuilder.SetTitle(rSelectFolder);

            // CURRENT PATH
            _currentPathView = new TextView(_androidContext);
            _currentPathView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            _currentPathView.SetPadding(viewPadding, viewPadding, viewPadding, viewPadding);
            _currentPathView.SetTextColor(Color.Black);
            _currentPathView.Gravity = GravityFlags.Top;
            _currentPathView.SetTextSize(Android.Util.ComplexUnitType.Dip, 20);
            _currentPathView.SetTypeface(null, TypefaceStyle.Bold);

            // DIRECTORY LIST VIEW
            _directoryListView = new ListView(_androidContext);
            _directoryListView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            _directoryListView.SetPadding(viewPadding * 2, viewPadding, viewPadding, viewPadding);
            _directoryListView.SetForegroundGravity(GravityFlags.FillVertical);

            // DIRECTORY SELECTION
            _directoryListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
               string selectedPath = "" + _directoryListView.Adapter.GetItem(e.Position);
               if (selectedPath =="..")
               {
                  this.CurrentPath = this.CurrentPath.Substring(0, this.CurrentPath.LastIndexOf("/"));
                  if (this.CurrentPath == "") { this.CurrentPath = "/"; }
               }
               else { this.CurrentPath += $"/{selectedPath}"; }
            };

            // LAYOUT VIEW
            var layoutView = new LinearLayout(_androidContext);
            layoutView.Orientation = Orientation.Vertical;
            layoutView.AddView(_currentPathView);
            layoutView.AddView(_directoryListView);
            // _listView.Adapter = StringsArrayAdaper.GetAdaper(_AndroidContext, this._initialPath);
            dialogBuilder.SetView(layoutView);
           
            // DEFINE COMMANDS
            dialogBuilder.SetCancelable(true);
            dialogBuilder.SetPositiveButton(rCommandOK, (sender, args) => {
               this.ResultPath = this.CurrentPath;
               this._autoResetEvent.Set();
            });
            dialogBuilder.SetNegativeButton(rCommandCancel, (sender, args) => { });

            // CREATE DIALOG
            _dialog = dialogBuilder.Create();
            _dialog.CancelEvent += (sender, args) => { this._autoResetEvent.Set(); };
            _dialog.DismissEvent += (sender, args) => { this._autoResetEvent.Set(); };

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
            return (!string.IsNullOrEmpty(this.ResultPath));
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region CurrentPath
      public string ResultPath { get; set; }

      string _CurrentPath;
      public string CurrentPath
      {
         get { return _CurrentPath; }
         set
         {
            _CurrentPath = value;
            _currentPathView.Text = value;
            _directoryListView.Adapter = StringsArrayAdaper.GetAdaper(_androidContext, value);
         }
      }
      #endregion

      #region ConvertDpToPx
      private int ConvertDpToPx(int dp)
      {
         var r = _androidContext.Resources;
         var metrics = r.DisplayMetrics;
         int px = (int)Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Dip, dp, metrics);
         return px;
      }
      #endregion

      #region Dispose
      public void Dispose()
      {
         if (_currentPathView != null) { _currentPathView.Dispose(); _currentPathView = null; }
         if (_directoryListView != null) { _directoryListView.Dispose(); _directoryListView = null; }
         if (_dialog != null) { _dialog.Dispose(); _dialog = null; }
         // if (_autoResetEvent != null) { _autoResetEvent.Dispose(); _autoResetEvent = null; }
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
         var dirList = new List<string>();

         var dirFile = new Java.IO.File(path);
         var dirFiles = dirFile.ListFiles();

         if (dirFiles != null)
         {
            dirList = dirFiles
               .Where(x => x.IsDirectory)
               .Select(x => x.Name)
               .OrderBy(x => x)
               .ToList();           
         }

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