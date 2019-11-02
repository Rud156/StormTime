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
            ChargeGun
        }

        public enum SacrificialItem
        {
            // Sacrificial Items
            SpeedSacrificeHealthBoost,
            SpeedSacrificeDamageIncrease,
            HealthSacrificeDamageIncrease,
            ShootTimeSacrificeDamageIncrease,
            HealthSacrificeSpeedIncrease,
            SpeedSacrificeShieldTimeIncrease,
            HealthSacrificeShieldTimeIncrease,
            ShieldTimeSacrificeHealthBoost,
            ShieldTimeSacrificeSpeedIncrease
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

                case SacrificialItem.SpeedSacrificeShieldTimeIncrease:
                    return "Become slower but add 20% to shield time";

                case SacrificialItem.HealthSacrificeShieldTimeIncrease:
                    return "Sacrifice max health but add 25% to shield time";

                case SacrificialItem.ShieldTimeSacrificeHealthBoost:
                    return "Decrease shield time but increase maximum health by 10%";

                case SacrificialItem.ShieldTimeSacrificeSpeedIncrease:
                    return "Decrease shield time but increase speed by 20%";

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

                case SacrificialItem.SpeedSacrificeShieldTimeIncrease:
                    return new SacrificialItemInfo()
                    {
                        sacrificialItem = SacrificialItem.SpeedSacrificeShieldTimeIncrease,
                        reducedPercent = 20,
                        increasedPercent = 20
                    };

                case SacrificialItem.HealthSacrificeShieldTimeIncrease:
                    return new SacrificialItemInfo()
                    {
                        sacrificialItem = SacrificialItem.HealthSacrificeShieldTimeIncrease,
                        reducedPercent = 10,
                        increasedPercent = 25
                    };

                case SacrificialItem.ShieldTimeSacrificeHealthBoost:
                    return new SacrificialItemInfo()
                    {
                        sacrificialItem = SacrificialItem.ShieldTimeSacrificeHealthBoost,
                        reducedPercent = 25,
                        increasedPercent = 10
                    };

                case SacrificialItem.ShieldTimeSacrificeSpeedIncrease:
                    return new SacrificialItemInfo()
                    {
                        sacrificialItem = SacrificialItem.ShieldTimeSacrificeSpeedIncrease,
                        reducedPercent = 20,
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
            string dialogueString;

            switch (shopItem)
            {
                case ShopItem.HealthPotion:
                    dialogueString = "Gives a bit of health";
                    break;

                case ShopItem.ShotGun:
                    dialogueString = "High damage recoiling gun";
                    break;

                case ShopItem.ChargeGun:
                    dialogueString = "The longer you charge the stronger it becomes";
                    break;

                case ShopItem.BulletsFreezeEnemy:
                    dialogueString = "Bullets temporarily freeze enemies randomly";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(shopItem), shopItem, null);
            }

            return $"{dialogueString}. ({GetShopItemCost(shopItem)} Souls)";
        }

        public static int GetShopItemCost(ShopItem shopItem)
        {
            switch (shopItem)
            {
                case ShopItem.HealthPotion:
                    return 50;

                case ShopItem.BulletsFreezeEnemy:
                    return 175;

                case ShopItem.ShotGun:
                    return 120;

                case ShopItem.ChargeGun:
                    return 90;

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

                default:
                    throw new ArgumentOutOfRangeException(nameof(shopItem), shopItem, null);
            }
        }

        #endregion
    }
}