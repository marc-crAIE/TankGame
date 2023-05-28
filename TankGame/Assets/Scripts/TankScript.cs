using RayEngine.Core;
using RayEngine.GameObjects;
using RayEngine.GameObjects.Components;
using SharpMaths;

namespace TankGame.Assets.Scripts
{
    internal class TankScript : ScriptableGameObject
    {
        private TransformComponent _Transform;

        private const float MovementSpeed = 75.0f;
        private const float RotationSpeed = 1.5f;
        private const float Scale = 150.0f;

        public override void OnCreate()
        {
            Window window = Application.Instance.GetWindow();
            Transform.Translation = new Vector2(window.GetWidth() / 2f, window.GetHeight() / 2f);

            var sprite = AddComponent<SpriteComponent>(new Colour(176, 223, 142));
            sprite.Texture = Resources.Textures.TankBody;
            Vector2 tankTextSize = new Vector2(sprite.Texture.Width, sprite.Texture.Height);
            tankTextSize.Normalize();
            Transform.Scale = tankTextSize * Scale;

            _Transform = Transform;
        }

        public override void OnUpdate(Timestep ts)
        {
            // Forwards and backwards
            if (Input.IsKeyPressed(Key.KEY_W))
            {
                float rotation = _Transform.Rotation.z;
                Vector2 moveDirection = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                moveDirection.Normalize();
                _Transform.Translation += moveDirection * (MovementSpeed * ts);
            }
            if (Input.IsKeyPressed(Key.KEY_S))
            {
                float rotation = _Transform.Rotation.z;
                Vector2 moveDirection = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                moveDirection.Normalize();
                _Transform.Translation -= moveDirection * (MovementSpeed * ts);
            }

            // Rotation
            if (Input.IsKeyPressed(Key.KEY_A))
                _Transform.Rotation.z -= RotationSpeed * ts;
            if (Input.IsKeyPressed(Key.KEY_D))
                _Transform.Rotation.z += RotationSpeed * ts;
        }
    }
}
