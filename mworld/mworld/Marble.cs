using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace mworld
{
    public class Marble
    {
        //
        // Members
        //

        public Vector2 pos;
        public Vector2 vel;
        PartPathVertex vert;
        float vert_dist;
        float path_vel;
        public Color color;


        //
        // 'Tors
        //

        public Marble()
        {
            pos = Vector2.Zero;
            vert = null;
            vert_dist = 0f;
            color = Color.DeepSkyBlue;
        }
        public Marble(Vector2 _pos)
        {
            pos = _pos;
            vert = null;
            vert_dist = 0f;
            color = Color.DeepSkyBlue;
        }


        //
        // Methods
        //

        public void enpath(PartPathVertex _vert, float _vert_dist)
        {
            vert = _vert;
            vert_dist = _vert_dist;
            Vector2 path_tan;
            path_vel = 0;
            vert.get_path().get_part().update_path_pos(ref vert, ref vert_dist, ref path_vel, out pos, out path_tan);
        }

        public void depath(bool remove_from_list = true)
        {
            vert = null;
            vert_dist = 0;
            if (remove_from_list) MWorld.state.marbles.Remove(this);
        }

        public virtual Marble new_copy(Marble marble = null)
        {
            if (marble == null) marble = new Marble();
            marble.color = color;
            return marble;
        }

        public virtual void update(float dt)
        {
            // Set up gravitational acceleration
            Vector2 acc = -pos;
            acc.Normalize();

            // If we are attached to a path
            if (vert != null)
            {
                Vector2 path_tan;
                vert.get_path().get_part().update_path_pos(ref vert, ref vert_dist, ref path_vel, out pos, out path_tan);
                path_vel += Vector2.Dot(acc, path_tan);
                vel = path_tan * path_vel;
            }
            else
            {
                pos += vel;
                vel += acc;
            }
        }

        public virtual void draw()
        {
            Graphics.draw_tex("marble", pos, 0f, 1f, color, 1f, .5f);
            //Graphics.draw_line(pos, pos + vel, Color.Orchid, 2);
        }

        public bool is_pathed() { return vert != null; }
    }
}
