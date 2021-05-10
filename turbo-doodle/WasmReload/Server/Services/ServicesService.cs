using System;
using System.Collections.Generic;

namespace WasmReload.Server.Services
{
    public static class ServicesService
    {
        /// <summary>
        /// List of services registered. Instance Id and the actual type
        /// </summary>
        public static List<(string, dynamic)> Services { get; set; } = new List<(string, dynamic)>();
    }
}
