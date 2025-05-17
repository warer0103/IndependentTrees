using IndependentTrees.API.DataStorage;
using IndependentTrees.API.Middlewares;
using IndependentTrees.API.ModelConvention;
using IndependentTrees.API.Providers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var dataStorage = DataStorageFactory.Create(builder.Configuration); 

builder.Services
    .AddTransient<ErrorHandlerMiddleware>()
    .AddSingleton<IDataStorage>(dataStorage)
    .AddSingleton<IDateTimeProvider, DateTimeProvider>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(c =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath, true);
    });

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new CamelCaseControllerNamesConvention());
});

var app = builder.Build();
app.UseMiddleware<ErrorHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
