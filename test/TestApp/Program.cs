using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.FeeShareEngine.Client;
using Service.FeeShareEngine.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();

            //
            // var factory = new FeeShareEngineClientFactory("http://localhost:5001");
            // var client = factory.GetHelloService();
            //
            // var resp = await  client.SayHelloAsync(new AddReferralRequest(){ClientId = "Alex"});
            // Console.WriteLine(resp?.Message);

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
