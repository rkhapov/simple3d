using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using simple3d.Drawing;
using simple3d.MathUtils;

namespace simple3d.Levels
{
    public abstract class Player : IMapObject
    {
        public readonly float FieldOfView = MathF.PI / 3;
        public readonly float ViewDistance = 60.0f;
        public const float WalkMovingSpeed = 0.003f;
        public const float SprintMovingSpeed = 0.005f;
        public float MovingSpeed { get; private set; } = WalkMovingSpeed;

        protected Player(Vector2 position, Vector2 size, float directionAngle)
        {
            Position = position;
            Size = size;
            DirectionAngle = directionAngle;
            MaxHealth = 32;
            Health = MaxHealth;
            MaxEndurance = 32;
            Endurance = MaxEndurance;
            MaxSpellPoints = 32;
            SpellPoints = MaxSpellPoints;
        }

        public Vector2 Position { get; set; }
        private Vector2 oldEPosition = new Vector2(-1, -1);
        public bool interactMod;
        public Vector2 Size { get; }
        public float DirectionAngle { get; set; }
        public Sprite Sprite => throw new InvalidOperationException("Player should not be drawn");

        public float Health { get; set; }
        public int MaxHealth { get; set; }
        public float Endurance { get; set; }
        public int MaxEndurance { get; set; }
        public float SpellPoints { get; set; }
        public int MaxSpellPoints { get; set; }
        public Weapon Weapon => Weapons[WeaponIndex];
        public Weapon[] Weapons { get; set; }
        public int WeaponIndex { get; set; }

        public void SetWeaponToNext()
        {
            Weapon.GoStatic();

            WeaponIndex++;

            if (WeaponIndex == Weapons.Length)
                WeaponIndex = 0;
        }

        public enum PlayerState
        {
            Normal,
            TextReading
        }

        public PlayerState State { get; private set; } = PlayerState.Normal;

        public string LastNoteText { get; private set; }

        public Monologue CurrentMonologue { get; set; }

        public void DoReadText(string text)
        {
            State = PlayerState.TextReading;
            LastNoteText = text;
        }

