namespace MMOJam
{
    /// <summary>
    /// To be implemented by a component that can be damaged
    /// </summary>
    internal interface IShootable
    {
        /// <summary>
        /// Damages a component
        /// </summary>
        /// <param name="amount">The amount of damage to take</param>
        public void TakeDamage(int amount);
    }
}
