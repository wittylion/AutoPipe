using System;
using System.Threading.Tasks;

namespace AutoPipe
{
    /// <summary>
    /// Extension methods for classes <see cref="Action{T}"/>
    /// and <see cref="Func{TResult}"/>.
    /// </summary>
    public static class ActionExtensionMethods
    {
        /// <summary>
        /// Converts an <see cref="Action"/> to the asynchronous
        /// function <see cref="Func{T, TResult}"/> where TResult is Task.
        /// </summary>
        /// <typeparam name="TArgs">
        /// Parameter of the action.
        /// </typeparam>
        /// <param name="action">
        /// Represents an action to be converted to asynchronous function.
        /// </param>
        /// <returns>
        /// Returns a function which is produced from <paramref name="action"/>
        /// or <c>null</c> if action is null.
        /// </returns>
        internal static Func<TArgs, Task> ToAsync<TArgs>(this Action action)
        {
            if (action.HasNoValue())
                return null;

            return args => Task.Run(action);
        }

        /// <summary>
        /// Converts an <see cref="Action{T}"/> to the asynchronous
        /// function <see cref="Func{T, TResult}"/> where TResult is Task.
        /// </summary>
        /// <typeparam name="TArgs">
        /// Parameter of the action.
        /// </typeparam>
        /// <param name="action">
        /// Represents an action to be converted to asynchronous function.
        /// </param>
        /// <returns>
        /// Returns a function which is produced from <paramref name="action"/>
        /// or <c>null</c> if action is null.
        /// </returns>
        internal static Func<TArgs, Task> ToAsync<TArgs>(this Action<TArgs> action)
        {
            if (action.HasNoValue())
                return null;

            return args => Task.Run(() => action(args));
        }

        /// <summary>
        /// Converts an <see cref="Action{TArg1, Targ2}"/> to the asynchronous
        /// function <see cref="Func{TArg1, Targ2, TResult}"/> where TResult is Task.
        /// </summary>
        /// <typeparam name="TArg1">
        /// Parameter of the action.
        /// </typeparam>
        /// <typeparam name="TArg2">
        /// Parameter of the action.
        /// </typeparam>
        /// <param name="action">
        /// Represents an action to be converted to asynchronous function.
        /// </param>
        /// <returns>
        /// Returns a function which is produced from <paramref name="action"/>
        /// or <c>null</c> if action is null.
        /// </returns>
        internal static Func<TArg1, TArg2, Task> ToAsync<TArg1, TArg2>(this Action<TArg1, TArg2> action)
        {
            if (action.HasNoValue())
                return null;

            return (arg1, arg2) => Task.Run(() => action(arg1, arg2));
        }

        /// <summary>
        /// Converts an action to a processor. Helps to write code quickly,
        /// without introducing instance of interface <see cref="IProcessor"/>.
        /// </summary>
        /// <param name="action">
        /// Represents an action to be converted to a processor.
        /// </param>
        /// <returns>
        /// Returns a processor which is produced from <paramref name="action"/>
        /// or <c>null</c> if action is null.
        /// </returns>
        public static IProcessor ToProcessor(this Action action)
        {
            if (action.HasNoValue())
                return null;

            return ActionProcessor.From(action);
        }

        /// <summary>
        /// Converts an action to a processor. Helps to write code quickly,
        /// without introducing instance of interface <see cref="IProcessor"/>.
        /// </summary>
        /// <param name="action">
        /// Represents an action to be converted to a processor.
        /// </param>
        /// <returns>
        /// Returns a processor which is produced from <paramref name="action"/>
        /// or <c>null</c> if action is null.
        /// </returns>
        public static IProcessor ToProcessor(this Func<Task> action)
        {
            if (action.HasNoValue())
                return null;

            return ActionProcessor.From(context => action());
        }
    }
}