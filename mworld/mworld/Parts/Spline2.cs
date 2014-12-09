using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace mworld.Parts
{
    public class Spline2 : Part
    {
        //
        // Members
        //

        PartPath path;
        int num_points;


        //
        // 'Tors
        //
        public Spline2()
        {
            _name = "quadratic spline";

            num_points = 15;
            draw_intermediate_nodes = false;

            // Make a basic 2-point path
            path = new PartPath(this);
            path.add_vert(new PartPathVertex(path, -16, 0));
            path.add_vert(new PartPathVertex(path, 16, 0));
            paths.Add(path);

            //
            _can_edit_endpoints = true;
            _can_rotate = false;
        }

        //
        // Methods
        //

        //
        // Virtual methods
        //

        public override Part new_copy(Part part = null)
        {
            if (part == null) part = new Spline2();
            return base.new_copy(part);
        }

        public override void update(float dt)
        {
            // Recalculate radius
            r = 0;
            r = Math.Max(r, path.first_vert().pos.Length() + 8);
            r = Math.Max(r, path.last_vert().pos.Length() + 8);

            // Recalculate the spline path
            // TODO: Figure out a way to only do this when the path points have changed
            Vector2 a = path.first_vert().pos;
            Vector2 b = path.last_vert().pos;
            if (path.num_verts() == num_points + 1)
            {
                for (int i = 0; i <= num_points; i++)
                    path.get_vert(i).pos = Tween.quadratic_bezier(a, b, Vector2.Zero, (float)i / (float)num_points);
            }
            else
            {
                path.clear_verts();
                for (int i = 0; i <= num_points; i++)
                    path.add_vert(new PartPathVertex(path, Tween.quadratic_bezier(a, b, Vector2.Zero, (float)i / (float)num_points)));
            }

            //
            base.update(dt);
        }

        public override void draw(Color color)
        {

            //
            if (MWorld.mode == MWorld.GameMode.Play)
            {
                draw_paths(color, 20);
            }
            else if (MWorld.mode == MWorld.GameMode.Edit)
            {
                Vector3 a = Vector3.Transform(path.first_vert().pos3(), transformation);
                Vector3 b = Vector3.Transform(path.last_vert().pos3(), transformation);
                Vector3 p = Vector3.Transform(Vector3.Zero, transformation);

                Graphics.draw_tex("node", pos, MWorld.hud_rotation * 5, .5f / MWorld.view.get_scale(), Color.DeepSkyBlue, 1f);

                if (is_selected())
                {
                    Graphics.draw_line(a, b, Color.DeepSkyBlue);
                    Graphics.draw_line(a, p, Color.DeepSkyBlue);
                    Graphics.draw_line(b, p, Color.DeepSkyBlue);
                }
            }


            //
            base.draw(color);
        }
    }
}
