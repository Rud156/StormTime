using System;
using Godot;

namespace StormTime.Player.Modifiers
{
    public class PlayerModifierTypes : Node
    {
        public enum ShopItem
        {
            // Shop Items
            HealthPotion,
            BulletsFreezeEnemy,
            ShotGun,
            ChargeGun,
            Shield
        }

        public enum SacrificialItem
        {
            // Sacrificial Items
            SpeedSacrificeHealthBoost,
            SpeedSacrificeDamageIncrease,
            HealthSacrificeDamageIncrease,
            ShootTimeSacrificeDamageIncrease,
            HealthSacrificeSpeedIncrease
        }

        #region Sacrificial Items

        public struct SacrificialItemInfo
        {
            public SacrificialItem sacrificialItem;
            public int increasedPercent;
            public int reducedPercent;
        }

        public static SacrificialItem GetRandomSacrificialItem()
        {
            Array sacrificialValues = Enum.GetValues(typeof(SacrificialItem));
            return (SacrificialItem)sacrificialValues.GetValue(GD.Randi() % sacrificialValues.Length);
        }

        public static string GetSacrificialItemDescription(SacrificialItem sacrificialItem)
        {
            switch (sacrificialItem)
            {
                case SacrificialItem.SpeedSacrificeHealthBoost:
                    return "Become slower to but increase maximum health by 10%";

                case SacrificialItem.SpeedSacrificeDamageIncrease:
                    return "Become slower but add 20% to weapon damage";

                case SacrificialItem.HealthSacrificeDamageIncrease:
                    return "Sacrifice max health but add 30% to weapon damage";

                case SacrificialItem.ShootTimeSacrificeDamageIncrease:
                    return "Shoot slowly but increase damage by 25%";

                case SacrificialItem.HealthSacrificeSpeedIncrease:
                    return "Sacrifice max health but increase speed by 20%";

                default:
                    throw new ArgumentOutOfRangeException(nameof(sacrificialItem), sacrificialItem, null);
            }
        }

        public static SacrificialItemInfo GetSacrificialItemAffecter(SacrificialItem sacrificialItem)
        {
            switch (sacrificialItem)
            {
                case SacrificialItem.SpeedSacrificeHealthBoost:
                    return new SacrificialItemInfo()
                    {
                        sacrificialItem = SacrificialItem.SpeedSacrificeHealthBoost,
                        reducedPercent = 20,
                        increasedPercent = 10
                    };

                case SacrificialItem.SpeedSacrificeDamageIncrease:
                    return new SacrificialItemInfo()
                    {
                        sacrificialItem = SacrificialItem.SpeedSacrificeDamageIncrease,
                        reducedPercent = 20,
                        increasedPercent = 20
                    };

                case SacrificialItem.HealthSacrificeDamageIncrease:
                    return new SacrificialItemInfo()
                    {
                        sacrificialItem = SacrificialItem.HealthSacrificeDamageIncrease,
                        reducedPercent = 10,
                        increasedPercent = 30
                    };

                case SacrificialItem.ShootTimeSacrificeDamageIncrease:
                    return new SacrificialItemInfo()
                    {
                        sacrificialItem = SacrificialItem.ShootTimeSacrificeDamageIncrease,
                        reducedPercent = 10,
                        increasedPercent = 25
                    };

                case SacrificialItem.HealthSacrificeSpeedIncrease:
                    return new SacrificialItemInfo()
                    {
                        sacrificialItem = SacrificialItem.HealthSacrificeSpeedIncrease,
                        reducedPercent = 10,
                        increasedPercent = 20
                    };

                default:
                    throw new ArgumentOutOfRangeException(nameof(sacrificialItem), sacrificialItem, null);
            }
        }

        #endregion

        #region Shop Items

        private const float HealthPotionIncrease = 40;

        public struct ShopItemInfo
        {
            public ShopItem shopItem;
            public float valueChange;
        }

        public static ShopItem GetRandomShopItem()
        {
            Array shopValues = Enum.GetValues(typeof(ShopItem));
            return (ShopItem)shopValues.GetValue(GD.Randi() % shopValues.Length);
        }

        public static string GetShopItemDescription(ShopItem shopItem)
        {
            switch (shopItem)
            {
                case ShopItem.HealthPotion:
                    return "Gives a bit of health";

                case ShopItem.ShotGun:
                    return "High damage but recoiling gun. Spray your enemies";

                case ShopItem.ChargeGun:
                    return "The longer you charge the stronger it becomes";

                case ShopItem.Shield:
                    return "Protects for a few seconds. Rechargeable";

                case ShopItem.BulletsFreezeEnemy:
                    return "Bullets temporarily freeze enemies randomly";

                default:
                    throw new ArgumentOutOfRangeException(nameof(shopItem), shopItem, null);
            }
        }

        public static ShopItemInfo GetShopItem(ShopItem shopItem)
        {
            switch (shopItem)
            {
                case ShopItem.HealthPotion:
                    return new ShopItemInfo()
                    {
                        shopItem = shopItem,
                        valueChange = HealthPotionIncrease
                    };

                case ShopItem.BulletsFreezeEnemy:
                    return new ShopItemInfo()
                    {
                        shopItem = shopItem
                    };

                case ShopItem.ShotGun:
                    return new ShopItemInfo()
                    {
                        shopItem = shopItem
                    };

                case ShopItem.ChargeGun:
                    return new ShopItemInfo()
                    {
                        shopItem = shopItem
                    };


                case ShopItem.Shield:
                    return new ShopItemInfo()
                    {
                        shopItem = shopItem
                    };

                default:
                    throw new ArgumentOutOfRangeException(nameof(shopItem), shopItem, null);
            }
        }

        #endregion
    }
}