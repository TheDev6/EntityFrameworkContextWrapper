namespace EntityFrameworkContextWrapper
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public interface IDbContextWrapper<TContext> where TContext : DbContext, new()
    {
        TContext BuildContext(bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false);
        void Call(Action<TContext> action, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false);
        T Call<T>(Func<TContext, T> func, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false);
        Task CallAsync(Func<TContext, Task> func, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false);
        Task<T> CallAsync<T>(Func<TContext, Task<T>> func, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false);
        void CallWithTransaction(Action<TContext> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false);
        T CallWithTransaction<T>(Func<TContext, T> func, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false);
        Task CallWithTransactionAsync(Func<TContext, Task> func, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false);
        Task<T> CallWithTransactionAsync<T>(Func<TContext, Task<T>> func, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false);
    }
}