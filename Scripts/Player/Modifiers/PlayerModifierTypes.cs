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
            HealthSacrificeSpeedIncrease,
            BulletsFreezeEnemy
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

                default:
                    throw new ArgumentOutOfRangeException(nameof(shopItem), shopItem, null);
            }
        }

        public static string GetSacrificialItemDescription(SacrificialItem sacrificialItem)
        {
            switch (sacrificialItem)
            {
                case SacrificialItem.SpeedSacrificeHealthBoost:
                    return "Become slower to but increase maximum health";

                case SacrificialItem.SpeedSacrificeDamageIncrease:
                    return "Become slower but add 20% to weapon damage";

                case SacrificialItem.HealthSacrificeDamageIncrease:
                    return "Sacrifice max health but add 30% to weapon damage";

                case SacrificialItem.ShootTimeSacrificeDamageIncrease:
                    return "Shoot slowly but increase damage by 25%";

                case SacrificialItem.HealthSacrificeSpeedIncrease:
                    return "Sacrifice max health but increase speed by 20%";
                    
                case SacrificialItem.BulletsFreezeEnemy:
                    return "Bullets temporarily freeze enemies randomly";

                default:
                    throw new ArgumentOutOfRangeException(nameof(sacrificialItem), sacrificialItem, null);
            }
        }
    }
}