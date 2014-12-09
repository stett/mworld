using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace mworld.Parts
{
    public class Generator : Part
    {
        //
        // Members
        //

        PartPath path;
        float t_trans;
        float t_trans_targ;
        float v_trans;
        float graphic_angle;
        public List<Marble> marbles;
        int marbles_spent;


        //
        // 'Tors
        //

        public Generator()
        {
            _name = "generator";

            //
            t_trans = 0f;
            t_trans_targ = 0f;
            v_trans = .1f;
            graphic_angle = 0f;
            marbles = new List<Marble>();
            marbles_spent = 0;

            //
            float r = 50f;

            // Set up path nodes
            path = new PartPath(this);
            path.add_vert(new PartPathVertex(path, 0, 0));
            path.add_vert(new PartPathVertex(path, 0, r));
            paths.Add(path);
        }


        //
        // Methods
        //

        public void generate()
        {
            if (marbles_spent <= marbles.Count - 1)
            {
                Marble marble = marbles[marbles_spent];
                marble.enpath(path.first_vert(), 0);
                MWorld.state.marbles.Add(marble);
                marbles_spent++;
            }
        }

        public override Part new_copy(Part part = null)
        {
            if (part == null) part = new Generator();
            foreach (Marble marble in marbles)
                ((Generator)part).marbles.Add(marble.new_copy());
            return base.new_copy(part);
        }

        public override void update(float dt)
        {
            //
            r = Graphics.get_tex("generator").Width * .5f;

            //
            if (MWorld.mode == MWorld.GameMode.Edit)
            {
                marbles_spent = 0;
            }

            //
            if (MWorld.mode == MWorld.GameMode.Play && (MWorld.mouse.get_world_pos() - pos).Length() < r)
            {
                graphic_angle += .035f;

                if (MWorld.mouse.left_down()) t_trans_targ = 0f;
                else t_trans_targ = 1f;

                if (MWorld.mouse.left_clicked()) generate();
            }
            else
            {
                graphic_angle += .02f;
                t_trans_targ = .5f;
            }

            //
            if (Math.Abs(t_trans - t_trans_targ) <= v_trans) t_trans = t_trans_targ;
            else if (t_trans < t_trans_targ) t_trans += v_trans;
            else t_trans -= v_trans;

            //
            base.update(dt);
        }

        public override void draw(Color color)
        {
            //
            float scale = Tween.smoothstep(.9f, 1.1f, t_trans);

            //
            for (int i = 0; i < marbles.Count; i++)
            {
                float tab_scale = .71f;
                if (i >= marbles_spent) tab_scale = .95f;

                Graphics.draw_tex("tab", pos, angle + i * 45 * (float)Math.PI / 180f, scale * tab_scale, marbles[i].color, 1f, .6f);
                //Graphics.draw_tex("tab-frame", pos, angle + i * 45 * (float)Math.PI / 180f, scale * tab_scale, color, 1f, .61f);
            }

            //
            Graphics.draw_tex("generator", pos, graphic_angle, scale, color, 1f, .4f);

            //
            base.draw(color);
        }

        public override void receive()
        {
            generate();

            //
            base.receive();
        }
    }
}
