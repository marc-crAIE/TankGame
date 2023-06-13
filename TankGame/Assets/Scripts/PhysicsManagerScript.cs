using RayEngine.Core;
using RayEngine.Debug;
using RayEngine.GameObjects;
using RayEngine.GameObjects.Components;
using RayEngine.Utils;
using Raylib_cs;
using SharpECS;
using SharpMaths;
using TankGame.Components;

using Rectangle = SharpMaths.Rectangle;

namespace TankGame.Assets.Scripts
{
    internal class PhysicsManagerScript : ScriptableGameObject
    {
        public bool ExperimentalPhysics = true;
        public bool RenderPhysicsBoxes = false;
        private Timestep DeltaTime = 0;

        public override void OnUpdate(Timestep ts)
        {
            using var _it = Profiler.Function();

            DeltaTime = ts;

            EntityRegistry registry = Scene.GetRegistry();
            EntityQuery physicsEntities = registry.GetEntities().With<Rigidbody2D>();

            BroadCollision(physicsEntities.AsSet());

            if (Input.IsKeyTyped(Key.KEY_F2))
                RenderPhysicsBoxes = !RenderPhysicsBoxes;
        }

        private void BroadCollision(EntitySet entities)
        {
            using var _it = Profiler.Function();

            if (entities.Count == 0)
                return;

            EntityRegistry registry = Scene.GetRegistry();

            for (int i = 0; i < entities.Count; i++)
            {
                Entity entityA = entities[i];
                Rectangle rectA = GetBoundingRectangle(entityA, out Polygon polyA);
                bool collision = false;
                for (int j = 0; j < entities.Count; j++)
                {
                    if (i == j)
                        continue;

                    Entity entityB = entities[j];
                    if (registry.Get<Rigidbody2D>(entityB).Dirty)
                        continue;

                    Rectangle rectB = GetBoundingRectangle(entityB, out Polygon polyB);
                    collision = AABB(rectA, rectB);

                    if (RenderPhysicsBoxes)
                    {
                        Raylib.DrawRectangleLines((int)rectA.x, (int)rectA.y, (int)rectA.width, (int)rectA.height, collision ? Color.RED : Color.GREEN);
                        Raylib.DrawRectangleLines((int)rectB.x, (int)rectB.y, (int)rectB.width, (int)rectB.height, collision ? Color.RED : Color.GREEN);
                    }

                    float overlap = float.MaxValue;
                    if (collision)
                        collision = NarrowCollision(ref polyA, ref polyB, out overlap);

                    // Still colliding after narrow collision
                    if (collision)
                    {
                        TransformComponent transformA = registry.Get<TransformComponent>(entityA);
                        TransformComponent transformB = registry.Get<TransformComponent>(entityB);

                        // Resolve collision
                        if (ExperimentalPhysics)
                        {
                            Rigidbody2D rigidBodyA = registry.Get<Rigidbody2D>(entityA);
                            Rigidbody2D rigidBodyB = registry.Get<Rigidbody2D>(entityB);

                            Vector2 collisionNormal = polyB.Position - polyA.Position;
                            Vector2 relativeVelocity = rigidBodyB.Velocity - rigidBodyA.Velocity;

                            Vector2 sizeA = transformA.Scale * rigidBodyA.Scale;
                            Vector2 sizeB = transformB.Scale * rigidBodyB.Scale;
                            float radiusA = 0.5f * sizeA.Magnitude();
                            float radiusB = 0.5f * sizeB.Magnitude();

                            float relativeVelocityAlongNormal = relativeVelocity.Dot(collisionNormal)
                                        + ((radiusA * rigidBodyA.AngularVelocity) - (radiusB * rigidBodyB.AngularVelocity));

                            if (relativeVelocityAlongNormal > 0)
                                relativeVelocityAlongNormal = -relativeVelocityAlongNormal;

                            float restitution = 0.2f; // Adjust as needed
                            float impulse = ((1 + restitution) * relativeVelocityAlongNormal) / (rigidBodyA.Mass * rigidBodyB.Mass);

                            polyA.Position += impulse * collisionNormal * rigidBodyB.Mass;
                            polyB.Position -= impulse * collisionNormal * rigidBodyA.Mass;
                        }
                        else
                        {
                            // Very dirty collision resolution
                            Vector2 d = polyB.Position - polyA.Position;
                            float s = d.Magnitude();
                            if (s != 0)
                            {
                                polyA.Position.x -= overlap * d.x / s;
                                polyA.Position.y -= overlap * d.y / s;
                            }
                        }

                        transformA.Translation = polyA.Position;
                        transformB.Translation = polyB.Position;

                        // Try to run the OnCollision2D function for both
                        ScriptablePhysicsGameObject? scriptA = null;
                        ScriptablePhysicsGameObject? scriptB = null;

                        if (registry.Has<ScriptComponent>(entityA))
                            scriptA = registry.Get<ScriptComponent>(entityA).Instance as ScriptablePhysicsGameObject;
                        if (registry.Has<ScriptComponent>(entityB))
                            scriptB = registry.Get<ScriptComponent>(entityB).Instance as ScriptablePhysicsGameObject;

                        scriptA?.OnCollision2D(Scene?.GetWithUUID(registry.Get<UUID>(entityB)));
                        scriptB?.OnCollision2D(Scene?.GetWithUUID(registry.Get<UUID>(entityA)));
                    }
                }

                registry.Get<Rigidbody2D>(entityA).Dirty = true;
            }

            for (int i = 0; i < entities.Count; i++)
                registry.Get<Rigidbody2D>(entities[i]).Dirty = false;
        }

