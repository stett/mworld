using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mworld
{
    /// <summary>
    /// Describes the pos and angle of the game view
    /// </summary>
    public class View
    {
        //
        // Members
        //

        Vector2 pos;
        public Vector2 target_pos;
        float angle;
        public float target_angle;
        float scale;
        public float target_scale;
        Matrix transformation;

        //
        // 'Tors
        //

        public View()
        {
            pos = Vector2.Zero;
            angle = 0;
            scale = 1;
            target_scale = .5f;
            transformation = Matrix.Identity;
        }


        //
        // Methods
        //

        public Vector3 pos3()
        {
            return new Vector3(pos.X, pos.Y, 0);
        }

        public void update(float dt, GraphicsDevice graphics_device)
        {

            float max_scale = 1.0f;
            float min_scale = .1f;
            if (scale > max_scale) scale = max_scale;
            if (scale < min_scale) scale = min_scale;
            if (target_scale > max_scale) target_scale = max_scale;
            if (target_scale < min_scale) target_scale = min_scale;

            pos += (target_pos - pos) * .1f;
            scale += (target_scale - scale) * .1f;
            angle += (target_angle - angle) * .1f;

            transformation = Matrix.CreateTranslation(-pos3()) *
                             Matrix.CreateRotationZ(angle) *
                             Matrix.CreateScale(scale) *
                             Matrix.CreateTranslation(new Vector3(graphics_device.Viewport.Width * .5f, graphics_device.Viewport.Height * .5f, 0f));
        }
        
        public Matrix get_trans() { return transformation; }
        public Vector2 get_pos() { return pos; }
        public float get_angle() { return angle; }
        public float get_scale() { return scale; }
    }
}
