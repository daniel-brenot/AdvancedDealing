using System.Collections.Generic;

namespace AdvancedDealing.Economy
{
    public class LevelSystem
    {
        private readonly DealerManager _dealerManager;

        private static readonly List<DealerLevel> levels = [];

        public LevelSystem(DealerManager dealerManager)
        {
            _dealerManager = dealerManager;
        }

        public void AddXP(float amount)
        {
            _dealerManager.DealerData.Experience += amount;
            int calculatedLevel = CalculateLevel(_dealerManager.DealerData.Experience);

            if (calculatedLevel > _dealerManager.DealerData.Level)
            {
                LevelUp(calculatedLevel);
            }
        }

        public void LevelUp(int newLevel)
        {
            DealerLevel level = levels[newLevel - 1];

            _dealerManager.DealerData.Level = newLevel;
            _dealerManager.DealerData.MaxCustomers = level.MaxCustomers;
            _dealerManager.DealerData.ItemSlots = level.ItemSlots;
            _dealerManager.DealerData.SpeedMultiplier = level.SpeedMultiplier;

            DealerManager.Update(_dealerManager.ManagedDealer, true);

            // NOTIFY PLAYER
        }

        public static int CalculateLevel(float experience)
        {
            int level = 0;
            bool levelFound = false;

            for (int i = levels.Count - 1; !levelFound && i >= 0; i--)
            {
                if (levels[i].RequiredExperience < experience)
                {
                    level++;
                }
                else
                {
                    levelFound = true;
                }
            }

            return level;
        }

        public static void CreateLevels()
        {
            // Level 1
            levels.Add(new()
            {
                Level = 1,
                MaxCustomers = 4,
                ItemSlots = 5,
                SpeedMultiplier = 0.8f
            });
            // Level 2
            levels.Add(new()
            {
                Level = 2,
                MaxCustomers = 5,
                ItemSlots = 5,
                SpeedMultiplier = 0.85f
            });
            // Level 3
            levels.Add(new()
            {
                Level = 3,
                MaxCustomers = 6,
                ItemSlots = 5,
                SpeedMultiplier = 0.9f
            });
            // Level 4
            levels.Add(new()
            {
                Level = 4,
                MaxCustomers = 7,
                ItemSlots = 6,
                SpeedMultiplier = 1f
            });
            // Level 5
            levels.Add(new()
            {
                Level = 5,
                MaxCustomers = 8,
                ItemSlots = 6,
                SpeedMultiplier = 1.15f
            });
            // Level 6
            levels.Add(new()
            {
                Level = 6,
                MaxCustomers = 10,
                ItemSlots = 7,
                SpeedMultiplier = 1.25f
            });
            // Level 7
            levels.Add(new()
            {
                Level = 7,
                MaxCustomers = 12,
                ItemSlots = 7,
                SpeedMultiplier = 1.4f
            });
            // Level 8
            levels.Add(new()
            {
                Level = 8,
                MaxCustomers = 14,
                ItemSlots = 8,
                SpeedMultiplier = 1.6f
            });
            // Level 9
            levels.Add(new()
            {
                Level = 9,
                MaxCustomers = 16,
                ItemSlots = 8,
                SpeedMultiplier = 1.8f
            });
            // Level 10
            levels.Add(new()
            {
                Level = 10,
                MaxCustomers = 20,
                ItemSlots = 10,
                SpeedMultiplier = 2f
            });
        }
    }
}
