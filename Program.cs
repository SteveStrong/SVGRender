using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SVGRender.Services;
using ApprenticeFoundryMentorModeler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register traditional hinge service
builder.Services.AddScoped<LivingHingeService>();

// Register FoundryMentorModeler services
builder.Services.AddSingleton<IMentorServices, MentorServices>();
builder.Services.AddSingleton<IMentorModelManager, MentorModelManager>();
builder.Services.AddSingleton<IMentorPlayground, MentorPlayground>();

// Register our knowledge-based service
builder.Services.AddScoped<LivingHingeKnowledgeService>();

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
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
