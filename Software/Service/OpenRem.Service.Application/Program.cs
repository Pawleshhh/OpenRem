﻿using Autofac;
using OpenRem.Common.Application.Autofac;
using OpenRem.Service.Server;
using System;

namespace OpenRem.Service.Application;

class Program
{
    static void Main(string[] args)
    {
        var container = Bootstraper.BuildContainer(AssemblyFilter.OnlyLogic);

        var serviceWrapper = container.Resolve<IEngineServiceHost>();
        serviceWrapper.Start();

        Console.WriteLine(
            $"Running OpenRem service on {serviceWrapper.HostName}:{serviceWrapper.Port}. Press Enter to close...");
        Console.ReadLine();

        serviceWrapper.StopAsync();
    }
}