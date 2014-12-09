using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace mworld
{
    public class Grid
    {
        //
        // Members
        //

        public float r_inc;
        public int r_marks;
        public int a_sections;
        public bool on;
        private float t_trans;
        private float v_trans;


        //
        // 'Tors
        //

        public Grid()
        {
            r_inc = 64;
            r_marks = 110;
            a_sections = 60;
            on = true;
            t_trans = 0;
            v_trans = 0.05f;
        }


        //
        // Methods
        //

        public void update(float dt)
        {
            if (on)
            {
                if (t_trans < 1) t_trans += v_trans; // * dt;
                else t_trans = 1;
            }
            else
            {
                if (t_trans > 0) t_trans -= v_trans; // * dt;
                else t_trans = 0;
            }
        }

        public void draw(Color color)
        {
            if (t_trans == 0) return;

            float alpha = Tween.smoothstep(0, 1, t_trans);
            
            // Draw radius marks
            for (int i = 1; i <= r_marks; i++)
            {
                float r = i * r_inc;
                Graphics.draw_circle(Vector2.Zero, r, color * alpha);
            }

            // Draw angular marks
            float r_max = r_marks * r_inc;
            for (int i = 0; i < a_sections; i++)
            {
                float a = i * 2 * (float)Math.PI / a_sections;
                Vector2 n = new Vector2((float)Math.Cos(a), (float)Math.Sin(a));
                Graphics.draw_line(n * r_inc, n * r_max, color * alpha);
            }
        }

        public Vector2 closest(Vector2 v0)
        {
            // Get the initial radial pos
            float r0 = v0.Length();
            float a0 = (float)Math.Atan2(v0.Y, v0.X);

            // Limit the angle to 0 < a < 2 pi
            while (a0 > 2 * (float)Math.PI) a0 -= (float)Math.PI;
            while (a0 < 0) a0 += 2 * (float)Math.PI;

            // Calculate the snapped angle and radius
            float r1 = (float)Math.Round(r0 / r_inc, 0) * r_inc;
            float a1 = (float)Math.Round(a_sections * a0 / (2 * (float)Math.PI), 0) * 2 * (float)Math.PI / a_sections;

            // Return the snapped vector
            return new Vector2((float)Math.Cos(a1), (float)Math.Sin(a1)) * r1;
        }
    }
}
