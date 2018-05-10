using SQLite;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Database
{
   internal class Connector : IDisposable
   {

      #region Connector
      public SQLiteConnection Connection { get; set; }
      public async Task InitializeConnector(string path)
      {
         try
         {
            await Task.Run(() =>
            {
               this.Connection = new SQLiteConnection(path);
               this.Connection.CreateTable<Configs>();
               this.Connection.CreateTable<ComicFolders>();
               this.Connection.CreateTable<ComicFiles>();
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

      public TableQuery<T> Table<T>() where T : new()
      { return this.Connection.Table<T>(); }

      public int Insert(object data)
      { return this.Connection.Insert(data); }

      public int Update(object data)
      { return this.Connection.Update(data); }

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