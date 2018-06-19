using PostSharp.Aspects;
using System;

namespace FileFormatter.Common
{
    [Serializable]
    public class UniversalOnMethodBoundaryAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            BeforeExecutingEvent?.Invoke(args);
            base.OnEntry(args);
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            AfterExecutingEvent?.Invoke(args);
            base.OnSuccess(args);
        }


        public static event Action<MethodExecutionArgs> BeforeExecutingEvent;
        public static event Action<MethodExecutionArgs> AfterExecutingEvent;
    }
}
