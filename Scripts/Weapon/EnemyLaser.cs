using System.Collections.Generic;
using System;
using Godot;
using StormTime.Player.Movement;

namespace StormTime.Weapon
{
    public class EnemyLaser : EnemyBullet
    {
        [Export] public float timeBetweenDamage;
        [Export] public float laserStartUpTime;
        [Export] public float laserDestroyAfterTime;
        [Export] public NodePath laserAreaNodePath;
        [Export] public Godot.Collections.Array<NodePath> laserLineEffectNodePaths;

        private enum LaserState
        {
            Launched,
            Running,
            Dying
        }

        private LaserState _currentLaserState;
        private float _currentLaserTimer;

        private Area2D _laserArea;
        private List<Particles2D> _laserLineEffects;
        private PlayerController _playerController;

        public override void _Ready()
        {
            base._Ready();

            _laserArea = GetNode<Area2D>(laserAreaNodePath);
            _laserArea.Connect("body_entered", this, nameof(HandleBodyEntered));
            _laserArea.Connect("body_exited", this, nameof(HandleBodyExited));

            _laserLineEffects = new List<Particles2D>();
            foreach (NodePath laserLineEffectNodePath in laserLineEffectNodePaths)
            {
                Particles2D particleEffect = GetNode<Particles2D>(laserLineEffectNodePath);
                particleEffect.SetEmitting(true);
                _laserLineEffects.Add(particleEffect);
            }

            _currentLaserTimer = laserStartUpTime;
            SetLaserState(LaserState.Launched);
        }

        public override void _ExitTree()
        {
            _laserArea.Disconnect("body_entered", this, nameof(HandleBodyEntered));
            _laserArea.Disconnect("body_exited", this, nameof(HandleBodyExited));
        }

        public override void _Process(float delta)
        {
            switch (_currentLaserState)
            {
                case LaserState.Launched:
                    HandleLaunchedStateUpdate(delta);
                    break;

                case LaserState.Running:
                    HandleRunningStateUpdate(delta);
                    break;

                case LaserState.Dying:
                    HandleDyingStateUpdate(delta);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_currentLaserState), _currentLaserState, null);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            // Do Nothing Here...
            // This ensures that the laser is destroyed only using the DestroyLaser function
        }


        #region External Functions

        public override void LaunchBullet(Vector2 forwardVectorNormalized)
        {

        }

        public void DestroyLaser()
        {
            foreach (Particles2D laserLineEffect in _laserLineEffects)
            {
                laserLineEffect.SetEmitting(false);
            }

            _currentLaserTimer = laserDestroyAfterTime;
        }

        #endregion

        #region Utility Functions


        #region State Updates

        private void HandleLaunchedStateUpdate(float delta)
        {
            _currentLaserTimer -= delta;
            if (_currentLaserTimer <= 0)
            {
                _currentLaserTimer = timeBetweenDamage;
                SetLaserState(LaserState.Running);
            }
        }

        private void HandleRunningStateUpdate(float delta)
        {
            _currentLaserTimer -= delta;
            if (_currentLaserTimer <= 0)
            {
                _currentLaserTimer = timeBetweenDamage;
                _playerController?.TakeExternalDamage(GetBulletDamage());
            }
        }

        private void HandleDyingStateUpdate(float delta)
        {
            _currentLaserTimer -= delta;
            if (_currentLaserTimer <= 0)
            {
                base.RemoveBulletFromTree();
            }
        }

        #endregion

        private void HandleBodyEntered(PhysicsBody2D other)
        {
            if (!(other is PlayerController))
            {
                return;
            }

            if (_currentLaserState == LaserState.Running)
            {
                // Reset timer just when the player enters the collider
                _currentLaserTimer = 0;
            }
            _playerController = (PlayerController)other;
        }

        private void HandleBodyExited(PhysicsBody2D other)
        {
            if (!(other is PlayerController))
            {
                return;
            }

            _playerController = null;
        }

        private void SetLaserState(LaserState laserState)
        {
            if (_currentLaserState == laserState)
            {
                return;
            }

            _currentLaserState = laserState;
        }

        #endregion
    }
}