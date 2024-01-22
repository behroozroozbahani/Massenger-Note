﻿using System;
using System.Collections.Generic;
using System.Linq;
using PortalCore.Application.Common.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace PortalCore.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public ValidationException(IdentityResult identityResult)
            : this()
        {
            Errors = identityResult.GetErrors();
        }

        public ValidationException(string propertyName, string[] errorMessage)
            : this()
        {
            Errors.Add(propertyName, errorMessage);
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}