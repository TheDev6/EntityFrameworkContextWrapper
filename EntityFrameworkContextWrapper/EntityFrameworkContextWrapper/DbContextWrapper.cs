namespace EntityFrameworkContextWrapper
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using System.Data.Entity;

    public class DbContextWrapper<TContext> : IDbContextWrapper<TContext> where TContext : DbContext, new()
    {
        public virtual TContext BuildContext(bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false)
        {
            //Just override this method in a child class to modify this behavior
            TContext result = new TContext();
            result.Configuration.AutoDetectChangesEnabled = enableChangeTracking;
            result.Configuration.LazyLoadingEnabled = enableLazyLoading;
            result.Configuration.ProxyCreationEnabled = enableProxyCreation;
            return result;
        }

        public virtual T Call<T>(Func<TContext, T> func, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false)
        {
            using (var context = this.BuildContext(enableChangeTracking: enableChangeTracking, enableLazyLoading: enableLazyLoading, enableProxyCreation: enableProxyCreation))
            {
                return func(context);
            }
        }

        public virtual void Call(Action<TContext> action, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false)
        {
            using (var context = this.BuildContext(enableChangeTracking: enableChangeTracking, enableLazyLoading: enableLazyLoading, enableProxyCreation: enableProxyCreation))
            {
                action(context);
            }
        }

        public virtual async Task<T> CallAsync<T>(Func<TContext, Task<T>> func, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false)
        {
            using (var context = this.BuildContext(enableChangeTracking: enableChangeTracking, enableLazyLoading: enableLazyLoading, enableProxyCreation: enableProxyCreation))
            {
                return await func(context).ConfigureAwait(false);
            }
        }

        public virtual async Task CallAsync(Func<TContext, Task> func, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false)
        {
            using (var context = this.BuildContext(enableChangeTracking: enableChangeTracking, enableLazyLoading: enableLazyLoading, enableProxyCreation: enableProxyCreation))
            {
                await func(context).ConfigureAwait(false);
            }
        }

        public virtual T CallWithTransaction<T>(Func<TContext, T> func, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false)
        {
            using (var context = this.BuildContext(enableChangeTracking: enableChangeTracking, enableLazyLoading: enableLazyLoading, enableProxyCreation: enableProxyCreation))
            {
                using (var tx = context.Database.BeginTransaction(isolationLevel: isolationLevel))
                {
                    var result = func(context);
                    tx.Commit();
                    return result;
                }
            }
        }

        public virtual void CallWithTransaction(Action<TContext> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false)
        {
            using (var context = this.BuildContext(enableChangeTracking: enableChangeTracking, enableLazyLoading: enableLazyLoading, enableProxyCreation: enableProxyCreation))
            {
                using (var tx = context.Database.BeginTransaction(isolationLevel: isolationLevel))
                {
                    action(context);
                    tx.Commit();
                }
            }
        }

        public virtual async Task<T> CallWithTransactionAsync<T>(Func<TContext, Task<T>> func, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false)
        {
            using (var context = this.BuildContext(enableChangeTracking: enableChangeTracking, enableLazyLoading: enableLazyLoading, enableProxyCreation: enableProxyCreation))
            {
                using (var tx = context.Database.BeginTransaction(isolationLevel: isolationLevel))
                {
                    var result = await func(context).ConfigureAwait(false);
                    tx.Commit();
                    return result;
                }
            }
        }

        public virtual async Task CallWithTransactionAsync(Func<TContext, Task> func, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false)
        {
            using (var context = this.BuildContext(enableChangeTracking: enableChangeTracking, enableLazyLoading: enableLazyLoading, enableProxyCreation: enableProxyCreation))
            {
                using (var tx = context.Database.BeginTransaction(isolationLevel: isolationLevel))
                {
                    await func(context).ConfigureAwait(false);
                    tx.Commit();
                }
            }
        }

    }
}