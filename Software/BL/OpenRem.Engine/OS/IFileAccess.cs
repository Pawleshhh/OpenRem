﻿namespace OpenRem.Engine.OS;

internal interface IFileAccess
{
    Stream RecreateAlwaysFile(string fileName);
}