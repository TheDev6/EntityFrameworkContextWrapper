namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using EntityFrameworkContextWrapper;
    using FluentAssertions;
    using MyEfWrapperDb.DataAccess.Entities;
    using Xunit;

    public class DbContextWrapperUnitTests
    {

        [Fact]
        public void Constructor()
        {
            Action act = () => new DbContextWrapper<MyEfWrapperDbContext>();
            act.ShouldNotThrow("The sut should not fail on empty constructor");
        }

        [Fact]
        public void Constructor_Param()
        {
            var connString = "sup sql";
            var builder = new Func<MyEfWrapperDbContext>(() => new MyEfWrapperDbContext(connString));
            DbContextWrapper<MyEfWrapperDbContext> sut = null;
            Action act = () => { sut = new DbContextWrapper<MyEfWrapperDbContext>(builder); };
            var isLazyLoading = true;

            act.ShouldNotThrow("The sut should not fail on Context Builder constructor");
            sut.Should().NotBeNull();
            var result = sut.BuildContext(enableLazyLoading: isLazyLoading);
            result.Should().NotBeNull();
            result.Configuration.AutoDetectChangesEnabled.Should().Be(false);
            result.Configuration.LazyLoadingEnabled.Should().Be(isLazyLoading);
            result.Configuration.ProxyCreationEnabled.Should().Be(false);
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, false, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, true, false)]
        public void Call_Void(bool enableChangeTracking, bool enableLazyLoading, bool enableProxyCreation)
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            MyEfWrapperDbContext context = null;
            var callFunc = new Action<MyEfWrapperDbContext>(ctx => context = ctx);

            sut.Call(action: callFunc, enableChangeTracking: enableChangeTracking, enableLazyLoading: enableLazyLoading, enableProxyCreation: enableProxyCreation);

            context.Should().NotBeNull(because: "It should pass an instantiated Context Instance.");
            context.Configuration.AutoDetectChangesEnabled.Should().Be(enableChangeTracking);
            context.Configuration.LazyLoadingEnabled.Should().Be(enableLazyLoading);
            context.Configuration.ProxyCreationEnabled.Should().Be(enableProxyCreation);
        }

        [Fact]
        public void Call_Void_()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            MyEfWrapperDbContext context = null;
            var callFunc = new Action<MyEfWrapperDbContext>(ctx => context = ctx);

            sut.Call(callFunc);

            context.Should().NotBeNull(because: "It should pass an instantiated Context Instance.");
            context.Configuration.AutoDetectChangesEnabled.Should().Be(false);
            context.Configuration.LazyLoadingEnabled.Should().Be(false);
            context.Configuration.ProxyCreationEnabled.Should().Be(false);
        }

        [Fact]
        public void Call_Void_DisposedContext()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            MyEfWrapperDbContext context = null;
            var callFunc = new Action<MyEfWrapperDbContext>(ctx => context = ctx);
            var act = new Action(() =>
            {
                context.Customers.ToList();
            });
            sut.Call(callFunc);

            context.Should().NotBeNull(because: "It should pass an instantiated Context Instance.");
            context.Configuration.AutoDetectChangesEnabled.Should().Be(false);
            context.Configuration.LazyLoadingEnabled.Should().Be(false);
            context.Configuration.ProxyCreationEnabled.Should().Be(false);
            act.ShouldThrow<InvalidOperationException>(because: "The context should be disposed and throw an error.").WithMessage("The operation cannot be completed because the DbContext has been disposed.");

        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, false, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, true, false)]
        public async void CallAsync_Void(bool enableChangeTracking, bool enableLazyLoading, bool enableProxyCreation)
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            MyEfWrapperDbContext context = null;
            var callFunc = new Func<MyEfWrapperDbContext, Task>(ctx =>
            {
                return Task.Run(() => context = ctx);
            });

            await sut.CallAsync(callFunc, enableChangeTracking: enableChangeTracking, enableLazyLoading: enableLazyLoading, enableProxyCreation: enableProxyCreation);

            context.Should().NotBeNull(because: "It should pass an instantiated Context Instance.");
            context.Configuration.AutoDetectChangesEnabled.Should().Be(enableChangeTracking);
            context.Configuration.LazyLoadingEnabled.Should().Be(enableLazyLoading);
            context.Configuration.ProxyCreationEnabled.Should().Be(enableProxyCreation);
        }

        [Fact]
        public async void CallAsync_Void_()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            MyEfWrapperDbContext context = null;
            var callFunc = new Func<MyEfWrapperDbContext, Task>(ctx =>
            {
                return Task.Run(() => context = ctx);
            });

            await sut.CallAsync(func: callFunc);

            context.Should().NotBeNull(because: "It should pass an instantiated Context Instance.");
            context.Configuration.AutoDetectChangesEnabled.Should().Be(false);
            context.Configuration.LazyLoadingEnabled.Should().Be(false);
            context.Configuration.ProxyCreationEnabled.Should().Be(false);
        }

        [Fact]
        public void Call_T()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();

            var customer = new Customer() { CustomerGuid = Guid.NewGuid() };
            MyEfWrapperDbContext context = null;
            var func = new Func<MyEfWrapperDbContext, Customer>(ctx =>
            {
                context = ctx;
                return customer;
            });

            var result = sut.Call(func);

            result.Should().NotBeNull();
            result.CustomerGuid.Should().Be(customer.CustomerGuid);
            context.Should().NotBeNull();
        }

        [Fact]
        public async void CallAsync_T()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();

            var customer = new Customer() { CustomerGuid = Guid.NewGuid() };
            MyEfWrapperDbContext context = null;
            var func = new Func<MyEfWrapperDbContext, Task<Customer>>(async ctx =>
            {
                context = ctx;
                return await Task.Run(() => customer).ConfigureAwait(false);
            });

            var result = await sut.CallAsync(func);

            result.Should().NotBeNull();
            result.CustomerGuid.Should().Be(customer.CustomerGuid);
            context.Should().NotBeNull();
        }

        [Fact]
        public void ParamString()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var prms = new SqlParameter[]
            {
                new SqlParameter(parameterName: "MyFirstParam", value: 1),
                new SqlParameter(parameterName: "@MySecondParam", value: "2"),
                new SqlParameter(parameterName: "MyNullParam", value: DBNull.Value)
            };

            var result = sut.ParamString(prms);

            result.Should().Be(" @MyFirstParam ,@MySecondParam ,@MyNullParam");
        }

        [Fact]
        public void ParamString_OneParam()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var prms = new SqlParameter[]
            {
                new SqlParameter(parameterName: "MyFirstParam", value: 1),
            };

            var result = sut.ParamString(prms);

            result.Should().Be(" @MyFirstParam");
        }

        [Fact]
        public void CallQuerySingle_MissingParam()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var act = new Func<Customer>(() => sut.CallQuerySingle<Customer>(parameterizedSql: null));
            var isBlown = false;
            try
            {
                act.Invoke();
            }
            catch (Exception ex)
            {
                ex.Should().NotBeNull();
                isBlown = true;
            }
            isBlown.Should().Be(true, because: "Argument Null Exception should have been thrown");
        }

        [Fact]
        public void CallQuerySingleAsync_MissingParam()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            Func<Task<Customer>> act = async () => await sut.CallQuerySingleAsync<Customer>(parameterizedSql: null, parameters: null);
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void CallProc_MissingParam()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var act = new Action(() => sut.CallProc<Customer>(procedureName: null, parameters: null));
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void CallProcAsync_MissingParam()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var act = new Func<Task<List<Customer>>>(async () => await sut.CallProcAsync<Customer>(procedureName: null, parameters: null));
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void CallProcSingle_MissingParam()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var act = new Action(() => sut.CallProcSingle<Customer>(procedureName: null, parameters: null));
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void CallProcSingleAsync_MissingParam()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            Func<Task<Customer>> act = async () => await sut.CallProcSingleAsync<Customer>(procedureName: null, parameters: null);
            act.ShouldThrow<Exception>();
        }
    }
}
