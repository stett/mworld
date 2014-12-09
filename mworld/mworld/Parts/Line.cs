using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace mworld.Parts
{
    public class Line : Part
    {
        //
        // Members
        //

        PartPath path;
        protected Vector2 a, b;


        //
        // 'Tors
        //

        public Line()
        {
            _name = "line";

            draw_intermediate_nodes = false;

            //
            path = new PartPath(this);
            path.add_vert(new PartPathVertex(path, -16, 0));
            path.add_vert(new PartPathVertex(path, 16, 0));
            paths.Add(path);

            //
            a = path.first_vert().pos;
            b = path.last_vert().pos;

            //
            _can_edit_endpoints = true;
            _can_rotate = false;
        }


        //
        // Methods
        //

        public override Part new_copy(Part part = null)
        {
            if (part == null) part = new Line();
            return base.new_copy(part);
        }

        public override void update(float dt)
        {
            // Recalculate radius
            r = 0;
            r = Math.Max(r, path.first_vert().pos.Length() + 8);
            r = Math.Max(r, path.last_vert().pos.Length() + 8);

            if (a != path.first_vert().pos || b != path.last_vert().pos)
            {
                a = path.first_vert().pos;
                b = path.last_vert().pos;

                Vector2 wa = path.first_vert().w_pos();
                Vector2 wb = path.last_vert().w_pos();

                Vector2 new_pos = (wa + wb) * .5f;
                Vector2 pos_diff = new_pos - pos;

                a -= pos_diff;
                b -= pos_diff;

                path.first_vert().pos = a;
                path.last_vert().pos = b;

                pos += pos_diff;
            }

            //
            base.update(dt);
        }

        public override void draw(Color color)
        {
            if (MWorld.mode == MWorld.GameMode.Play)
                Graphics.draw_line(path.first_vert().w_pos(), path.last_vert().w_pos(), color, 20, true, .6f);

            //
            base.draw(color);
        }
    }
}