        private bool NarrowCollision(ref Polygon poly1, ref Polygon poly2, out float overlap)
        {
            using var _it = Profiler.Function();

            overlap = float.MaxValue;

            Polygon p1 = poly1;
            Polygon p2 = poly2;

            // Check edge normals for both shapes
            for (int shape = 0; shape < 2; shape++)
            {
                if (shape == 1)
                {
                    // Do the other shape
                    Polygon temp = p1;
                    p1 = p2;
                    p2 = temp;
                }

                for (int a = 0; a < p1.Points.Length; a++)
                {
                    int b = (a + 1) % p1.Points.Length;
                    Vector2 axisProj = new Vector2(-(p1.Points[b].y - p1.Points[a].y), p1.Points[b].x - p1.Points[a].x);
                    axisProj.Normalize();

                    // Work out min and max 1D points for poly1
                    float min_r1 = float.MaxValue, max_r1 = -float.MaxValue;
                    for (int p = 0; p < p1.Points.Length; p++)
                    {
                        float q = (p1.Points[p].x * axisProj.x + p1.Points[p].y * axisProj.y);
                        min_r1 = MathF.Min(min_r1, q);
                        max_r1 = MathF.Max(max_r1, q);
                    }

                    // Work out min and max 1D points for poly2
                    float min_r2 = float.MaxValue, max_r2 = -float.MaxValue;
                    for (int p = 0; p < p2.Points.Length; p++)
                    {
                        float q = (p2.Points[p].x * axisProj.x + p2.Points[p].y * axisProj.y);
                        min_r2 = MathF.Min(min_r2, q);
                        max_r2 = MathF.Max(max_r2, q);
                    }

                    // Calculate the actualy overlap along the projected axis
                    overlap = MathF.Min(MathF.Min(max_r1, max_r2) - MathF.Max(min_r1, min_r2), overlap);

                    // Check if they are not colliding
                    if (!(max_r2 >= min_r1 && max_r1 >= min_r2))
                    {
                        if (RenderPhysicsBoxes)
                        {
                            Rectangle rect1 = new Rectangle(p1.Position, p1.Vertices[0] * 2);
                            Rectangle rect2 = new Rectangle(p2.Position, p2.Vertices[0] * 2);
                            Raylib.DrawRectanglePro(rect1.ToRLRectangle(), rect1.Size / 2, SharpMath.ToDegrees(p1.Rotation), Color.GREEN);
                            Raylib.DrawRectanglePro(rect2.ToRLRectangle(), rect2.Size / 2, SharpMath.ToDegrees(p2.Rotation), Color.GREEN);
                        }
                        return false;
                    }
                }
            }

            if (RenderPhysicsBoxes)
            {
                Rectangle rect1 = new Rectangle(p1.Position, p1.Vertices[0] * 2);
                Rectangle rect2 = new Rectangle(p2.Position, p2.Vertices[0] * 2);
                Raylib.DrawRectanglePro(rect1.ToRLRectangle(), rect1.Size / 2, SharpMath.ToDegrees(p1.Rotation), Color.RED);
                Raylib.DrawRectanglePro(rect2.ToRLRectangle(), rect2.Size / 2, SharpMath.ToDegrees(p2.Rotation), Color.RED);
            }

            return true;
        }

