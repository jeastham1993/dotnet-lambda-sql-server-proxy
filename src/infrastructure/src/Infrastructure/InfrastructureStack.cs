using Amazon.CDK;
using Constructs;

namespace Infrastructure
{
    public class InfrastructureStack : Stack
    {
        internal InfrastructureStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var network = new Network(this, "Networking");

            var persistenceLayer = new PersistenceLayer(this, "Persistence", new PersistenceLayerProps()
            {
                Vpc = network.Vpc
            });
        }
    }
}
