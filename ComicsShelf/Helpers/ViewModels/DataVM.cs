namespace ComicsShelf.Helpers.ViewModels
{
   public class DataVM<T> : NavVM
   {

      public T Data { get; protected set; }

      public override void Dispose()
      {
         base.Dispose();
         this.Data = default(T);
      }

   }
}