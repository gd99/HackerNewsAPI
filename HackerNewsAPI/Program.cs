using HackerNewsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var maxPoolSize = builder.Configuration.GetValue<int>("HttpClient:MaxSocketPoolSize");


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHackerNewsManager, HackerNewsManager>();
builder.Services.AddHttpClient("HackerNews").ConfigureHttpMessageHandlerBuilder(builder =>
{ 
    builder.PrimaryHandler = new SocketsHttpHandler { MaxConnectionsPerServer = maxPoolSize };
    builder.Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
