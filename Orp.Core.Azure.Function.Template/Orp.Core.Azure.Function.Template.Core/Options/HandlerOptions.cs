using System;

namespace Orp.Core.Azure.Function.Template.Core.Options
{
    public class HandlerOptions
    {
        private readonly string _property;

        public HandlerOptions(string property)
        {
            _property = property ?? throw new ArgumentNullException(nameof(property));
        }
    }
}