        private bool AABB(Rectangle rectA, Rectangle rectB)
        {
            bool x = rectA.x + rectA.width >= rectB.x && rectB.x + rectB.width >= rectA.x;
            bool y = rectA.y + rectA.height >= rectB.y && rectB.y + rectB.height >= rectA.y;
            return x && y;
        }

        private Rectangle GetBoundingRectangle(Entity entity, out Polygon polygon)
        {
            EntityRegistry registry = Scene.GetRegistry();

            TransformComponent transform = registry.Get<TransformComponent>(entity);
            Rigidbody2D rigidbody = registry.Get<Rigidbody2D>(entity);

            Vector2 position = transform.Translation + rigidbody.Position;
            float rotation = transform.Rotation.z + rigidbody.Rotation;
            Vector2 scale = transform.Scale * rigidbody.Scale;

            Vector2 halfSize = scale / 2f;

            float cosAngle = MathF.Cos(rotation);
            float sinAngle = MathF.Sin(rotation);

            Vector2 corner1 = position + new Vector2(halfSize.x * cosAngle - halfSize.y * sinAngle, halfSize.x * sinAngle + halfSize.y * cosAngle);
            Vector2 corner2 = position + new Vector2(-halfSize.x * cosAngle - halfSize.y * sinAngle, -halfSize.x * sinAngle + halfSize.y * cosAngle);
            Vector2 corner3 = position + new Vector2(-halfSize.x * cosAngle + halfSize.y * sinAngle, -halfSize.x * sinAngle - halfSize.y * cosAngle);
            Vector2 corner4 = position + new Vector2(halfSize.x * cosAngle + halfSize.y * sinAngle, halfSize.x * sinAngle - halfSize.y * cosAngle);

            Vector2 min = new Vector2(
                MathF.Min(corner1.x, MathF.Min(corner2.x, MathF.Min(corner3.x, corner4.x))),
                MathF.Min(corner1.y, MathF.Min(corner2.y, MathF.Min(corner3.y, corner4.y)))
                );
            Vector2 max = new Vector2(
                MathF.Max(corner1.x, MathF.Max(corner2.x, MathF.Max(corner3.x, corner4.x))),
                MathF.Max(corner1.y, MathF.Max(corner2.y, MathF.Max(corner3.y, corner4.y)))
                );

            polygon = new Polygon();
            polygon.Position = position;
            polygon.Rotation = rotation;
            polygon.Vertices = new Vector2[4] {
                new Vector2(halfSize.x, halfSize.y),
                new Vector2(-halfSize.x, halfSize.y),
                new Vector2(-halfSize.x, -halfSize.y),
                new Vector2(halfSize.x, -halfSize.y) 
            };
            polygon.Points = new Vector2[4] { corner1, corner2, corner3, corner4 } ;

            return new Rectangle((int)min.x, (int)min.y, (int)(max.x - min.x), (int)(max.y - min.y));
        }
    }
}
