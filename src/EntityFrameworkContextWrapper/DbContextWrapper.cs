namespace EntityFrameworkContextWrapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Linq;

    public class DbContextWrapper<TContext> : IDbContextWrapper<TContext> where TContext : DbContext, new()
    {
        private readonly Func<TContext> _contextBuilder;
        private readonly Action<string> _logListener;

        public DbContextWrapper()
        {

        }

        public DbContextWrapper(Func<TContext> contextBuilder = null, Action<string> logListener = null)
        {
            this._contextBuilder = contextBuilder;
            this._logListener = logListener;
        }

        public virtual TContext BuildContext(bool enableChangeTracking = false, bool enableLazyLoading = false, bool enableProxyCreation = false)
        {
            TContext result = this._contextBuilder?.Invoke() ?? new TContext();
            if (this._logListener != null)
            {   
                result.Database.Log = this._logListener;
            }
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

        public virtual T CallQuerySingle<T>(string parameterizedSql, SqlParameter[] parameters = null) where T : class, new()
        {
            if (parameterizedSql == null)
            {
                throw new ArgumentNullException(nameof(parameterizedSql));
            }

            var result = this.Call(ctx => ctx.Database.SqlQuery<T>(sql: parameterizedSql, parameters: parameters).SingleOrDefault());
            return result;
        }

        public virtual async Task<T> CallQuerySingleAsync<T>(string parameterizedSql, SqlParameter[] parameters = null) where T : class, new()
        {
            var result = await this.CallAsync(ctx => ctx.Database.SqlQuery<T>(sql: parameterizedSql, parameters: parameters).SingleOrDefaultAsync()).ConfigureAwait(false);
            return result;
        }

        public virtual List<T> CallQuery<T>(string parameterizedSql, SqlParameter[] parameters = null)
        {
            var result = this.Call(ctx => ctx.Database.SqlQuery<T>(sql: parameterizedSql, parameters: parameters).ToList());
            return result;
        }

        public virtual async Task<List<T>> CallQueryAsync<T>(string parameterizedSql, SqlParameter[] parameters = null)
        {
            var result = await this.CallAsync(ctx => ctx.Database.SqlQuery<T>(sql: parameterizedSql, parameters: parameters).ToListAsync());
            return result;
        }


        public virtual T CallProcSingle<T>(string procedureName, SqlParameter[] parameters = null)
        {
            if (String.IsNullOrWhiteSpace(procedureName))
            {
                throw new ArgumentNullException(nameof(procedureName));
            }

            var result = this.Call(ctx => parameters == null
                ? ctx.Database.SqlQuery<T>(sql: procedureName).SingleOrDefault()
                : ctx.Database.SqlQuery<T>(sql: $"{procedureName}{ParamString(parameters)}", parameters: parameters).SingleOrDefault());

            return result;
        }

        public virtual async Task<T> CallProcSingleAsync<T>(string procedureName, SqlParameter[] parameters = null)
        {
            if (String.IsNullOrWhiteSpace(procedureName))
            {
                throw new ArgumentNullException(nameof(procedureName));
            }

            var result = await this.CallAsync(ctx => parameters == null
                    ? ctx.Database.SqlQuery<T>(sql: procedureName).SingleOrDefaultAsync()
                    : ctx.Database.SqlQuery<T>(sql: $"{procedureName}{ParamString(parameters)}", parameters: parameters).SingleOrDefaultAsync())
                .ConfigureAwait(false);

            return result;
        }

        public virtual List<T> CallProc<T>(string procedureName, SqlParameter[] parameters = null)
        {
            if (String.IsNullOrWhiteSpace(procedureName))
            {
                throw new ArgumentNullException(nameof(procedureName));
            }

            var result = this.Call(ctx => parameters == null
                ? ctx.Database.SqlQuery<T>(sql: procedureName).ToList()
                : ctx.Database.SqlQuery<T>(sql: $"{procedureName}{ParamString(parameters)}", parameters: parameters).ToList());

            return result;
        }

        public virtual async Task<List<T>> CallProcAsync<T>(string procedureName, SqlParameter[] parameters = null)
        {
            if (String.IsNullOrWhiteSpace(procedureName))
            {
                throw new ArgumentNullException(nameof(procedureName));
            }

            var result = await this.CallAsync(ctx => parameters == null
                ? ctx.Database.SqlQuery<T>(sql: procedureName).ToListAsync()
                : ctx.Database.SqlQuery<T>(sql: $"{procedureName}{ParamString(parameters)}", parameters: parameters).ToListAsync());

            return result;
        }

        public virtual string ParamString(SqlParameter[] parameters)
        {
            var result = string.Empty;
            if (parameters != null)
            {
                var comma = "";
                foreach (var p in parameters)
                {
                    var at = p.ParameterName.StartsWith("@") ? "" : "@";
                    result += $" {comma}{at}{p.ParameterName}";
                    comma = ",";
                }
            }
            return result;
        }
    }
}