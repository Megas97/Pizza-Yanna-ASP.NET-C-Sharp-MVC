using System;
using System.Threading.Tasks;

namespace PizzaYanna
{
    public class DailyTrigger
    {
        readonly TimeSpan triggerHour;

        public DailyTrigger(int hour, int minute = 0, int second = 0)
        {
            triggerHour = new TimeSpan(hour, minute, second);
            InitiateAsync();
        }

        async void InitiateAsync()
        {
            while (true)
            {
                var triggerTime = DateTime.Today + triggerHour - DateTime.Now;
                if (triggerTime < TimeSpan.Zero)
                {
                    triggerTime = triggerTime.Add(new TimeSpan(24, 0, 0));
                }
                await Task.Delay(triggerTime);
                OnTimeTriggered?.Invoke();
            }
        }

        public event Action OnTimeTriggered;
    }
}