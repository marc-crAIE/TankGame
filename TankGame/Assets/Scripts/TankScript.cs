using RayEngine.Core;
using RayEngine.GameObjects;
using RayEngine.GameObjects.Components;
using SharpMaths;
using TankGame.Components;

namespace TankGame.Assets.Scripts
{
    internal class TankScript : ScriptableGameObject
    {
        private TransformComponent _Transform;
        private Rigidbody2D _Rigidbody;

        private Colour Colour = new Colour(176, 223, 142);

        private const float MovementSpeed = 75.0f;
        private const float RotationSpeed = 1.15f;
        private const float Scale = 150.0f;

        // Used for calculation with rigidbody velocity
        private const float TankLength = 7.5f; // In meters
        private float PixelsPerMeter = 1.0f;
        private float TankSpeedMeters = 1.0f;

        public override void OnCreate()
        {
            Window window = Application.Instance.GetWindow();
            Transform.Translation = new Vector2(window.GetWidth() / 2f, window.GetHeight() / 2f);

            var sprite = AddComponent<SpriteComponent>(Colour);
            sprite.Texture = Resources.Textures.TankBody;
            Vector2 tankTextureSize = new Vector2(sprite.Texture.Width, sprite.Texture.Height);
            tankTextureSize.Normalize();
            Transform.Scale = tankTextureSize * Scale;

            PixelsPerMeter = Transform.Scale.x / TankLength;
            TankSpeedMeters = MovementSpeed / PixelsPerMeter;

            _Rigidbody = AddComponent<Rigidbody2D>();
            _Rigidbody.Scale = new Vector2(1.0f, 0.734f);
            _Rigidbody.Mass = 60_000; // Mass of a tank is around 60 tons

            _Transform = Transform;
        }

        public override void OnUpdate(Timestep ts)
        {
            Vector2 moveDirection = new Vector2(0.0f);
            float angularVelocity = 0.0f;

            // Forwards and backwards
            if (Input.IsKeyPressed(Key.KEY_W))
            {
                float rotation = _Transform.Rotation.z;
                moveDirection = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                moveDirection.Normalize();
                _Transform.Translation += moveDirection * (MovementSpeed * ts);
            }
            if (Input.IsKeyPressed(Key.KEY_S))
            {
                float rotation = _Transform.Rotation.z;
                moveDirection = -(new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)));
                moveDirection.Normalize();
                _Transform.Translation += moveDirection * (MovementSpeed * ts);
            }
            _Rigidbody.Velocity = moveDirection * TankSpeedMeters;

            // Rotation
            if (Input.IsKeyPressed(Key.KEY_A))
            {
                _Transform.Rotation.z -= RotationSpeed * ts;
                angularVelocity = -RotationSpeed;
            }
            if (Input.IsKeyPressed(Key.KEY_D))
            {
                _Transform.Rotation.z += RotationSpeed * ts;
                angularVelocity = RotationSpeed;
            }
            _Rigidbody.AngularVelocity = angularVelocity;
        }
    }
}
