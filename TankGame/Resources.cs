using RayEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
    internal static class Resources
    {
        public static class Textures
        {
            public static Texture2D TankBody { get; internal set; }
            public static Texture2D TankTurret { get; internal set; }

            public static Texture2D EnemyTankBody { get; internal set; }
            public static Texture2D EnemyTankTurret { get; internal set; }

            public static Texture2D Blocker { get; internal set; }
        }

        public static void Init()
        {
            Textures.TankBody = new Texture2D("Assets/Textures/tank body.png");
            Textures.TankTurret = new Texture2D("Assets/Textures/tank turret.png");

            Textures.EnemyTankBody = new Texture2D("Assets/Textures/tank body enemy.png");
            Textures.EnemyTankTurret = new Texture2D("Assets/Textures/tank turret enemy.png");

            Textures.Blocker = new Texture2D("Assets/Textures/blocker.png");
        }

        public static void Dispose()
        {
            Textures.TankBody.Dispose();
            Textures.TankTurret.Dispose();
            Textures.EnemyTankBody.Dispose();
            Textures.EnemyTankTurret.Dispose();
            Textures.Blocker.Dispose();
        }
    }
}
