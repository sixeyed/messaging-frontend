using Autofac;
using HBase.Stargate.Client.Api;
using HBase.Stargate.Client.Autofac;
using System.Configuration;

namespace Sixeyed.MessagingPoweredFrontEnd.Handlers.Persistence.Data
{
    public static class StargateFactory
    {
        private static IContainer _Container;

        static StargateFactory()
        {
            var serverUrl = ConfigurationManager.AppSettings["hbase.cluster.url"];
            var builder = new ContainerBuilder();

            builder.RegisterModule(new StargateModule(new StargateOptions
            {
                ServerUrl = serverUrl
            }));

            _Container = builder.Build();
        }

        public static IStargate GetClient()
        {                      
            return _Container.Resolve<IStargate>();
        }
    }
}
