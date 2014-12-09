using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace mworld.Parts
{
    public class Merger : Part
    {
        //
        // Members
        //

        PartPath path_left;
        PartPath path_right;
        PartPath path_exit;

        //
        // 'Tors
        //

        public Merger()
        {
            _name = "merger";

            // Radius
            r = 50;

            // Set up path nodes
            path_left = new PartPath(this);
            path_left.add_vert(new PartPathVertex(path_left,
                (float)Math.Cos((270 + 45) * Math.PI / 180) * r,
                (float)Math.Sin((270 + 45) * Math.PI / 180) * r));
            path_left.add_vert(new PartPathVertex(path_left, 0, 0));
            paths.Add(path_left);

            path_right = new PartPath(this);
            path_right.add_vert(new PartPathVertex(path_right,
                (float)Math.Cos((270 - 45) * Math.PI / 180) * r,
                (float)Math.Sin((270 - 45) * Math.PI / 180) * r));
            path_right.add_vert(new PartPathVertex(path_right, 0, 0));
            paths.Add(path_right);

            path_exit = new PartPath(this);
            path_exit.add_vert(new PartPathVertex(path_exit, 0, 0));
            path_exit.add_vert(new PartPathVertex(path_exit, 0, r));
            paths.Add(path_exit);

            // Connect verts
            PartPath.connect_verts(path_left.last_vert(), path_exit.first_vert());
            path_right.last_vert().connection = path_exit.first_vert();
        }


        //
        // Methods
        //

        public override Part new_copy(Part part = null)
        {
            if (part == null) part = new Merger();
            return base.new_copy(part);
        }

        public override void update(float dt)
        {
            base.update(dt);
        }

        public override void draw(Color color)
        {
            if (MWorld.mode == MWorld.GameMode.Play)
                draw_paths(color, 20);
            base.draw(color);
        }
    }
}
