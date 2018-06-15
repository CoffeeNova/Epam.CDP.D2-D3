using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Unity.Interception.PolicyInjection.Pipeline;

namespace FileFormatter.Common
{
    public class LogMaker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static JsonSerializerSettings Settings => new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        public LogMaker(IGlobalSettings globalSettings)
        {
            if (globalSettings.LoggingType == GlobalSettings.LoggingAspectType.DynamicProxy)
            {
                UniversalInterceptionBehaviour.BeforeExecutingEvent += OnBeforeExecutingEvent;
                UniversalInterceptionBehaviour.AfterExecutingEvent += OnAfterExecutingEvent;
            }
            Settings.Converters.Add(new StringEnumConverter());
        }

        private static void OnBeforeExecutingEvent(IMethodInvocation inv)
        {
            var logobj = new LogObject
            {
                JoinTime = DateTime.Now.ToString("dd-MM-yy HH:mm:ss.fff"),
                LogPosition = JoinPointLog.MethodEntry,
                MethodName = inv.MethodBase.ReflectedType?.FullName,
                Parameters = GetParameters(inv)
            };
            Logger.Trace("{@joinInfo}", logobj);
        }

        private static void OnAfterExecutingEvent(IMethodInvocation inv, IMethodReturn ret)
        {
            var logobj = new LogObject
            {
                JoinTime = DateTime.Now.ToString("dd-MM-yy HH:mm:ss.fff"),
                MethodName = inv.MethodBase.ReflectedType?.FullName,
                LogPosition = JoinPointLog.MethodExit,
                ReturnedValue = ret.ReturnValue
            };
            Logger.Trace("{@joinInfo}", logobj);
        }

        private static string FormatLogObj(LogObject obj)
        {
            var serialized = JsonConvert.SerializeObject(obj, Settings);
            return serialized;
        }

        private static IDictionary<string, object> GetParameters(IMethodInvocation obj)
        {
            var pars = obj.MethodBase?.GetParameters();
            return pars?
                .Select(x => new { key = x.Name, value = obj.Arguments[x.Name] })
                .ToDictionary(x => x.key, x => x.value);
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class LogObject
        {
            public JoinPointLog LogPosition { get; set; }
            public string JoinTime { get; set; }
            public string MethodName { get; set; }
            public IDictionary<string, object> Parameters { get; set; }
            public object ReturnedValue { get; set; }
        }

        private enum JoinPointLog
        {
            MethodEntry,
            MethodExit
        }
    }
}
