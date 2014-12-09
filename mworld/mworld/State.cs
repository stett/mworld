using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace mworld
{
    /// <summary>
    /// Contains all information for the game state.
    /// A level is simply a derived State containing specific
    /// initializers and extra functions.
    /// </summary>
    public class State
    {
        //
        // Members
        //

        public List<Part> parts;
        public List<Marble> marbles;

        //
        // 'Tors
        //

        public State()
        {
            parts = new List<Part>();
            marbles = new List<Marble>();
        }

        //
        // Methods
        //

        public Vector2 closest_vert(Vector2 a, float threshhold, PartPathVertex ignore)
        {
            PartPathVertex close_vert;
            return closest_vert(a, threshhold, ignore, out close_vert);
        }
        public Vector2 closest_vert(Vector2 a, float threshhold, PartPathVertex ignore, out PartPathVertex close_vert)
        {
            close_vert = null;
            Vector2 closest = a;
            float diff_min = -1;
            foreach (Part part in parts)
            {
                Matrix transformation = part.get_trans();
                foreach (PartPath path in part.paths)
                {
                    for (int i = 0; i < path.num_verts(); i += path.num_verts() - 1)
                    {
                        PartPathVertex vert = path.get_vert(i);
                        if (vert == ignore) continue;
                        if (vert.connection != null) continue;
                        if ((ignore.is_first() && vert.is_first()) || (ignore.is_last() && vert.is_last())) continue;
                        Vector3 b3 = Vector3.Transform(vert.pos3(), part.get_trans());
                        Vector2 b = new Vector2(b3.X, b3.Y);
                        float diff = (b - a).Length();
                        if ((diff_min == -1 || diff < diff_min) && diff < threshhold)
                        {
                            diff_min = diff;
                            closest = b;
                            close_vert = vert;
                        }
                    }
                }
            }
            return closest;
        }
    }
}
