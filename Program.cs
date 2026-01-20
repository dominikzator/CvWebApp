using CvWebApp.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

builder.Services.AddDbContext<MainDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Headers.ContainsKey("X-MS-CLIENT-PRINCIPAL-ID"))
    {
        var azureAppServicePrincipalIdHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"].FirstOrDefault();
        var azureAppServicePrincipalNameHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"].FirstOrDefault();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, azureAppServicePrincipalIdHeader),
            new Claim(ClaimTypes.Name, azureAppServicePrincipalNameHeader ?? azureAppServicePrincipalIdHeader)
        };

        var identity = new ClaimsIdentity(claims, "AzureAppService");
        context.User = new ClaimsPrincipal(identity);
    }
    else
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "localhost")
        };

        var identity = new ClaimsIdentity(claims, "AzureAppService");
        context.User = new ClaimsPrincipal(identity);
        await Console.Out.WriteLineAsync("ELSE");
    }

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
