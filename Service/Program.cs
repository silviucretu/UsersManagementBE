using Casbin;
using Casbin.Persist.Adapter.File;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSingleton<IClaimsTransformation, ZitadelRoleClaimsTransformer>();


builder.Services.AddSingleton<IEnforcer>(provider =>
{
    var model = Casbin.Model.DefaultModel.Create();
    model.LoadModelFromFile("Casbin/model.conf");

    // Create file adapter
    var adapter = new FileAdapter("Casbin/policy.csv");

    // Create enforcer
    var enforcer = new Enforcer(model, adapter);

    return enforcer;
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;  //JUST FOR DEV WITH NO SSL!!
        options.Authority = "http://localhost:8080";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            NameClaimType = "name",
            RoleClaimType = "role"
        };

        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
               HttpClientHandler.DangerousAcceptAnyServerCertificateValidator //JUST FOR DEV WITH NO SSL!!
        };
    });


builder.Services.AddScoped<PolicyManager>();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseCasbinAuthorization();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();

// Disable CORS since angular will be running on port 4200 and the service on port 5258.  // JUST FOR DEV WITH NO SSL!!
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.Run();
