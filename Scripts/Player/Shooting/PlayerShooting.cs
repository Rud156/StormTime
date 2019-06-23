using System;
using System.Collections.Generic;
using Godot;
using StormTime.Player.Modifiers;
using StormTime.Player.Movement;
using StormTime.UI;
using StormTime.Utils;
using StormTime.Weapon;

namespace StormTime.Player.Shooting
{
    public class PlayerShooting : Node2D
    {
        // Normal Shooting Stats
        [Export] public NodePath playerBulletHolderNodePath;
        [Export] public PackedScene playerBulletPrefab;
        [Export] public float shootDelay;
        [Export] public int shootSoulDecrementCount = 1;
        [Export] public float weaponLockedErrorDisplayTime = 3;

        // Shot Gun Specific Stats
        [Export] public float shotGunAngleDiff = 20;
        [Export] public int shotGunBulletCount = 5;

        private Node2D _playerBulletHolder;
        private PlayerController _playerRoot;

        private bool _shootingActive;
        private float _currentShootDelay;
        private float _currentShootTimeLeft;

        private float _currentDamageDiff; // Add this value to the original damage value of the bullet
        private float _currentDamageDiffPercent;

        public enum WeaponType
        {
            SingleShot,
            Shotgun,
            ChargeGun
        }

        private struct WeaponState
        {
            public WeaponType weaponType;
            public bool isUnlocked;
        }

        private Dictionary<WeaponType, WeaponState> _playerWeapons;
        private WeaponType _currentWeaponIndex = WeaponType.SingleShot;

        public override void _Ready()
        {
            _shootingActive = true;
            _currentShootDelay = shootDelay;

            _playerBulletHolder = GetNode<Node2D>(playerBulletHolderNodePath);
            _playerRoot = GetParent<PlayerController>();

            _playerWeapons = new Dictionary<WeaponType, WeaponState>
            {
                {WeaponType.SingleShot, new WeaponState() {weaponType = WeaponType.SingleShot, isUnlocked = true}},
                {WeaponType.Shotgun, new WeaponState() {weaponType = WeaponType.Shotgun, isUnlocked = true}},
                {WeaponType.ChargeGun, new WeaponState() {weaponType = WeaponType.ChargeGun, isUnlocked = true}}
            };
        }

        public override void _Process(float delta)
        {
            _currentShootTimeLeft -= delta;

            if (!_shootingActive)
            {
                return;
            }

            if (Input.IsActionJustPressed(SceneControls.Shoot) && _currentShootTimeLeft <= 0 &&
                PlayerModifierSoulsManager.instance.HasSouls())
            {
                HandleWeaponShooting();
            }

            HandlePlayerWeaponSwitch();
        }

        #region Utility Functions

        private void HandleWeaponShooting()
        {
            switch (_currentWeaponIndex)
            {
                case WeaponType.SingleShot:
                    float currentRotation = _playerRoot.GetRotation();
                    float xVelocity = Mathf.Cos(currentRotation);
                    float yVelocity = Mathf.Sin(currentRotation);
                    ShootSingleShotBullet(new Vector2(xVelocity, yVelocity));
                    break;

                case WeaponType.Shotgun:
                    ShootShotGunBullet();
                    break;

                case WeaponType.ChargeGun:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandlePlayerWeaponSwitch()
        {
            int weaponIndex = -1;
            if (Input.IsActionJustPressed(SceneControls.PlayerWeaponSwitch_1))
            {
                weaponIndex = 0;
            }
            else if (Input.IsActionJustPressed(SceneControls.PlayerWeaponSwitch_2))
            {
                weaponIndex = 1;
            }
            else if (Input.IsActionJustPressed(SceneControls.PlayerWeaponSwitch_3))
            {
                weaponIndex = 2;
            }

            if (weaponIndex != -1)
            {
                WeaponType weaponType = (WeaponType)weaponIndex;
                WeaponState weaponState = _playerWeapons[weaponType];

                if (weaponState.isUnlocked)
                {
                    _currentWeaponIndex = weaponType;
                }
                else
                {
                    DialogueUiManager.instance.DisplaySingleStringTimed("Weapon not yet owned",
                        weaponLockedErrorDisplayTime);
                }
            }
        }

        private void ShootSingleShotBullet(Vector2 forwardVectorNormalized, bool decrementSouls = true)
        {
            Bullet bulletInstance = (Bullet)playerBulletPrefab.Instance();
            _playerBulletHolder.AddChild(bulletInstance);

            if (_currentDamageDiffPercent != 0)
            {
                float damageChange = bulletInstance.GetBulletDamage() * _currentDamageDiffPercent / 100;
                _currentDamageDiff += damageChange;
                _currentDamageDiffPercent = 0;
            }

            bulletInstance.SetBulletDamage(bulletInstance.GetBulletDamage() + _currentDamageDiff);
            bulletInstance.SetGlobalPosition(_playerRoot.GetGlobalPosition());
            bulletInstance.LaunchBullet(forwardVectorNormalized);

            _currentShootTimeLeft = _currentShootDelay;
            
            if (decrementSouls)
            {
                PlayerModifierSoulsManager.instance.DecrementSouls(shootSoulDecrementCount);
            }
        }

        private void ShootShotGunBullet()
        {
            float currentRotation = _playerRoot.GetRotationDegrees();
            float currentAngle = currentRotation - shotGunBulletCount / 2.0f * shotGunAngleDiff;

            for (int i = 0; i < shotGunBulletCount; i++)
            {
                float xVelocity = Mathf.Cos(Mathf.Deg2Rad(currentAngle));
                float yVelocity = Mathf.Sin(Mathf.Deg2Rad(currentAngle));

                Vector2 launchVelocity = new Vector2(xVelocity, yVelocity);
                ShootSingleShotBullet(launchVelocity.Normalized(), false);

                currentAngle += shotGunAngleDiff;
            }

            PlayerModifierSoulsManager.instance.DecrementSouls(shootSoulDecrementCount);
        }

        #endregion

        #region External Functions

        public void UnlockWeapon(WeaponType weaponType)
        {
            WeaponState weaponState = _playerWeapons[weaponType];

            weaponState.isUnlocked = true;
            _playerWeapons[weaponType] = weaponState;
        }

        public void ActivateShooting() => _shootingActive = true;

        public void DeActivateShooting() => _shootingActive = false;

        public float GetCurrentShootingDelay() => _currentShootDelay;

        public void SetCurrentShootingDelay(float shootingDelay) => _currentShootDelay = shootingDelay;

        public void AddToDamageDifferencePercent(float percentIncrease) => _currentDamageDiffPercent += percentIncrease;

        #endregion
    }
}