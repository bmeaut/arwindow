using ARWindow.Configuration.WindowConfigurationManagement;
using ARWindow.Core;
using ARWindow.ImageProcessing;
using Injecter.Unity;
using Microsoft.Extensions.DependencyInjection;
using System;
using UnityEngine;

/// <summary>
/// DI Bootstrapper based on
/// https://github.com/KuraiAndras/Injecter
/// </summary>
[DefaultExecutionOrder(-999)] //execute first
public sealed class Bootstrapper : InjectStarter
{
    // Override CreateServiceProvider to add service registrations
    protected override IServiceProvider CreateServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();

        // Mandatory to call AddSceneInjector, optionally configure options
        services.AddSceneInjector(
            injecterOptions => injecterOptions.UseCaching = true,
            sceneInjectorOptions =>
            {
                sceneInjectorOptions.DontDestroyOnLoad = true;
                sceneInjectorOptions.InjectionBehavior = SceneInjectorOptions.Behavior.Factory;
            });

        // Use the usual IServiceCollection methods
        //TODO: Add services here
        //services.AddTransient<IExampleService, ExampleService>();
        services.AddSingleton<WindowConfiguration>();

        // Resolve scripts already in the scene with FindObjectOfType()
        //TODO: Add services here
        services.AddSingleton(_ => FindObjectOfType<Core>());
        services.AddSingleton(_ => FindObjectOfType<CascadeFaceDetection>());

        return services.BuildServiceProvider();
    }
}
