using System;
using Godot;
using StormTime.Common;
using StormTime.Player.Data;
using StormTime.Player.Modifiers;
using StormTime.Player.Shooting;
using StormTime.UI;
using StormTime.Utils;
using StormTime.Weapon;

namespace StormTime.Player.Movement
{
    public class PlayerController : KinematicBody2D
    {
        [Export] public float movementSpeed;
        [Export] public float defaultScaleAmount;

        // Float Effects
        [Export] public float minScaleAmount;
        [Export] public float maxScaleAmount;
        [Export] public float scaleChangeFrequency;
        [Export] public float rotationRate;
        [Export] public float lerpVelocity;

        // Other Controls
        [Export] public NodePath playerShootingNodePath;
        [Export] public NodePath playerHealthSetterNodePath;

        // Souls Manager
        [Export] public int lowSoulsCount = 5;
        [Export] public float lowSoulsHealthDecrementRate;
        [Export] public float lowHealthWarningPercent = 0.3f;
        [Export] public Color lowHealthWarningColor;

        // ShotGun Weapon Recoil
        [Export] public float shotGunRecoilForce;
        [Export] public float shotGunRecoilAffectTime;

        public enum PlayerState
        {
            PlayerInControlMovement,
            PlayerFloatingMovement,
            PlayerDisabled
        }

        private PlayerShooting _playerShooting;
        private HealthSetter _playerHealthSetter;

        private PlayerState _playerState;
        private Vector2 _movement;
        private float _currentMovementSpeed;

        private Vector2 _targetScale;
        private Vector2 _lerpPosition;

        private float _playerTime;
        private float _currentShotGunRecoilTime;

        private bool _isSoulsLow;

        public override void _Ready()
        {
            _playerShooting = GetNode<PlayerShooting>(playerShootingNodePath);
            _playerHealthSetter = GetNode<HealthSetter>(playerHealthSetterNodePath);

            _movement = new Vector2();
            _currentMovementSpeed = movementSpeed;
            _targetScale = Vector2.One * defaultScaleAmount;
            _lerpPosition = new Vector2();

            SetPlayerState(PlayerState.PlayerInControlMovement);

            _playerHealthSetter.healthChanged += HandleHeathChange;
            _playerShooting.bulletShot += HandleBulletShot;
            PlayerModifierSoulsManager.instance.handleStatusChanged += HandleSoulsChange;
        }

        public override void _Process(float delta) => _playerTime += delta;

        public override void _PhysicsProcess(float delta)
        {
            SetScale(GetScale().LinearInterpolate(_targetScale, lerpVelocity * delta));
            DecrementHealthOnLowSouls(delta);

            switch (_playerState)
            {
                case PlayerState.PlayerInControlMovement:
                    HandlePlayerControlMovement(delta);
                    break;

                case PlayerState.PlayerFloatingMovement:
                    HandlePlayerFloatingMovement(delta);
                    break;

                case PlayerState.PlayerDisabled:
                    HandlePlayerDisabled(delta);
                    break;
            }
        }

        #region Player Controls

        private void HandlePlayerControlMovement(float delta)
        {
            RotatePlayer(delta);
            MovePlayer(delta);
        }

        private void HandlePlayerFloatingMovement(float delta)
        {
            ConstantRotatePlayer(delta);
            FloatingScaleChange(delta);
            LerpPlayerToPosition(delta);
        }

        private void HandlePlayerDisabled(float delta)
        {

        }

        #region Player Control Movement

        private void RotatePlayer(float delta)
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            LookAt(mousePosition);
        }

        private void MovePlayer(float delta)
        {
            if (_currentShotGunRecoilTime > 0)
            {
                _currentShotGunRecoilTime -= delta;
            }
            else
            {
                if (Input.IsActionPressed(SceneControls.Left))
                    _movement.x = -_currentMovementSpeed;
                else if (Input.IsActionPressed(SceneControls.Right))
                    _movement.x = _currentMovementSpeed;
                else
                    _movement.x = 0;

                if (Input.IsActionPressed(SceneControls.Up))
                    _movement.y = -_currentMovementSpeed;
                else if (Input.IsActionPressed(SceneControls.Down))
                    _movement.y = _currentMovementSpeed;
                else
                    _movement.y = 0;
            }

            _movement = MoveAndSlide(_movement);
            PlayerVariables.LastPlayerPosition = GetGlobalPosition();
        }

        #endregion

        #region Player Floating Movement

        private void ConstantRotatePlayer(float delta) => Rotate(Mathf.Deg2Rad(rotationRate * delta));

        private void FloatingScaleChange(float delta)
        {
            float scaleMultiplier = Mathf.Sin(scaleChangeFrequency * _playerTime);
            float scaleAmount = ExtensionFunctions.Map(scaleMultiplier, -1, 1,
                minScaleAmount, maxScaleAmount);

            _targetScale = Vector2.One * scaleAmount;
        }

        private void LerpPlayerToPosition(float delta) => SetGlobalPosition(GetGlobalPosition().LinearInterpolate(_lerpPosition, lerpVelocity * delta));

        #endregion

        #endregion

        #region External Functions

        public void BulletCollisionNotification(object bullet, bool isFreezingBullet)
        {
            if (_playerState == PlayerState.PlayerDisabled)
            {
                return;
            }

            bool isPlayerBullet = !(bullet is EnemyBullet);

            if (!isPlayerBullet)
            {
                float damageAmount = ((Bullet)bullet).GetBulletDamage();
                _playerHealthSetter.SubtractHealth(damageAmount);
            }
        }

        public void ResetSizeDefaults() => _targetScale = Vector2.One * defaultScaleAmount;

