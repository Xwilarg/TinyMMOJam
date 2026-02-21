using MMOJam;
using MMOJam.Manager;
using MMOJam.SO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Player
{
    public class HealthController : NetworkBehaviour, IShootable
    {
        [SerializeField]
        private int _health;

        /// <summary>
        /// Occurs when the object is damaged
        /// </summary>
        public UnityEvent OnTakeDamage { get; } = new();

        /// <summary>
        /// Occurs when the object is destroyed
        /// </summary>
        public UnityEvent OnDestroyed { get; } = new();

        /// <summary>
        /// Gets or sets the current health
        /// </summary>
        public int Health => _health;

        public void TakeDamage(FactionInfo faction, int amount)
        {
            // Don't take damage if we're already dead
            if (_health <= 0)
            {
                return;
            }

            _health -= amount;
            ServerManager.Instance.TakeDamageAt(transform.position, amount);
            OnTakeDamage.Invoke();

            if (_health <= 0)
            {
                OnDestroyed.Invoke();
            }
        }
    }
}
