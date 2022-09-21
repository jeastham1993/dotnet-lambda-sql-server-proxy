using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Amazon.CDK.AWS.EC2;
using Constructs;


namespace Infrastructure
{
    public class  Network : Construct
    {
        public IVpc Vpc { get; private set; }
        public Network(Construct scope, string id) : base(scope, id)
        {
            var vpc = new Vpc(this, "ApplicationVpc", new VpcProps()
            {
                Cidr = "10.0.0.0/16",
                EnableDnsHostnames = true,
                EnableDnsSupport = true
            });

            this.Vpc = vpc;
        }
    }
}