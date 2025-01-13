using Leapfrog.Application.Interfaces;
using Leapfrog.Core.FirmwareImages;
using Leapfrog.Infrastructure.Services;
using Leapfrog.Infrastructure.ViewModels;
using Leapfrog.Interfaces.Services;
using Leapfrog.WEBB.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<Func<Task>>(sp => () => Task.CompletedTask);
builder.Services.AddSingleton<IDelegateCommand, DelegateCommand>();
builder.Services.AddSingleton<IDelegateCommandService, DelegateCommandService>();
builder.Services.AddSingleton<IFirmwareLoaderGenService, FirmwareLoaderGenService>();
builder.Services.AddSingleton<IFirmwareLoaderService, FirmwareLoaderService>();
builder.Services.AddSingleton<IFirmwareLoaderSRecService, FirmwareLoaderSRecService>();
builder.Services.AddSingleton<IFirmwareLoaderUtilsService, FirmwareLoaderUtilsService>();
builder.Services.AddSingleton<IFlashPatternService, FlashPatternService>();
builder.Services.AddSingleton<ILightConfigService, LightConfigService>();
builder.Services.AddSingleton<ILoaderModelService, LoaderModelService>();
builder.Services.AddSingleton<ILoRaWANInterfaceService, LoRaWANInterfaceService>();
builder.Services.AddSingleton<ILoRaWANService, LoRaWANService>();
builder.Services.AddSingleton<IRadioConfigService, RadioConfigService>();
builder.Services.AddSingleton<ISecurityModelService, SecurityModelService>();
builder.Services.AddSingleton<IStateContainerService, StateContainerService>();
builder.Services.AddSingleton<IStatusService, StatusService>();
builder.Services.AddSingleton<IFirmwareBase, Firmware1_0_4>();


builder.Services.AddSingleton<FlashPatternViewModel>();

builder.Services.AddSingleton(sp => new Lazy<ILoRaWANInterfaceService>(sp.GetRequiredService<ILoRaWANInterfaceService>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
