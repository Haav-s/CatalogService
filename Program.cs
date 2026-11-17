using Repositories;
using Scalar.AspNetCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

var builder = WebApplication.CreateBuilder(args);

// Connect to Vault
var httpClientHandler = new HttpClientHandler();

httpClientHandler.ServerCertificateCustomValidationCallback =
    (message, cert, chain, errors) => true;

var authMethod = new TokenAuthMethodInfo(
    "00000000-0000-0000-0000-000000000000");

var vaultSettings = new VaultClientSettings(
    "https://localhost:8200",
    authMethod)
{
    MyHttpClientProviderFunc = handler =>
        new HttpClient(httpClientHandler)
};

IVaultClient vaultClient = new VaultClient(vaultSettings);

// Get Issuer and Secret from Vault
Secret<SecretData> jwtSecret =
    await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
        path: "jwt",
        mountPoint: "secret");

string myIssuer = jwtSecret.Data.Data["Issuer"].ToString()!;
string mySecret = jwtSecret.Data.Data["Secret"].ToString()!;


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = myIssuer,
            ValidAudience = "http://localhost",
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mySecret))
        };
    });

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// MongoDB Guid-format (skal sættes før første brug af MongoClient)
MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(
    new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));

builder.Services.AddSingleton<IProductRepository, ProductRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();