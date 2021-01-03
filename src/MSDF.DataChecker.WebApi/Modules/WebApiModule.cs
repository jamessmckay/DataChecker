// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using MediatR;
using MediatR.Pipeline;
using MSDF.DataChecker.Domain;
using MSDF.DataChecker.Domain.Providers;

namespace MSDF.DataChecker.WebApi.Modules
{
    public class WebApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAutoMapper(typeof(Startup).Assembly);
            builder.RegisterAutoMapper(typeof(IInfrastructureMarker).Assembly);

            RegisterMediatr();

            RegisterProviders();

            void RegisterProviders()
            {
                builder.RegisterType<DatabaseMapProvider>()
                    .As<IDatabaseMapProvider>()
                    .SingleInstance();

                builder.RegisterType<EncryptionProvider>()
                    .As<IEncryptionProvider>()
                    .SingleInstance();

                builder.RegisterType<DatabaseEnvironmentDatabaseEnvironmentConnectionStringProvider>()
                    .As<IDatabaseEnvironmentConnectionStringProvider>()
                    .SingleInstance();
            }

            void RegisterMediatr()
            {
                // Mediator itself
                builder
                    .RegisterType<Mediator>()
                    .As<IMediator>()
                    .InstancePerLifetimeScope();

                // request & notification handlers
                builder.Register<ServiceFactory>(
                    context =>
                    {
                        var c = context.Resolve<IComponentContext>();
                        return t => c.Resolve(t);
                    });

                var mediatrOpenTypes = new[]
                {
                    typeof(IRequestHandler<,>),
                    typeof(IRequestExceptionHandler<,,>),
                    typeof(IRequestExceptionAction<,>),
                    typeof(INotificationHandler<>),
                };

                foreach (var mediatrOpenType in mediatrOpenTypes)
                {
                    builder
                        .RegisterAssemblyTypes(typeof(Startup).Assembly)
                        .AsClosedTypesOf(mediatrOpenType)
                        .AsImplementedInterfaces();

                    builder
                        .RegisterAssemblyTypes(typeof(IInfrastructureMarker).Assembly)
                        .AsClosedTypesOf(mediatrOpenType)
                        .AsImplementedInterfaces();
                }
            }
        }
    }
}
