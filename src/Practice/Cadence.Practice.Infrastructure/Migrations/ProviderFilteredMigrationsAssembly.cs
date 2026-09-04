using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace Cadence.Practice.Infrastructure.Migrations
{
#pragma warning disable EF1001
    public sealed class ProviderFilteredMigrationsAssembly : MigrationsAssembly
    {
        private const string PostgresNamespaceMarker = ".Migrations.Postgres";

        private readonly ICurrentDbContext _currentDbContext;

        public ProviderFilteredMigrationsAssembly(
            ICurrentDbContext currentDbContext,
            IDbContextOptions options,
            IMigrationsIdGenerator idGenerator,
            IDiagnosticsLogger<DbLoggerCategory.Migrations> logger)
            : base(currentDbContext, options, idGenerator, logger)
        {
            _currentDbContext = currentDbContext;
        }

        public override IReadOnlyDictionary<string, TypeInfo> Migrations
        {
            get
            {
                bool isPostgres = CurrentProviderIsPostgres();

                return base.Migrations
                    .Where(pair => IsPostgresNamespace(pair.Value.Namespace) == isPostgres)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }

        public override ModelSnapshot? ModelSnapshot
        {
            get
            {
                bool isPostgres = CurrentProviderIsPostgres();

                return Assembly.DefinedTypes
                    .Where(type => !type.IsAbstract
                        && type.IsSubclassOf(typeof(ModelSnapshot))
                        && IsPostgresNamespace(type.Namespace) == isPostgres)
                    .Select(type => (ModelSnapshot)Activator.CreateInstance(type.AsType())!)
                    .FirstOrDefault();
            }
        }

        private bool CurrentProviderIsPostgres() =>
            _currentDbContext.Context.Database.ProviderName?
                .Contains("Npgsql", StringComparison.OrdinalIgnoreCase) ?? false;

        private static bool IsPostgresNamespace(string? namespaceName) =>
            namespaceName?.Contains(PostgresNamespaceMarker, StringComparison.Ordinal) ?? false;
    }
#pragma warning restore EF1001
}
