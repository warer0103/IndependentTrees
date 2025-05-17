using IndependentTrees.API.DataStorage.EF;
using Microsoft.EntityFrameworkCore;

namespace IndependentTrees.API.DataStorage
{
    public static class DataStorageFactory
    {
        public static IDataStorage Create(ConfigurationManager configurationManager) 
        {
            var dbContextOptionBuilder = new DbContextOptionsBuilder();
            
            var connectionString = configurationManager.GetConnectionString("PostgreSqlConnection");
            if (!string.IsNullOrWhiteSpace(connectionString))
                return CreatePostgreSqlEFDataSorage(dbContextOptionBuilder, connectionString);

            connectionString = configurationManager.GetConnectionString("MsSqlConnection");
            if (!string.IsNullOrWhiteSpace(connectionString))
                return CreateMsSqlEFDataSorage(dbContextOptionBuilder, connectionString);

            throw new NotImplementedException();
        }

        private static EFDataStorage CreateMsSqlEFDataSorage(DbContextOptionsBuilder builder, string connectionString) =>
            CreateEFDataStorage(
                builder
                    .UseSqlServer(connectionString)
                    .Options);

        private static EFDataStorage CreatePostgreSqlEFDataSorage(DbContextOptionsBuilder builder, string connectionString) =>
            CreateEFDataStorage(
                builder
                    .UseNpgsql(connectionString)
                    .Options);

        private static EFDataStorage CreateEFDataStorage(DbContextOptions dbContextOptions)
        {
            var dataStorage = new EFDataStorage(dbContextOptions);
            dataStorage.Init();
            return dataStorage;
        }
    }
}
