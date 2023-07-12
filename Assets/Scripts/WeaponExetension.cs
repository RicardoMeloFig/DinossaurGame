namespace Assets.Scripts
{
    public static class WeaponExetension
    {
        public static WeaponBooster SetDuration (this WeaponBooster weapon)
        {
            return new WeaponBooster () { Duration = weapon.Duration + 1};
        } 
    }
}
