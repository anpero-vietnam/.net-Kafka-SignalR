using Confluent.Kafka;
using KafkaSignalR.Pages.Hubs;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Web;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
//builder.Services.AddControllers();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
       new[] { "application/octet-stream" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseResponseCompression();

app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
// https://learn.microsoft.com/vi-vn/aspnet/core/tutorials/signalr?view=aspnetcore-6.0&tabs=visual-studio
app.MapHub<KafkaHub>("/KafkaHub");
app.MapControllers();
app.Run();
