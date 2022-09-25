using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.EKS;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.RDS;
using Constructs;

namespace Infrastructure
{
    public record PersistenceLayerProps
    {
        public IVpc Vpc { get; set; }
    }

    public class PersistenceLayer : Construct
    {
        public PersistenceLayer(Construct scope, string id, PersistenceLayerProps props) : base(scope, id)
        {
            var database = new DatabaseInstance(scope, "products-db", new DatabaseInstanceProps()
            {
                Vpc = props.Vpc,
                VpcSubnets = new SubnetSelection()
                {
                    SubnetType = SubnetType.PRIVATE_WITH_EGRESS
                },
                Engine = DatabaseInstanceEngine.SqlServerEx(new SqlServerExInstanceEngineProps()
                {
                    Version = SqlServerEngineVersion.VER_15
                }),
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.SMALL),
                Credentials = Credentials.FromGeneratedSecret("admin"),
                MultiAz = false,
                AllocatedStorage = 100,
                MaxAllocatedStorage = 105,
                DeleteAutomatedBackups = true,
                DeletionProtection = false,
                PubliclyAccessible = false,
            });

            database.Connections.AllowFromAnyIpv4(Port.Tcp(1433));
            database.Connections.AllowFrom(database.Connections.SecurityGroups[0], Port.Tcp(1433));

            // CDK does not yet support DatabaseProxy for RDS SQL Server
            // https://github.com/aws/aws-cdk/issues/22164
            // var proxy = new DatabaseProxy(this, "SqlProxy", new DatabaseProxyProps()
            // {
            //     ProxyTarget = ProxyTarget.FromInstance(database),
            //     Secrets = new[] {database.Secret},
            //     Vpc = props.Vpc
            // });
            //
            // var role = new Role(this, "SqlProxyRole", new RoleProps()
            // {
            //     AssumedBy = new AccountPrincipal(Stack.Of(this).Account)
            // });
            // proxy.GrantConnect(role, "admin");
        }
    }
}