using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using WasmReload.Server.Services;
using WasmReload.Shared.Models;

namespace WasmReload.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IHost host;

        public ServicesController(IHost host)
        {
            this.host = host;
        }

        [HttpGet]
        public string Get(string instanceId, string service)
        {
            var type = Type.GetType($"WasmReload.Server.Services.{service}");
            var scope = host.Services.CreateScope();
            var context = scope.ServiceProvider.GetService(type);
            ServicesService.Services.Add((instanceId, context));

            return instanceId;
        }

        [HttpGet("/ServiceMethod")]
        public string Method(string instanceId, string method, string parameters)
        {
            var service = ServicesService.Services.FirstOrDefault(s => s.Item1 == instanceId);
            var parsedParameters = JsonConvert.DeserializeObject<Customer[]>(parameters);

            MethodInfo info = service.Item2.GetType().GetMethod(method);
            var result = info.Invoke(service.Item2, parsedParameters);

            return JsonConvert.SerializeObject(result.Result);
        }
    }
}
