using Encrypted.Configuration.Util.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orp.Core.Azure.Function.Template.Configuration
{
    public class TenantConfigurationHolder
    {
        [EncryptedProperty]
        public List<TenantConfiguration> TenantConfigurations { get; set; }
    }

    public class TenantConfiguration
    {
        public string TenantId { get; set; }

        [EncryptedProperty]
        public List<ScopeConfiguration> ScopeConfigurations { get; set; }
    }
}
