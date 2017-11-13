using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

using Ocelot.Configuration.File;

namespace Ocelot.ConfigEditor
{
    public class FileReRouteValidator : AbstractValidator<FileReRoute>
    {
        public FileReRouteValidator()
        {
            RuleFor(f => f.UpstreamPathTemplate).NotNull().NotEmpty()
                .Must(StartWithBackslash).Length(2, int.MaxValue).WithMessage("Upstream Path Template must have a route and start with a backslash.");
            RuleFor(f => f.DownstreamPort).NotEqual(0).WithMessage("Downstream Port can not be 0.");
            RuleFor(f => f.DownstreamPathTemplate).NotEmpty().Must(StartWithBackslash).WithMessage("Downstream Path Template must start with a backslash.");
        }

        private static bool StartWithBackslash(string pathTemplate)
        {
            return pathTemplate != null && pathTemplate.StartsWith("/");
        }
    }
}
