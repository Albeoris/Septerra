using System;
using Septerra.Core.Hooks;

namespace Septerra;

public sealed class GameInjectionDescriptor
{
    public String DllPath { get; set; }
    public GameInjectionHookAddressTable AddressTable { get; set; }
}