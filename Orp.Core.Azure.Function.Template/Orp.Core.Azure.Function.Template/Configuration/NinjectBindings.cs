using Encrypted.Configuration.Provider.JsonFiles;
using log4net;
using Ninject;
using Ninject.Modules;
using Orp.Core.Azure.Function.Template.Core.Handlers;
using Orp.Core.Azure.Function.Template.Core.Options;
using Orp.Core.Logging.Adapters.Log4Net;
using Orp.Core.Logging.DI.Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Wolfpack.MultiTenant.Core;

namespace Orp.Core.Azure.Function.Template.Configuration
{
    internal class NinjectBindings : NinjectModule
    {
        private readonly ConfigurationHelper _configuration;

        public NinjectBindings(ConfigurationHelper configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override void Load()
        {
            BindNinjectContainerLogging();

            Bind<IProvideEncryptedConfiguration<TenantConfigurationHolder>>().To<EncryptedConfigurationProvider<TenantConfigurationHolder>>()
                .WithConstructorArgument("configurationFilePath", Path.Combine(_configuration.AppBaseDirectory, $"TenantConfiguration.{_configuration.CurrentEnvironment}.json"))
                .WithConstructorArgument("fingerPrint", _configuration.ConfigCertThumbprint);

            Bind<TenantConfigurationHolder>().ToMethod(x => x.Kernel.Get<IProvideEncryptedConfiguration<TenantConfigurationHolder>>().GetEncryptedConfiguration());

            // Bind services that are not tenant specific: 
            Bind<MessageHandler>().ToSelf();

            // Bind options that are not tenant specific: 
            Bind<HandlerOptions>().ToConstant(new HandlerOptions("someProperty"));

            foreach (TenantConfiguration tenantConfiguration in GetTenantConfigurationList())
            {
                foreach (ScopeConfiguration scopeConfiguration in tenantConfiguration.ScopeConfigurations ?? Enumerable.Empty<ScopeConfiguration>())
                {
                    var tenantContext = new TenantContext(tenantConfiguration.TenantId, scopeConfiguration.ScopeId);

                    Bind<TenantContext>().ToConstant(tenantContext)
                                         .MultiTenant(tenantContext.TenantScope)
                                         .InSingletonScope();

                    // Bind services that are tenant specific. 

                    Kernel.UseTenantLogger(tenantContext.TenantId, tenantContext.ScopeId);
                }
            }
        }

        /// <summary>
        /// Injects a logger into the Ninject DI container. 
        /// The services inside the Ninject DI container will use this logger since the logger 
        /// defined in the .NET Core container will be inaccessible in this context. 
        /// </summary>
        private void BindNinjectContainerLogging()
        {
            string path = Path.Combine(_configuration.AppBaseDirectory, $"log4net.{_configuration.CurrentEnvironment}.config");
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

            log4net.Config.XmlConfigurator.Configure(logRepository, new FileInfo(path));
            Kernel.UseLogging<Log4NetFactory>();
        }

        private IEnumerable<TenantConfiguration> GetTenantConfigurationList()
        {
            var list = Kernel.Get<TenantConfigurationHolder>()?.TenantConfigurations;

            if (!list?.Any() ?? true)
            {
                throw new Exception("Unable to find any tenant configurations in the configuration file.");
            }

            return list;
        }
    }
}
