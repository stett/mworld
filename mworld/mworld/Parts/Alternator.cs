using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace mworld.Parts
{
    public class Alternator : Part
    {
        //
        // Members
        //

        public enum Mode { Left, Right };
        Mode mode;
        float t_alternate;
        float v_alternate;

        PartPath path_entrance;
        PartPath path_left;
        PartPath path_right;


        //
        // 'Tors
        //

        public Alternator()
        {
            _name = "alternator";

            //
            t_alternate = 0;
            v_alternate = .05f;

            // Set up path nodes
            float r = 50;
            path_entrance = new PartPath(this);
            path_entrance.add_vert(new PartPathVertex(path_entrance, 0, -r));
            path_entrance.add_vert(new PartPathVertex(path_entrance, 0, 0));
            paths.Add(path_entrance);

            path_left = new PartPath(this);
            path_left.add_vert(new PartPathVertex(path_left, 0, 0));
            path_left.add_vert(new PartPathVertex(path_left, 
                (float)Math.Cos((90 + 45) * Math.PI / 180) * r,
                (float)Math.Sin((90 + 45) * Math.PI / 180) * r));
            paths.Add(path_left);

            path_right = new PartPath(this);
            path_right.add_vert(new PartPathVertex(path_right, 0, 0));
            path_right.add_vert(new PartPathVertex(path_right, 
                (float)Math.Cos((90 - 45) * Math.PI / 180) * r,
                (float)Math.Sin((90 - 45) * Math.PI / 180) * r));
            paths.Add(path_right);

            // Set up path connections
            alternate(Mode.Left);
        }


        //
        // Methods
        //

        public void alternate(Mode? new_mode = null)
        {

            // Alternate
            if (new_mode != null) mode = (Mode)new_mode;
            else
            {
                if (mode == Mode.Left) mode = Mode.Right;
                else mode = Mode.Left;
            }

            // Change path connections
            if (mode == Mode.Left) PartPath.connect_verts(path_entrance.last_vert(), path_left.first_vert());
            else PartPath.connect_verts(path_entrance.last_vert(), path_right.first_vert());
        }

        public override Part new_copy(Part part = null)
        {
            if (part == null) part = new Alternator();
            return base.new_copy(part);
        }

        public override void update(float dt)
        {
            //
            if (mode == Mode.Left)
            {
                if (t_alternate > 0) t_alternate -= v_alternate; // * dt;
                else t_alternate = 0;
            }
            else
            {
                if (t_alternate < 1) t_alternate += v_alternate; // * dt;
                else t_alternate = 1;
            }

            //
            r = Graphics.get_tex("alternator-outer").Width * .5f;

            //
            base.update(dt);
        }

        public override void draw(Color color)
        {
            float inner_angle = angle + Tween.smoothstep(45.0f, -45.0f, t_alternate) * (float)Math.PI / 180.0f;
            Graphics.draw_tex("alternator-inner", new Vector2(pos.X, pos.Y), inner_angle, 1, color, 1f, .4f);
            Graphics.draw_tex("alternator-outer", new Vector2(pos.X, pos.Y), angle, 1, color, .1f, .4f);

            //
            base.draw(color);
        }

        public override void receive()
        {
            alternate();

            //
            base.receive();
        }
    }
}
