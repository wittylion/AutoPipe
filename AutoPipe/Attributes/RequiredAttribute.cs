﻿using System;

namespace AutoPipe
{
    /// <summary>
    /// This attribute is supposed to be used for parameters of
    /// methods marked with <see cref="RunAttribute"/>.
    /// Allows to provide some data before method will be executed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class RequiredAttribute : Attribute
    {
        /// <summary>
        /// If true pipeline will end execution if
        /// <see cref="PipelineContext"/> does not contain 
        /// a property called as <see cref="Name"/> or parameter name.
        /// Will add an <see cref="ErrorMessage"/> as the end reason.
        /// </summary>
        public bool End { get; set; }

        /// <summary>
        /// In case <see cref="EndIfNotExist"/> true and property is not found,
        /// this message will be added to the messages collection of the <see cref="PipelineContext"/>.
        /// </summary>
        public string Message { get; set; }

    }
}
