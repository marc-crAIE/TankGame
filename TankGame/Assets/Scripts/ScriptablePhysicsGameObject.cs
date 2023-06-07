using RayEngine.GameObjects;

namespace TankGame.Assets.Scripts
{
    internal class ScriptablePhysicsGameObject : ScriptableGameObject
    {
        public virtual void OnCollision2D(GameObject other) { }
    }
}
