﻿using System;

namespace AutoPipe
{
    /// <summary>
    /// Marks a method to be executed within an <see cref="AutoProcessor"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RunAttribute : Attribute
    {
    }
}
