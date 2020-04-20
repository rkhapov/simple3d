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
        public readonly float ViewDistance = 30.0f;
        public readonly float MovingSpeed = 0.005f;

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
        public Vector2 Size { get; }
        public float DirectionAngle { get; set; }
        public Sprite Sprite => throw new InvalidOperationException("Player should not be drawn");

        public float Health { get; set; }
        public int MaxHealth { get; set; }
        public float Endurance { get; set; }
        public int MaxEndurance { get; set; }
        public float SpellPoints { get; set; }
        public int MaxSpellPoints { get; set; }
        public Weapon CurrentWeapon { get; set; }

        public abstract void OnWorldUpdate(Scene scene, float elapsedMilliseconds);

        public void ProcessAction(PlayerAction action, Scene scene, float elapsedMilliseconds)
        {
            switch (action)
            {
                case PlayerAction.LeftCameraTurn:
                    DoLeftTurn(scene, elapsedMilliseconds);
                    break;
                case PlayerAction.RightCameraTurn:
                    DoRightTurn(scene, elapsedMilliseconds);
                    break;
                case PlayerAction.MoveForward:
                    DoMoveForward(scene, elapsedMilliseconds);
                    break;
                case PlayerAction.MoveBackward:
                    DoMoveBackward(scene, elapsedMilliseconds);
                    break;
                case PlayerAction.MoveLeft:
                    DoMoveLeft(scene, elapsedMilliseconds);
                    break;
                case PlayerAction.MoveRight:
                    DoMoveRight(scene, elapsedMilliseconds);
                    break;
                case PlayerAction.MeleeLeftAttack:
                    DoMeleeLeftAttack(scene, elapsedMilliseconds);
                    break;
                case PlayerAction.MeleeRightAttack:
                    DoMeleeRightAttack(scene, elapsedMilliseconds);
                    break;
                case PlayerAction.Interact:
                    DoInteract(scene, elapsedMilliseconds);
                    break;
                case PlayerAction.Magic:
                    DoMagic(scene, elapsedMilliseconds);
                    break;
                case PlayerAction.Sprint:
                    DoSprint(scene, elapsedMilliseconds);
                    break;
                case PlayerAction.MeleeLeftBlock:
                    DoMeleeLeftBlock(scene, elapsedMilliseconds);
                    break;
                case PlayerAction.MeleeRightBlock:
                    DoMeleeRightBlock(scene, elapsedMilliseconds);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        protected virtual void DoMeleeRightBlock(Scene scene, in float elapsedMilliseconds)
        {
            throw new NotImplementedException();
        }

        protected virtual void DoMeleeLeftBlock(Scene scene, in float elapsedMilliseconds)
        {
            throw new NotImplementedException();
        }

        protected virtual void DoSprint(Scene scene, in float elapsedMilliseconds)
        {
            throw new NotImplementedException();
        }

        protected virtual void DoMagic(Scene scene, in float elapsedMilliseconds)
        {
            throw new NotImplementedException();
        }

        protected virtual void DoInteract(Scene scene, in float elapsedMilliseconds)
        {
            throw new NotImplementedException();
        }

        protected virtual void DoMeleeRightAttack(Scene scene, in float elapsedMilliseconds)
        {
            throw new NotImplementedException();
        }

        protected virtual void DoMeleeLeftAttack(Scene scene, in float elapsedMilliseconds)
        {
            throw new NotImplementedException();
        }

        protected virtual void DoMoveRight(Scene scene, in float elapsedMilliseconds)
        {
            var dx = MathF.Cos(DirectionAngle) * MovingSpeed * elapsedMilliseconds;
            var dy = MathF.Sin(DirectionAngle) * MovingSpeed * elapsedMilliseconds;

            TryMove(dx, -dy, scene, elapsedMilliseconds);
        }

        protected virtual void DoMoveLeft(Scene scene, in float elapsedMilliseconds)
        {
            var dx = MathF.Cos(DirectionAngle) * MovingSpeed * elapsedMilliseconds;
            var dy = MathF.Sin(DirectionAngle) * MovingSpeed * elapsedMilliseconds;

            TryMove(-dx, dy, scene, elapsedMilliseconds);
        }

        protected virtual void DoMoveBackward(Scene scene, in float elapsedMilliseconds)
        {
            var dx = MathF.Sin(DirectionAngle) * MovingSpeed * elapsedMilliseconds;
            var dy = MathF.Cos(DirectionAngle) * MovingSpeed * elapsedMilliseconds;

            TryMove(-dx, -dy, scene, elapsedMilliseconds);
        }

        protected virtual void DoMoveForward(Scene scene, in float elapsedMilliseconds)
        {
            var dx = MathF.Sin(DirectionAngle) * MovingSpeed * elapsedMilliseconds;
            var dy = MathF.Cos(DirectionAngle) * MovingSpeed * elapsedMilliseconds;

            TryMove(dx, dy, scene, elapsedMilliseconds);
        }

        protected virtual void DoRightTurn(Scene scene, in float elapsedMilliseconds)
        {
            DirectionAngle += 0.003f * elapsedMilliseconds;
        }

        protected virtual void DoLeftTurn(Scene scene, in float elapsedMilliseconds)
        {
            DirectionAngle -= 0.003f * elapsedMilliseconds;
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

            var dEndurance = (oldPosition - player.Position).Length() * elapsedMilliseconds * 0.03f;
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

                if (!map.InBound(testY, testX) || map.At(testY, testX).Type == MapCellType.Wall)
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
        }
    }
}