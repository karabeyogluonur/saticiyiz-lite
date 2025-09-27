using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using SL.Persistence.Contexts;
using SL.Persistence.Seeds;
using SL.Web.Mvc.Middlewares;

namespace SL.Web.Mvc
{
    public static class BuilderRegistration
    {
        public static void AddBaseBuilder(this WebApplication application)
        {
            application.UseHttpsRedirection();
            application.UseStaticFiles();
        }
        public static void AddDevelopmentBuilder(this WebApplication application)
        {
            if (!application.Environment.IsDevelopment())
            {
                application.UseExceptionHandler("/Home/Error");
                application.UseHsts();
            }
        }
        public static void AddRouteBuilder(this WebApplication app)
        {
            app.UseRouting();
        }
        public static void AddAuthBuilder(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<TenantMiddleware>();

        }
        public static void UseSpecialRoute(this WebApplication app)
        {
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
        public async static Task DatabaseSeedAsync(this WebApplication app)
        {
            await DbInitializer.InitializeAsync(app);
        }
        public static void AddSerilogConfigurations(this WebApplicationBuilder? builder)
        {
            Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine($"[Serilog Internal HATA] {msg}"));
            ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
            string connectionString = builder.Configuration.GetConnectionString("LogConnection");

            using (var scope = serviceProvider.CreateScope())
            {
                var logConnectionString = connectionString;

                var optionsBuilder = new DbContextOptionsBuilder<LogDbContext>();
                optionsBuilder.UseNpgsql(logConnectionString);

                using (var dbContext = new LogDbContext(optionsBuilder.Options))
                {
                    dbContext.Database.EnsureCreated();
                    Console.WriteLine("SL_Log veritabanı başarıyla var edildi.");
                }
            }

            Dictionary<string, ColumnWriterBase> columnOptions = new Dictionary<string, ColumnWriterBase>
            {
                { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
                { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                { "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
                { "tenant_id", new SinglePropertyColumnWriter("TenantId", PropertyWriteMethod.Raw, NpgsqlDbType.Uuid)},
                { "properties", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
                { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.Raw, NpgsqlDbType.Text) },
                { "environment_user", new SinglePropertyColumnWriter("EnvironmentUserName", PropertyWriteMethod.Raw, NpgsqlDbType.Text) },
                { "process_id", new SinglePropertyColumnWriter("ProcessId", PropertyWriteMethod.Raw, NpgsqlDbType.Integer) },
                { "thread_id", new SinglePropertyColumnWriter("ThreadId", PropertyWriteMethod.Raw, NpgsqlDbType.Integer) }
            };

            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .WriteTo.Console()
            .WriteTo.PostgreSQL(
                connectionString: connectionString,
                tableName: "logs",
                columnOptions: columnOptions,
                needAutoCreateTable: true,
                batchSizeLimit: 50,
                period: TimeSpan.FromSeconds(5))
            .MinimumLevel.Information()
            .CreateLogger();

            builder.Host.UseSerilog();

        }
    }
}

