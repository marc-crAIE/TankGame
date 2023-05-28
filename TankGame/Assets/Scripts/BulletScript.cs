using RayEngine.Core;
using RayEngine.GameObjects;
using RayEngine.GameObjects.Components;
using SharpMaths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Assets.Scripts
{
    internal class BulletScript : ScriptableGameObject
    {
        private float Angle = 0.0f;
        private Vector2 Velocity;
        private TransformComponent _Transform;

        private const float Speed = 1500.0f;
        private const float Size = 5;

        public BulletScript(float angle)
        {
            Angle = angle;
            Velocity = new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
            _Transform = new TransformComponent();
        }

        public override void OnCreate()
        {
            AddComponent<SpriteComponent>(new Colour(255, 0, 255));

            _Transform = Transform;
            _Transform.Scale = new Vector2(Size);
        }

        public override void OnUpdate(Timestep ts)
        {
            _Transform.Translation += Velocity * (Speed * ts);
        }
    }
}
