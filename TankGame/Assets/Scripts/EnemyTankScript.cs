using RayEngine.Core;
using RayEngine.GameObjects;
using RayEngine.GameObjects.Components;
using SharpMaths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Components;

namespace TankGame.Assets.Scripts
{
    internal class EnemyTankScript : ScriptableGameObject
    {
        private TransformComponent _Transform;

        private Colour Colour = new Colour(255, 255, 255);

        private const float MovementSpeed = 75.0f;
        private const float RotationSpeed = 1.15f;
        private const float Scale = 150.0f;

        public override void OnCreate()
        {
            var sprite = AddComponent<SpriteComponent>(Colour);
            sprite.Texture = Resources.Textures.EnemyTankBody;
            Vector2 tankTextureSize = new Vector2(sprite.Texture.Width, sprite.Texture.Height);
            tankTextureSize.Normalize();
            Transform.Scale = tankTextureSize * Scale;

            Rigidbody2D rigidbody = AddComponent<Rigidbody2D>();
            rigidbody.Scale = new Vector2(1.0f, 0.734f);
            rigidbody.Mass = 60_000; // Mass of a tank is around 60 tons

            _Transform = Transform;
        }
    }
}
