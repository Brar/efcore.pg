using Microsoft.EntityFrameworkCore.TestModels.Northwind;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.Query;

public class NorthwindChangeTrackingQueryNpgsqlTest : NorthwindChangeTrackingQueryTestBase<NorthwindQueryNpgsqlFixture<NoopModelCustomizer>>
{
    // ReSharper disable once UnusedParameter.Local
    public NorthwindChangeTrackingQueryNpgsqlTest(
        NorthwindQueryNpgsqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    protected override NorthwindContext CreateNoTrackingContext()
        => new NorthwindRelationalContext(
            new DbContextOptionsBuilder(Fixture.CreateOptions())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options);
}