﻿using Autofac;
using System.Reflection;

namespace OpenRem.Service.Client;

class ServiceClientModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var dataAccess = Assembly.GetExecutingAssembly();
        builder.RegisterAssemblyTypes(dataAccess).AsImplementedInterfaces();
        builder.RegisterType<ServiceConfig>().AsSelf().SingleInstance();
    }
}