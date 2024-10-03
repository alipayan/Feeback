using Feedback.APIs;
using Feedback.APIs.Endpoints;
using Feedback.APIs.Persistence;
using Feedback.APIs.Serivces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<FeedbackDbContext>(configure =>
{

    configure.UseSqlServer(builder.Configuration.GetConnectionString(FeedbackDbContext.ConnectionStringName));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserPrincipals, UserPrincipal>(sp =>
{
    var httpcontext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;

    if (!httpcontext.Request.Headers.TryGetValue(UserPrincipal.TenantHeaderName, out StringValues value))
    {
        throw new InvalidOperationException($"can't find {UserPrincipal.TenantHeaderName} in your header request");
    }

    return new UserPrincipal(Convert.ToInt32(value));
});

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<SubjectService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapSubjectEndpoint();

app.Run();
