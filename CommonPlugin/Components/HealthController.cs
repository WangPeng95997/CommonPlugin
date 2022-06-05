using PlayerStatsSystem;
using UnityEngine;

namespace CommonPlugin.Components
{
    public class HealthController : MonoBehaviour
    {
        private float maxHealth;

        private HealthStat healthStat;

        public float Health
        {
            get => this.healthStat.CurValue;
            set => this.healthStat.CurValue = value;
        }

        public float MaxHealth
        {
            get => this.maxHealth;
            set => this.maxHealth = value;
        }

        public float MaxHealth2
        {
            get => this.healthStat.MaxValue;
        }

        private void Awake()
        {
            this.healthStat = ReferenceHub.GetHub(this.gameObject).playerStats.GetModule<HealthStat>();
        }

        public void Start()
        {
            this.MaxHealth = this.MaxHealth2;
        }

        private void OnEnable()
        {

        }

        private void FixedUpdate()
        {

        }

        private void Update()
        {

        }

        private void LateUpdate()
        {

        }

        private void OnDisable()
        {

        }

        private void OnDestroy()
        {

        }
    }
}