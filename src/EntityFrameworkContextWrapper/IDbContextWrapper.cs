namespace EntityFrameworkContextWrapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.SqlClient;
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
        List<T> CallProc<T>(string procedureName, SqlParameter[] parameters = null);
        Task<List<T>> CallProcAsync<T>(string procedureName, SqlParameter[] parameters = null);
        List<T> CallQuery<T>(string parameterizedSql, SqlParameter[] parameters = null);
        Task<List<T>> CallQueryAsync<T>(string parameterizedSql, SqlParameter[] parameters = null);
        T CallQuerySingle<T>(string parameterizedSql, SqlParameter[] parameters = null) where T : class, new();
        Task<T> CallQuerySingleAsync<T>(string parameterizedSql, SqlParameter[] parameters = null) where T : class, new();
        T CallProcSingle<T>(string procedureName, SqlParameter[] parameters = null);
        Task<T> CallProcSingleAsync<T>(string procedureName, SqlParameter[] parameters = null);
    }
}