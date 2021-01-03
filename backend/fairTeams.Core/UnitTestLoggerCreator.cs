using Microsoft.Extensions.Logging;

namespace fairTeams.Core
{
    public static class UnitTestLoggerCreator
    {
        public static ILogger<T> CreateUnitTestLogger<T>() where T : class
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            return loggerFactory.CreateLogger<T>();
        }

        public static ILoggerFactory CreateUnitTestLoggerFactory()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            return loggerFactory;
        }
    }
}
