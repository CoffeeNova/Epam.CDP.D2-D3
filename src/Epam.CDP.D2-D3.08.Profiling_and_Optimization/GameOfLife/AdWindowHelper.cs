using System;
using System.Windows.Threading;

namespace GameOfLife
{
    internal static class AdWindowHelper
    {
        public static DispatcherTimer CreateAdTimer(EventHandler onTick)
        {
            var adTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            adTimer.Tick += onTick;
            adTimer.Start();
            return adTimer;
        }

        public static void UnsubscribeAdTime(this DispatcherTimer adTimer, EventHandler onTick)
        {
            if (adTimer == null)
                throw new ArgumentNullException(nameof(adTimer));

            if (onTick != null)
                adTimer.Tick -= onTick;
        }
    }
}
