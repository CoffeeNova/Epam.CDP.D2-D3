using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace FileFormatter.Common
{
    public class UniversalInterceptionBehaviour : IInterceptionBehavior
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            BeforeExecutingEvent?.Invoke(input);
            var result = getNext()(input, getNext);
            AfterExecutingEvent?.Invoke(input, result);
            return result;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Enumerable.Empty<Type>();
        }

        public bool WillExecute => true;

        public static event Action<IMethodInvocation> BeforeExecutingEvent;
        public static event Action<IMethodInvocation, IMethodReturn> AfterExecutingEvent;
    }
}
