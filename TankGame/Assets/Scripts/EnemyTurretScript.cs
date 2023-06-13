using RayEngine.GameObjects;
using RayEngine.GameObjects.Components;
using SharpMaths;

namespace TankGame.Assets.Scripts
{
    internal class EnemyTurretScript : ScriptableGameObject
    {
        private TransformComponent _Transform;
        private TransformComponent PivotTransform;
        private TransformComponent BodyTransform;

        private Colour Colour = new Colour(255, 255, 255);

        private const float RotationSpeed = 1.5f;
        private const float Scale = 1.0f;

        public override void OnCreate()
        {
            var sprite = AddComponent<SpriteComponent>(Colour);
            sprite.Texture = Resources.Textures.EnemyTankTurret;

            Vector2 tankTextSize = new Vector2(sprite.Texture.Width, sprite.Texture.Height);
            tankTextSize.Normalize();

            // These values were grabbed when using the debugger editing tools
            Transform.Translation = new Vector2(0.2f, 0.01f);
            Transform.Scale = tankTextSize * Scale;

            _Transform = Transform;
            GameObject? tankBody = Self.Parent?.Parent;
            PivotTransform = Self.Parent?.GetComponent<TransformComponent>();
            if (tankBody is not null)
                BodyTransform = tankBody.GetComponent<TransformComponent>();
        }
    }
}
