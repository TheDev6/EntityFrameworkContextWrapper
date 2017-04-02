namespace IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using EntityFrameworkContextWrapper;
    using FluentAssertions;
    using MyEfWrapperDb.DataAccess.Entities;
    using Xunit;

    public class DbContextWrapper_IntegrationTests
    {
        [Fact]
        public void CallQuerySingle()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            sut.Call(action: ctx =>
            {
                ctx.Customers.Add(customer);
                ctx.SaveChanges();
            });
            var paramz = new SqlParameter[] { new SqlParameter("CustomerGuid", customer.CustomerGuid) };

            var result = sut.CallQuerySingle<Customer>(
                parameterizedSql: "SELECT c.CustomerGuid, c.Name FROM Customer c WHERE c.CustomerGuid = @CustomerGuid",
                parameters: paramz);

            result.Should().NotBeNull();
            result.ShouldBeEquivalentTo(customer);
        }

        [Fact]
        public async void CallQuerySingleAsync()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            sut.Call(action: ctx =>
            {
                ctx.Customers.Add(customer);
                ctx.SaveChanges();
            });
            var paramz = new SqlParameter[] { new SqlParameter("CustomerGuid", customer.CustomerGuid) };

            var result = await sut.CallQuerySingleAsync<Customer>(
                parameterizedSql: "SELECT c.CustomerGuid, c.Name FROM Customer c WHERE c.CustomerGuid = @CustomerGuid",
                parameters: paramz).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.ShouldBeEquivalentTo(customer);
        }



        [Fact]
        public void CallQuery()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            sut.Call(action: ctx =>
            {
                ctx.Customers.Add(customer);
                ctx.SaveChanges();
            });
            var paramz = new SqlParameter[] { new SqlParameter("CustomerGuid", customer.CustomerGuid) };

            var result = sut.CallQuery<Customer>(
                parameterizedSql: "SELECT c.CustomerGuid, c.Name FROM Customer c WHERE c.CustomerGuid = @CustomerGuid",
                parameters: paramz);

            result.Should().NotBeNull();
            result[0].CustomerGuid.Should().Be(customer.CustomerGuid);
        }

        [Fact]
        public async void CallQueryAsync()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            sut.Call(action: ctx =>
            {
                ctx.Customers.Add(customer);
                ctx.SaveChanges();
            });
            var paramz = new SqlParameter[] { new SqlParameter("CustomerGuid", customer.CustomerGuid) };

            var result = await sut.CallQueryAsync<Customer>(
                parameterizedSql: "SELECT c.CustomerGuid, c.Name FROM Customer c WHERE c.CustomerGuid = @CustomerGuid",
                parameters: paramz).ConfigureAwait(false);

            result.Should().NotBeNull();
            result[0].CustomerGuid.Should().Be(customer.CustomerGuid);
        }

        [Fact]
        public void CallProcSingle()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            sut.Call(action: ctx =>
            {
                ctx.Customers.Add(customer);
                ctx.SaveChanges();
            });
            var paramz = new SqlParameter[]
                {new SqlParameter(parameterName: "CustomerGuid", value: customer.CustomerGuid)};

            var result = sut.CallProcSingle<Customer>(procedureName: "CustomerSelect", parameters: paramz);

            result.Should().NotBeNull();
            result.ShouldBeEquivalentTo(customer);
        }

        [Fact]
        public void CallProcSingle_NoParams()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            sut.Call(action: ctx =>
            {
                ctx.Customers.Add(customer);
                ctx.SaveChanges();
            });

            var result = sut.CallProcSingle<Customer>(procedureName: "CustomerSelectTopOne");

            result.Should().NotBeNull();
        }

        [Fact]
        public async void CallProcSingleAsync()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            await sut.CallAsync(func: async ctx =>
            {
                ctx.Customers.Add(customer);
                await ctx.SaveChangesAsync();
            });
            var paramz = new SqlParameter[]
                {new SqlParameter(parameterName: "CustomerGuid", value: customer.CustomerGuid)};

            var result = await sut.CallProcSingleAsync<Customer>(procedureName: "CustomerSelect", parameters: paramz);

            result.Should().NotBeNull();
            result.ShouldBeEquivalentTo(customer);
        }

        [Fact]
        public async void CallProcSingleAsync_NoParams()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            sut.Call(action: ctx =>
            {
                ctx.Customers.Add(customer);
                ctx.SaveChanges();
            });

            var result =
                await sut.CallProcSingleAsync<Customer>(procedureName: "CustomerSelectTopOne").ConfigureAwait(false);

            result.Should().NotBeNull();
        }

        [Fact]
        public void CallProc()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            var customer2 = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            sut.Call(action: ctx =>
            {
                ctx.Customers.Add(customer);
                ctx.Customers.Add(customer2);
                ctx.SaveChanges();
            });

            var result = sut.CallProc<Customer>(procedureName: "CustomerSelectAll");

            result.Should().NotBeNull();
            result?.Count.Should().BeGreaterOrEqualTo(2);
            result.Any(r => r.CustomerGuid == customer2.CustomerGuid).Should().BeTrue();
        }

        [Fact]
        public async void CallProcAsync()
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            var customer2 = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            sut.Call(action: ctx =>
            {
                ctx.Customers.Add(customer);
                ctx.Customers.Add(customer2);
                ctx.SaveChanges();
            });

            var result = await sut.CallProcAsync<Customer>(procedureName: "CustomerSelectAll");

            result.Should().NotBeNull();
            result?.Count.Should().BeGreaterOrEqualTo(2);
            result.Any(r => r.CustomerGuid == customer2.CustomerGuid).Should().BeTrue();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CallWithTransaction_Void(bool throwErrorBeforeCommit)
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            var action = new Action<MyEfWrapperDbContext>((ctx) =>
            {
                ctx.Customers.Add(customer);
                ctx.SaveChanges();
                if (throwErrorBeforeCommit)
                {
                    throw new Exception("Boom");
                }
            });

            try
            {
                sut.CallWithTransaction(action: action);
            }
            catch (Exception ex)
            {
                ex.Should().NotBeNull();
            }

            var savedCustomer = sut.Call(ctx => ctx.Customers.SingleOrDefault(c => c.CustomerGuid == customer.CustomerGuid));
            if (throwErrorBeforeCommit)
            {
                savedCustomer.Should().BeNull(because: "no customer record should be inserted when transaction is not committed via an exception.");
            }
            else
            {
                savedCustomer.Should().NotBeNull(because: "a customer record should be inserted when transaction is committed.");
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void CallWithTransactionAsync_Task(bool throwErrorBeforeCommit)
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            var func = new Func<MyEfWrapperDbContext, Task>(async (ctx) =>
            {
                ctx.Customers.Add(customer);
                await ctx.SaveChangesAsync();
                if (throwErrorBeforeCommit)
                {
                    throw new Exception("Boom");
                }
            });

            try
            {
                await sut.CallWithTransactionAsync(func: func);
            }
            catch (Exception ex)
            {
                ex.Should().NotBeNull();
            }

            var savedCustomer = await sut.CallAsync(async ctx => await ctx.Customers.SingleOrDefaultAsync(c => c.CustomerGuid == customer.CustomerGuid)).ConfigureAwait(false);
            if (throwErrorBeforeCommit)
            {
                savedCustomer.Should().BeNull(because: "no customer record should be inserted when transaction is not committed via an exception.");
            }
            else
            {
                savedCustomer.Should().NotBeNull(because: "a customer readon should be inserted when transaction is committed.");
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CallWithTransaction_T(bool throwErrorBeforeCommit)
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            var func = new Func<MyEfWrapperDbContext, Customer>((ctx) =>
             {
                 ctx.Customers.Add(customer);
                 ctx.SaveChanges();
                 if (throwErrorBeforeCommit)
                 {
                     throw new Exception("Boom");
                 }
                 return customer;
             });

            try
            {
                sut.CallWithTransaction(func: func);
            }
            catch (Exception ex)
            {
                ex.Should().NotBeNull();
            }

            var savedCustomer = sut.Call(ctx => ctx.Customers.SingleOrDefault(c => c.CustomerGuid == customer.CustomerGuid));
            if (throwErrorBeforeCommit)
            {
                savedCustomer.Should().BeNull(because: "no customer record should be inserted when transaction is not committed via an exception.");
            }
            else
            {
                savedCustomer.Should().NotBeNull(because: "a customer record should be inserted when transaction is committed.");
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void CallWithTransactionAsync_T(bool throwErrorBeforeCommit)
        {
            var sut = new DbContextWrapper<MyEfWrapperDbContext>();
            var customer = new Customer() { CustomerGuid = Guid.NewGuid(), Name = Guid.NewGuid().ToString("n") };
            var func = new Func<MyEfWrapperDbContext, Task<Customer>>(async (ctx) =>
            {
                ctx.Customers.Add(customer);
                await ctx.SaveChangesAsync();
                if (throwErrorBeforeCommit)
                {
                    throw new Exception("Boom");
                }
                return customer;
            });

            try
            {
                await sut.CallWithTransactionAsync(func: func);
            }
            catch (Exception ex)
            {
                ex.Should().NotBeNull();
            }

            var savedCustomer = await sut.CallAsync(async ctx => await ctx.Customers.SingleOrDefaultAsync(c => c.CustomerGuid == customer.CustomerGuid)).ConfigureAwait(false);
            if (throwErrorBeforeCommit)
            {
                savedCustomer.Should().BeNull(because: "no customer record should be inserted when transaction is not committed via an exception.");
            }
            else
            {
                savedCustomer.Should().NotBeNull(because: "a customer readon should be inserted when transaction is committed.");
            }
        }

        [Fact]
        public void ConstructorWithLoggerBuilder()
        {
            var builder = new Func<MyEfWrapperDbContext>(() => new MyEfWrapperDbContext());
            var logs = new List<string>();
            var logger = new Action<string>((s) => logs.Add(s));

            var sut = new DbContextWrapper<MyEfWrapperDbContext>(contextBuilder: builder, logListener: logger);

            var result = sut.CallProcSingle<Customer>(procedureName: "CustomerSelect", parameters: new SqlParameter[] { new SqlParameter("CustomerGuid", Guid.NewGuid()), });

            result.Should().BeNull();
            logs.Count.Should().BeGreaterThan(0);
        }
    }
}
