using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace mworld.Parts
{
    public class Trigger : Part
    {
        //
        // Members
        //
        PartPath path;
        float t_light;

        //
        // 'Tors
        //

        public Trigger()
        {
            _name = "trigger";

            t_light = 0f;

            r = 56f;

            path = new PartPath(this);
            path.add_vert(new PartPathVertex(path, 0, -r + 12));
            path.add_vert(new PartPathVertex(path, 0,  r - 12));
            paths.Add(path);
        }

        //
        // Methods
        //

        public override Part new_copy(Part part = null)
        {
            if (part == null) part = new Trigger();
            return base.new_copy(part);
        }

        public override void update(float dt)
        {
            //
            if (t_light > 0f) t_light -= .1f;
            if (t_light < 0f) t_light = 0f;

            //
            base.update(dt);
        }

        public override void send()
        {
            //
            t_light = 1f;

            //
            base.send();
        }

        public override void update_path_pos(ref PartPathVertex vert, ref float dist, ref float vel, float vel_step, out Vector2 path_pos, out Vector2 path_tan)
        {
            float d0 = dist;
            base.update_path_pos(ref vert, ref dist, ref vel, vel_step, out path_pos, out path_tan);
            float d1 = dist;

            if (vert == path.first_vert())
            {
                float mid = vert.get_length() * .5f;
                if (d0 < mid && d1 > mid || d0 > mid && d1 < mid)
                    send();
            }
        }

        public override void draw(Color color)
        {
            //
            float light_alpha = Tween.smoothstep(0f, 1f, t_light);
            Graphics.draw_tex("trigger", pos, angle, 1f, color, 1f, .4f);
            Graphics.draw_tex("trigger-light", pos, angle, 1f, Color.White, light_alpha, .4f);

            //
            base.draw(color);
        }
    }
}
