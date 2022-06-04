using PlayerStatsSystem;

namespace CommonPlugin.Extensions
{
    public class HealthStatEx : HealthStat
    {
        public HealthStatEx()
        {
            this.Health = base.CurValue;
            this.MaxHealth = base.MaxValue;
        }

        public float Health
        {
            get => base.CurValue;
            set => base.CurValue = value;
        }

        public float MaxHealth
        {
            get => MaxHealth;
            set => MaxHealth = value;
        }

        public float MaxHealth2
        {
            get => base.MaxValue;
        }
    }
}