        public void HandleSacrificialItemInfluence(PlayerModifierTypes.SacrificialItemInfo sacrificialItemInfo)
        {
            switch (sacrificialItemInfo.sacrificialItem)
            {
                case PlayerModifierTypes.SacrificialItem.SpeedSacrificeHealthBoost:
                    {
                        float speedLossAmount = _currentMovementSpeed * sacrificialItemInfo.reducedPercent / 100;
                        _currentMovementSpeed -= speedLossAmount;

                        float maxHealthIncrease =
                            _playerHealthSetter.GetMaxHealth() * sacrificialItemInfo.increasedPercent / 100;

                        _playerHealthSetter.SetMaxHealth(_playerHealthSetter.GetMaxHealth() + maxHealthIncrease);
                    }
                    break;

                case PlayerModifierTypes.SacrificialItem.SpeedSacrificeDamageIncrease:
                    {
                        float speedLossAmount = _currentMovementSpeed * sacrificialItemInfo.reducedPercent / 100;
                        _currentMovementSpeed -= speedLossAmount;

                        _playerShooting.AddToDamageDifferencePercent(sacrificialItemInfo.increasedPercent);
                    }
                    break;

                case PlayerModifierTypes.SacrificialItem.HealthSacrificeDamageIncrease:
                    {
                        float healthLossAmount =
                            _playerHealthSetter.GetMaxHealth() * sacrificialItemInfo.reducedPercent / 100;
                        _playerHealthSetter.SetMaxHealth(_playerHealthSetter.GetMaxHealth() - healthLossAmount);

                        _playerShooting.AddToDamageDifferencePercent(sacrificialItemInfo.increasedPercent);
                    }
                    break;

                case PlayerModifierTypes.SacrificialItem.ShootTimeSacrificeDamageIncrease:
                    {
                        float shootingTimeIncrease = _playerShooting.GetCurrentShootingDelay() *
                                                     sacrificialItemInfo.reducedPercent / 100;
                        _playerShooting.SetCurrentShootingDelay(_playerShooting.GetCurrentShootingDelay() + shootingTimeIncrease);

                        _playerShooting.AddToDamageDifferencePercent(sacrificialItemInfo.increasedPercent);
                    }
                    break;

                case PlayerModifierTypes.SacrificialItem.HealthSacrificeSpeedIncrease:
                    {
                        float healthLossAmount =
                            _playerHealthSetter.GetMaxHealth() * sacrificialItemInfo.reducedPercent / 100;
                        _playerHealthSetter.SetMaxHealth(_playerHealthSetter.GetMaxHealth() - healthLossAmount);

                        float speedIncrease = _currentMovementSpeed * sacrificialItemInfo.increasedPercent / 100;
                        _currentMovementSpeed += speedIncrease;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void HandleShopItemInfluence(PlayerModifierTypes.ShopItemInfo shopItemInfo)
        {
            switch (shopItemInfo.shopItem)
            {
                case PlayerModifierTypes.ShopItem.HealthPotion:
                    {
                        float potionHealthAmount = shopItemInfo.valueChange;
                        _playerHealthSetter.AddHealth(potionHealthAmount);
                    }
                    break;

                case PlayerModifierTypes.ShopItem.BulletsFreezeEnemy:
                    _playerShooting.BuyFreezingBullet();
                    break;

                case PlayerModifierTypes.ShopItem.ShotGun:
                    _playerShooting.UnlockWeapon(PlayerShooting.WeaponType.Shotgun);
                    break;

                case PlayerModifierTypes.ShopItem.ChargeGun:
                    _playerShooting.UnlockWeapon(PlayerShooting.WeaponType.ChargeGun);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetLerpPosition(Vector2 position) => _lerpPosition = position;

        public void ActivateShooting() => _playerShooting.ActivateShooting();

        public void DeActivateShooting() => _playerShooting.DeActivateShooting();

        public float GetPlayerMovementSpeed() => _currentMovementSpeed;

        public void SetPlayerState(PlayerState playerState)
        {
            if (_playerState == playerState)
            {
                return;
            }

            _playerState = playerState;
        }

        #endregion

        #region Utility Functions

        private void HandleBulletShot(PlayerShooting.WeaponType weaponType)
        {
            switch (weaponType)
            {
                case PlayerShooting.WeaponType.SingleShot:
                    break;

                case PlayerShooting.WeaponType.Shotgun:
                    {
                        float currentRotation = GetRotation() - Mathf.Pi;
                        float xVelocity = Mathf.Cos(currentRotation);
                        float yVelocity = Mathf.Sin(currentRotation);

                        Vector2 reverseVelocity = new Vector2(xVelocity, yVelocity);
                        reverseVelocity.x *= shotGunRecoilForce;
                        reverseVelocity.y *= shotGunRecoilForce;

                        _movement = reverseVelocity;
                        _currentShotGunRecoilTime = shotGunRecoilAffectTime;
                    }
                    break;

                case PlayerShooting.WeaponType.ChargeGun:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(weaponType), weaponType, null);
            }
        }

        private void HandleHeathChange(float currentHealth, float maxHealth)
        {
            float heathRatio = currentHealth / maxHealth;
            if (heathRatio <= lowHealthWarningPercent)
            {
                WarningManager.instance.StartWarning(this, lowHealthWarningColor);
            }
            else
            {
                WarningManager.instance.StopWarning(this);
            }
        }

        private void HandleSoulsChange(int currentSouls) => _isSoulsLow = currentSouls <= lowSoulsCount;

        private void DecrementHealthOnLowSouls(float delta)
        {
            if (!_isSoulsLow)
            {
                return;
            }

            float decrementAmount = lowSoulsHealthDecrementRate * delta;
            _playerHealthSetter.SubtractHealth(decrementAmount);
        }

        #endregion
    }
}
