namespace BobTheDiceMaster.Api
{
  public class Startup
  {
    private const string corsPolicyName = "MyPolicy";

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();
      services.AddCors(o => o.AddPolicy(corsPolicyName, builder =>
      {
        builder
          .AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
      }));
      services.AddSwaggerGen(c =>
      {
        // Required to mark nullable properties as nullable as described at
        // https://stackoverflow.com/questions/71299450/make-swashbuckle-describe-a-reference-type-property-as-nullable-or-make-nswag-d.
        c.UseAllOfToExtendReferenceSchemas();
      }); 
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseCors(corsPolicyName);

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapGet("/", async context =>
          {
            await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
          });
      });
    }
  }
}