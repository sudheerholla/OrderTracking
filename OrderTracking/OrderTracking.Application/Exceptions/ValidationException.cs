using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OrderTracking.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new ModelStateDictionary();
        }

        public ValidationException(List<ValidationFailure> failures)
            : this()
        {
            var failureGroups = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

            foreach (var failureGroup in failureGroups)
            {
                var propertyName = failureGroup.Key;
                
                Errors.AddModelError(propertyName, failureGroup.ToString());
            }
        }

        public ModelStateDictionary Errors { get; }
    }
}
