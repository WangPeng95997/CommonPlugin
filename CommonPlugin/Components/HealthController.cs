using PlayerStatsSystem;
using UnityEngine;

namespace CommonPlugin.Components
{
    public class HealthController : MonoBehaviour
    {
        private bool evolution;

        private float heal;

        private float heal2;

        private float maxHealth;

        private HealthStat healthStat;

        public bool Evolved
        {
            get => this.evolution;
            set => this.evolution = value;
        }

        public float Heal
        {
            get => this.heal;
            set => this.heal = value;
        }

        public float Heal2
        {
            get => this.heal2;
            set => this.heal2 = value;
        }

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

        }

        public void Start()
        {
            this.healthStat = ReferenceHub.GetHub(this.gameObject).playerStats.GetModule<HealthStat>();

            this.Evolved = false;
            this.MaxHealth = this.MaxHealth2;
            this.Heal = 0;
            this.Heal2 = 0;
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