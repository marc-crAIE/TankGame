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
        }

        public static void Init()
        {
            Textures.TankBody = new Texture2D("Assets/Textures/tank body.png");
            Textures.TankTurret = new Texture2D("Assets/Textures/tank turret.png");
        }
    }
}