        public void ProcessAction(PlayerAction action, Scene scene, float elapsedMilliseconds)
        {
            switch (action.Type)
            {
                case PlayerActionType.CameraTurnLeft:
                    DoLeftTurn(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.CameraTurnRight:
                    DoRightTurn(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.MoveForward:
                    DoMoveForward(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.MoveBackward:
                    DoMoveBackward(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.MoveLeft:
                    DoMoveLeft(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.MoveRight:
                    DoMoveRight(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.MeleeLeftAttack:
                    DoMeleeLeftAttack(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.MeleeRightAttack:
                    DoMeleeRightAttack(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.Interact:
                    DoInteract(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.Magic:
                    DoMagic(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.Sprint:
                    DoSprint(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.MeleeLeftBlock:
                    DoMeleeLeftBlock(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.MeleeRightBlock:
                    DoMeleeRightBlock(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.Shoot:
                    DoShoot(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.SetWeapon:
                    DoSetWeapon(action, scene, elapsedMilliseconds);
                    break;
                case PlayerActionType.Cancel:
                    State = PlayerState.Normal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action.Type), action.Type, null);
            }
        }

        public virtual void DoSetWeapon(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            if (!action.Enabled)
                return;

            SetWeaponToNext();
        }

        public virtual void DoShoot(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            if (!action.Enabled)
                return;

            if (!(Weapon is ShootingWeapon shootingWeapon))
                return;

            shootingWeapon.MakeShoot(scene);
        }

        protected virtual void DoMeleeRightBlock(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            if (!(Weapon is MeleeWeapon meleeWeapon))
                return;

            if (action.Enabled)
                meleeWeapon.State = MeleeWeaponState.BlockRight;
            else
                meleeWeapon.State = MeleeWeaponState.Static;
        }

        protected virtual void DoMeleeLeftBlock(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            if (!(Weapon is MeleeWeapon meleeWeapon))
                return;

            if (action.Enabled)
                meleeWeapon.State = MeleeWeaponState.BlockLeft;
            else
                meleeWeapon.State = MeleeWeaponState.Static;
        }

        protected virtual void DoSprint(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            MovingSpeed = action.Enabled ? SprintMovingSpeed : WalkMovingSpeed;
        }

        protected virtual void DoMagic(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            throw new NotImplementedException();
        }

        protected virtual void DoInteract(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            interactMod = true;
            oldEPosition = new Vector2((int)Position.X, (int)Position.Y);
        }

        protected virtual void DoMeleeRightAttack(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            if (!(Weapon is MeleeWeapon meleeWeapon) || meleeWeapon.State == MeleeWeaponState.AttackRight)
                return;

            meleeWeapon.DoRightAttack(scene);
        }

        protected virtual void DoMeleeLeftAttack(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            if (!(Weapon is MeleeWeapon meleeWeapon) || meleeWeapon.State == MeleeWeaponState.AttackLeft)
                return;

            meleeWeapon.DoLeftAttack(scene);
        }

        protected virtual void DoMoveRight(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            var dx = MathF.Cos(DirectionAngle) * MovingSpeed * elapsedMilliseconds;
            var dy = MathF.Sin(DirectionAngle) * MovingSpeed * elapsedMilliseconds;

            TryMove(dx, -dy, scene, elapsedMilliseconds);
        }

        protected virtual void DoMoveLeft(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            var dx = MathF.Cos(DirectionAngle) * MovingSpeed * elapsedMilliseconds;
            var dy = MathF.Sin(DirectionAngle) * MovingSpeed * elapsedMilliseconds;

            TryMove(-dx, dy, scene, elapsedMilliseconds);
        }

        protected virtual void DoMoveBackward(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            var dx = MathF.Sin(DirectionAngle) * MovingSpeed * elapsedMilliseconds;
            var dy = MathF.Cos(DirectionAngle) * MovingSpeed * elapsedMilliseconds;

            TryMove(-dx, -dy, scene, elapsedMilliseconds);
        }

        protected virtual void DoMoveForward(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            if (!action.Enabled)
                return;

            var dx = MathF.Sin(DirectionAngle) * MovingSpeed * elapsedMilliseconds;
            var dy = MathF.Cos(DirectionAngle) * MovingSpeed * elapsedMilliseconds;

            TryMove(dx, dy, scene, elapsedMilliseconds);
        }

        protected virtual void DoRightTurn(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            DirectionAngle += 0.0025f * elapsedMilliseconds;
        }

        protected virtual void DoLeftTurn(PlayerAction action, Scene scene, in float elapsedMilliseconds)
        {
            DirectionAngle -= 0.0025f * elapsedMilliseconds;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TryMove(float dx, float dy, Scene scene, float elapsedMilliseconds)
        {
            //TODO: implement with physics engine?
            var player = scene.Player;
            var map = scene.Map;
            var oldPosition = player.Position;
            var newPosition = player.Position + new Vector2(dx, 0);

            TryMove(scene, newPosition, map);

            newPosition = player.Position + new Vector2(0, dy);

            TryMove(scene, newPosition, map);

            var dEndurance = (oldPosition - player.Position).Length() * 0.3f;
            if (player.Endurance > dEndurance)
            {
                player.Endurance -= dEndurance;
            }
            else
            {
                player.Position = oldPosition;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TryMove(Scene scene, Vector2 newPosition, Map map)
        {
            var playerNewVertices = GeometryHelper.GetRotatedVertices(newPosition, Size, DirectionAngle);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var vertex in playerNewVertices)
            {
                var testX = (int) vertex.X;
                var testY = (int) vertex.Y;

                if (!map.InBound(testY, testX) || !CellTypes.walkable.Contains(map.At(testY, testX).Type))
                {
                    return;
                }
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var objectVertices in scene
                .Objects
                .Select(o => o.GetRotatedVertices()))
            {
                if (GeometryHelper.IsRectanglesIntersects(playerNewVertices, objectVertices))
                    return;
            }

            Position = newPosition;

            Trigger.CheckAndDo(Position, scene);
        }

        public virtual void OnWorldUpdate(Scene scene, float elapsedMilliseconds)
        {
            if (Weapon == null)
                return;

            Weapon.UpdateAnimation(elapsedMilliseconds);

            if (Weapon.AnimationIsOver)
            {
                if (Weapon is ShootingWeapon shootingWeapon && shootingWeapon.State == ShootingWeaponState.Shooting)
                {
                    shootingWeapon.SpawnArrow(scene);
                }

                Weapon.GoStatic();
            }

            if ((int)Position.X != oldEPosition.X || (int)Position.Y != oldEPosition.Y)
            {
                interactMod = false;
            }
        }

        public abstract void OnLeftMeleeAttack(Scene scene, int damage);

        public abstract void OnRightMeleeAttack(Scene scene, int damage);

        public abstract void OnShoot(Scene scene, int damage);
    }
}