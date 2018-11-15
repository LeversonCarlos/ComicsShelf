using SQLite;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers.Database
{
   public class dbContext : IDisposable
   {
      public SQLiteConnection Connection { get; set; }

      public TableQuery<T> Table<T>() where T : new()
      { return this.Connection.Table<T>(); }

      public int Insert(object data)
      { return this.Connection.Insert(data); }

      public int Update(object data)
      { return this.Connection.Update(data); }

      public int Delete(object data)
      { return this.Connection.Delete(data); }

      #region Initialize
      public async Task Initialize()
      {
         try
         {
            var databasePath = App.Settings.Paths.DatabasePath;
            await Task.Run(() =>
            {

               this.Connection = new SQLiteConnection(databasePath);
               this.Connection.CreateTable<Configs>();
               // this.Connection.CreateTable<ComicFolder>();
               this.Connection.CreateTable<ComicFile>();
               this.Connection.CreateTable<Library>();
            });
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region Date
      public string GetDate()
      { return this.GetDate(DateTime.Now); }
      public string GetDate(DateTime dateTime)
      { return dateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
      #endregion

      #region Dispose
      public void Dispose()
      {
         if (this.Connection != null)
         {
            this.Connection.Close();
            this.Connection.Dispose();
            this.Connection = null;
         }
      }
      #endregion

   }
}