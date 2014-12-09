using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace mworld
{
    public static class Tween
    {
        public static float linear(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static float smoothstep(float t)
        {
            return t * t * (3.0f - 2.0f * t);
        }

        public static float smoothstep(float a, float b, float t)
        {
            return a + (b - a) * smoothstep(t);
        }

        public static Vector2 quadratic_bezier(Vector2 a, Vector2 b, Vector2 p, float t)
        {
            return (1 - t) * (1 - t) * a +
                   2 * (1 - t) * t * p +
                   t * t * b;
        }
    }
}
