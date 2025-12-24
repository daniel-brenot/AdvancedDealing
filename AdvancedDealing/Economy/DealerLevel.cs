using System;

namespace AdvancedDealing.Economy
{
    public class DealerLevel
    {
        public int Level;

        public int MaxCustomers;

        public int ItemSlots;

        public float SpeedMultiplier;

        public float RequiredExperience => (float)Math.Round(4 * Math.Pow(Level, 3) / 3);
    }
}