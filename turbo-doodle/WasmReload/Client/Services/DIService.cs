using Dynamitey;
using ImpromptuInterface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WasmReload.Shared.Interfaces;
using WasmReload.Shared.Models;

namespace WasmReload.Client.Services
{
    public class DIService
    {
        private readonly HttpClient client;

        public DIService(HttpClient client)
        {
            this.client = client;
        }

        public string InstanceId { get; set; }

        public async Task<object> ResolveCustomersServiceAsync(Type serviceType)
        {
            var type = Type.GetType($"WasmReload.Shared.Interfaces.{serviceType.Name}, WasmReload.Shared");
            var props = type.GetMethods();

            var expando = new ExpandoObject() as IDictionary<string, object>;

            foreach (var prop in props)
            {
                var parameters = prop.GetParameters();
                if(parameters.Length == 0)
                    expando.Add(prop.Name, Return<Task<dynamic>>.Arguments(async () => await InvokeRemoteMethodAsync(prop.Name, 0)));
                if (parameters.Length == 1)
                    //expando.Add(prop.Name, Return<Task<Customer>>.Arguments<dynamic>(async (p1) => await InvokeRemoteMethodAsync(prop.Name, 1, p1)));
                    expando.Add(prop.Name, Return<Task<dynamic>>.Arguments<dynamic>(async (p1) => await InvokeRemoteMethodAsync(prop.Name, 1, p1)));
                if (parameters.Length == 2)
                    expando.Add(prop.Name, Return<Task<dynamic>>.Arguments<dynamic, dynamic>(async (p1, p2) => await InvokeRemoteMethodAsync(prop.Name, 2, p1, p2)));
                if (parameters.Length == 3)
                    expando.Add(prop.Name, Return<Task<dynamic>>.Arguments<dynamic, dynamic, dynamic>(async (p1, p2, p3) => await InvokeRemoteMethodAsync(prop.Name, 3, p1, p2, p3)));
            }

            dynamic myInterface = Impromptu.DynamicActLike(expando, type);
            InstanceId = await client.GetStringAsync($"Services?Service={serviceType.Name.TrimStart('I')}&InstanceId={Guid.NewGuid()}");

            return myInterface;
        }

        public async Task<dynamic> InvokeRemoteMethodAsync(string name, int count, dynamic obj1 = null, dynamic obj2 = null, dynamic obj3 = null)
        {
            var parameters = new List<object>() { obj1, obj2, obj3 };
            parameters = parameters.Take(count).ToList();
            var method = await client.GetStringAsync($"ServiceMethod?method={name}&instanceId={InstanceId}{(count > 0 ? $"&parameters={JsonConvert.SerializeObject(parameters)}" : "")}");
            var type = Type.GetType($"WasmReload.Shared.Models.{"Customer"}, WasmReload.Shared");
            var des = JsonConvert.DeserializeObject(method, type);

            return des;
        }
    }
}
