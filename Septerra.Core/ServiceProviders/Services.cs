using System;
using System.Collections.Generic;
using Septerra.Core;

namespace Septerra.Core.Hooks
{
    public static class Services<T> where T : IService, new()
    {
        public static T Instance { get; } = new T();
    }
}