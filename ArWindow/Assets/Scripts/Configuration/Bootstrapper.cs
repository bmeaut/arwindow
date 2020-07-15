﻿using UnityEngine;
using System.Collections;
using Injecter.Unity;
using System;
using Microsoft.Extensions.DependencyInjection;

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
        //services.AddSingleton<I,>();

        // Resolve scripts already in the scene with FindObjectOfType()
        //services.AddSingleton<MonoBehaviourService>(_ => GameObject.FindObjectOfType<MonoBehaviourService>());

        return services.BuildServiceProvider();
    }
}
