namespace MMOJam
{
    /// <summary>
    /// To be implemented by a component that can be shot
    /// </summary>
    internal interface IShootable
    {
        public void TakeDamage(int amount);
    }
}
