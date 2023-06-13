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
    internal class BlockerScript : ScriptablePhysicsGameObject
    {
        private Colour Colour = new Colour(60, 46, 33);
        private const float Scale = 150.0f;

        public override void OnCreate()
        {
            var sprite = AddComponent<SpriteComponent>(Colour);
            sprite.Texture = Resources.Textures.Blocker;

            Vector2 textureSize = new Vector2(sprite.Texture.Width, sprite.Texture.Height);
            textureSize.Normalize();
            Transform.Scale = textureSize * Scale;

            var rigidbody = AddComponent<Rigidbody2D>();
            rigidbody.Mass = 100_000_000;
        }
    }
}
