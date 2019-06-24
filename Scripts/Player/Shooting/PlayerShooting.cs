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
        [Export] public PackedScene playerChargedBulletPrefab;
        [Export] public NodePath playerShootingPositionNodePath;
        [Export] public NodePath playerChargedShootingPositionNodePath;
        [Export] public float shootDelay;
        [Export] public int singleShotSoulDecrementCount = 1;
        [Export] public float weaponLockedErrorDisplayTime = 3;

        // Shot Gun Specific Stats
        [Export] public float shotGunAngleDiff = 20;
        [Export] public int shotGunBulletCount = 5;
        [Export] public int shotGunShotSoulDecrementCount = 3;

        // Charge Gun Specific Stats
        [Export] public float chargeGunMaxScaleAmount = 7;
        [Export] public float chargeGunScaleIncrementRate;
        [Export] public int chargedShotSoulDecrementCount = 5;

        private Node2D _playerBulletHolder;
        private Node2D _playerBulletShootingPosition;
        private Node2D _playerChargedShotShootingPosition;
        private PlayerController _playerRoot;

        private bool _shootingActive;
        private float _currentShootDelay;
        private float _currentShootTimeLeft;

        private float _currentDamageDiff; // Add this value to the original damage value of the bullet
        private float _currentDamageDiffPercent;

        // Charged Shot Weapon
        private float _chargeWeaponCurrentScale;
        private bool _chargeWeaponActive;
        private Bullet _chargedShotBullet;

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
        private WeaponType _currentWeaponType = WeaponType.SingleShot;

        public override void _Ready()
        {
            _shootingActive = true;
            _currentShootDelay = shootDelay;

            _playerBulletHolder = GetNode<Node2D>(playerBulletHolderNodePath);
            _playerBulletShootingPosition = GetNode<Node2D>(playerShootingPositionNodePath);
            _playerChargedShotShootingPosition = GetNode<Node2D>(playerChargedShootingPositionNodePath);
            _playerRoot = GetParent<PlayerController>();

            _playerWeapons = new Dictionary<WeaponType, WeaponState>
            {
                {WeaponType.SingleShot, new WeaponState() {weaponType = WeaponType.SingleShot, isUnlocked = true}},
                {WeaponType.Shotgun, new WeaponState() {weaponType = WeaponType.Shotgun, isUnlocked = false}},
                {WeaponType.ChargeGun, new WeaponState() {weaponType = WeaponType.ChargeGun, isUnlocked = false}}
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

            if (_chargeWeaponActive && _currentWeaponType == WeaponType.ChargeGun)
            {
                if (Input.IsActionPressed(SceneControls.Shoot))
                {
                    _chargeWeaponCurrentScale += delta * chargeGunScaleIncrementRate;
                    if (_chargeWeaponCurrentScale > chargeGunMaxScaleAmount)
                    {
                        _chargeWeaponCurrentScale = chargeGunMaxScaleAmount;
                    }

                    _chargedShotBullet.SetGlobalScale(Vector2.One * _chargeWeaponCurrentScale);
                }
                else if (Input.IsActionJustReleased(SceneControls.Shoot))
                {
                    ShootChargedBullet();
                }
            }

            HandlePlayerWeaponSwitch();
        }

        #region Utility Functions

        private void HandleWeaponShooting()
        {
            switch (_currentWeaponType)
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
                    {
                        _chargeWeaponCurrentScale = 1;
                        _chargeWeaponActive = true;

                        _chargedShotBullet = (Bullet)playerChargedBulletPrefab.Instance();
                        _playerChargedShotShootingPosition.AddChild(_chargedShotBullet);

                        _chargedShotBullet.SetGlobalPosition(_playerChargedShotShootingPosition.GetGlobalPosition());
                        _chargedShotBullet.SetAsStaticBullet();
                    }

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
                    _currentWeaponType = weaponType;
                }
                else
                {
                    DialogueUiManager.instance
                        .DisplaySingleStringTimed($"{weaponType.ToString()} weapon not yet owned",
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
            bulletInstance.SetGlobalPosition(_playerBulletShootingPosition.GetGlobalPosition());
            bulletInstance.LaunchBullet(forwardVectorNormalized);

            _currentShootTimeLeft = _currentShootDelay;

            if (decrementSouls)
            {
                PlayerModifierSoulsManager.instance.DecrementSouls(singleShotSoulDecrementCount);
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

            PlayerModifierSoulsManager.instance.DecrementSouls(shotGunShotSoulDecrementCount);
        }

        private void ShootChargedBullet()
        {
            if (_chargedShotBullet == null)
            {
                return;
            }

            if (_currentDamageDiffPercent != 0)
            {
                float damageChange = _chargedShotBullet.GetBulletDamage() * _currentDamageDiffPercent / 100;
                _currentDamageDiff += damageChange;
                _currentDamageDiffPercent = 0;
            }

            _playerChargedShotShootingPosition.RemoveChild(_chargedShotBullet);
            _playerBulletHolder.AddChild(_chargedShotBullet);

            _chargedShotBullet.SetBulletDamage(_chargedShotBullet.GetBulletDamage() + _currentDamageDiff);
            _chargedShotBullet.SetGlobalPosition(_playerChargedShotShootingPosition.GetGlobalPosition());
            _chargedShotBullet.SetGlobalScale(Vector2.One * _chargeWeaponCurrentScale);

            float currentRotation = _playerRoot.GetRotation();
            _chargedShotBullet.SetGlobalRotation(currentRotation);
            float xVelocity = Mathf.Cos(currentRotation);
            float yVelocity = Mathf.Sin(currentRotation);
            _chargedShotBullet.SetAsDynamicBullet();
            _chargedShotBullet.LaunchBullet(new Vector2(xVelocity, yVelocity), false);

            _chargedShotBullet = null;
            _chargeWeaponActive = false;
            _currentShootTimeLeft = _currentShootDelay;

            PlayerModifierSoulsManager.instance.DecrementSouls(chargedShotSoulDecrementCount);
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