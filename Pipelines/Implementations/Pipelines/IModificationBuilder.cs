using Pipelines.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipelines.Implementations.Pipelines
{
    public interface IModificationBuilder
    {
        IModificationBuilder Before<TProcessorOriginal, TProcessorBefore>() where TProcessorBefore : new();
        IModificationBuilder Before<TProcessorOriginal>(IProcessor before);
        IModificationBuilder Before(Type original, IProcessor before);
        IModificationBuilder Before(IProcessor original, IProcessor before);
        IModificationBuilder Before(IProcessor original, Func<IProcessor> before);

        IModificationBuilder After<TProcessorOriginal, TProcessorAfter>() where TProcessorAfter : new();
        IModificationBuilder After<TProcessorOriginal>(IProcessor after);
        IModificationBuilder After(Type original, IProcessor after);
        IModificationBuilder After(IProcessor original, IProcessor after);
        IModificationBuilder After(IProcessor original, Func<IProcessor> after);

        IModificationBuilder Instead<TProcessorOriginal, TProcessorReplacement>() where TProcessorReplacement : new();
        IModificationBuilder Instead<TProcessorOriginal>(IProcessor replacement);
        IModificationBuilder Instead(Type original, IProcessor replacement);
        IModificationBuilder Instead(IProcessor original, IProcessor replacement);
        IModificationBuilder Instead(IProcessor original, Func<IProcessor> replacement);

        IModificationConfiguration GetConfiguration();
    }

}
