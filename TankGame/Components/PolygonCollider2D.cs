using SharpMaths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Components
{
    internal struct Polygon
    {
        public Vector2[] Points; // Transformed points
        public Vector2 Position;
        public float Rotation;
        public Vector2[] Vertices; // Polygon model
    }

    internal struct PolygonCollider2D
    {
        
    }
}
