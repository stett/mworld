using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace mworld
{
    static class Graphics
    {
        //
        // Global graphics data
        //

        static Texture2D pixel;
        static Dictionary<string, Texture2D> textures;
        static SpriteFont sprite_font;
        static SpriteBatch batch;
        static bool initialized = false;

        //
        // Global graphics functions
        //

        public static void initialize(GraphicsDevice graphics_device)
        {
            if (initialized) return;
            initialized = true;

            pixel = new Texture2D(graphics_device, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });

            textures = new Dictionary<string, Texture2D>();

            batch = new SpriteBatch(graphics_device);
        }

        public static void load_textures(ContentManager content_manager)
        {
            // Load the textures directory info, abort if none
            DirectoryInfo dir = new DirectoryInfo(content_manager.RootDirectory + "/graphics");
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            // Load all files that matches the file filter
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string f_name = Path.GetFileNameWithoutExtension(file.Name);
                //textures.Add(content_manager.Load<Texture2D>("graphics/" + f_name));
                textures.Add(f_name, content_manager.Load<Texture2D>("graphics/" + f_name));
            }

            // Load the font
            sprite_font = content_manager.Load<SpriteFont>("Consolas");
        }

        public static Texture2D get_tex(string f_name)
        {
            return textures[f_name];
        }

        public static void begin(View view) {
            batch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, view.get_trans());
        }

        public static void end() { batch.End(); }

        public static void draw_tex(string tex_name, Rectangle dest_rect, Color color)
        {
            Texture2D tex;
            if (!textures.TryGetValue(tex_name, out tex)) return;
            batch.Draw(tex, dest_rect, color);
        }
        public static void draw_tex(string tex_name, Vector2 pos, Color color)
        {
            Texture2D tex;
            if (!textures.TryGetValue(tex_name, out tex)) return;
            batch.Draw(tex, pos - new Vector2(tex.Width, tex.Height) * .5f, color);
        }
        public static void draw_tex(string tex_name, Vector2 pos, Rectangle source_rect, Color color, float angle, Vector2 origin, float scale, SpriteEffects effects, float layer_depth)
        {
            Texture2D tex;
            if (!textures.TryGetValue(tex_name, out tex)) return;
            batch.Draw(tex, pos, source_rect, color, angle, origin, scale, effects, layer_depth);
        }
        public static void draw_tex(string tex_name, Vector2 pos, float angle)
        {
            Texture2D tex;
            if (!textures.TryGetValue(tex_name, out tex)) return;
            batch.Draw(tex, pos, null, Color.White, angle, new Vector2(tex.Width, tex.Height) * 0.5f, 1.0f, SpriteEffects.None, 0f);
        }
        public static void draw_tex(string tex_name, Vector2 pos, float angle, float scale)
        {
            Texture2D tex;
            if (!textures.TryGetValue(tex_name, out tex)) return;
            batch.Draw(tex, pos, null, Color.White, angle, new Vector2(tex.Width, tex.Height) * 0.5f, scale, SpriteEffects.None, 0f);
        }
        public static void draw_tex(string tex_name, Vector2 pos, float angle, float scale, Color color)
        {
            Texture2D tex;
            if (!textures.TryGetValue(tex_name, out tex)) return;
            batch.Draw(tex, pos, null, color, angle, new Vector2(tex.Width, tex.Height) * 0.5f, scale, SpriteEffects.None, 0f);
        }
        public static void draw_tex(string tex_name, Vector2 pos, float angle, float scale, Color color, float alpha)
        {
            Texture2D tex;
            if (!textures.TryGetValue(tex_name, out tex)) return;
            batch.Draw(tex, pos, null, color * alpha, angle, new Vector2(tex.Width, tex.Height) * 0.5f, scale, SpriteEffects.None, 0f);
        }
        public static void draw_tex(string tex_name, Vector2 pos, float angle, float scale, Color color, float alpha, float depth)
        {
            Texture2D tex;
            if (!textures.TryGetValue(tex_name, out tex)) return;
            batch.Draw(tex, pos, null, color * alpha, angle, new Vector2(tex.Width, tex.Height) * 0.5f, scale, SpriteEffects.None, depth);
        }

        public static void draw_line(Vector3 a, Vector3 b, Color color, float width = 1f, bool scale_width = false, float depth = 0f)
        {
            draw_line(new Vector2(a.X, a.Y), new Vector2(b.X, b.Y), color, width, scale_width, depth);
        }
        public static void draw_line(Vector2 a, Vector2 b, Color color, float width = 1f, bool scale_width = false, float depth = 0f)
        {
            float angle = (float)Math.Atan2(b.Y - a.Y, b.X - a.X);
            float length = Vector2.Distance(a, b) + width * .1f;
            if (!scale_width) width /= MWorld.view.get_scale();
            batch.Draw(pixel, a, null, color, angle, new Vector2(0, .5f), new Vector2(length, width), SpriteEffects.None, depth);
        }

        public static void draw_arrow(Vector2 a, Vector2 b, Color color, float width = 1f, bool scale_width = false, float depth = 0f)
        {
            Vector2 diff = b - a;
            Vector2 n = diff;
            n.Normalize();
            draw_line(a, b, color, width, scale_width);
            draw_tex("arrowhead", b, (float)Math.Atan2(diff.Y, diff.X) + .5f * (float)Math.PI, (width / 30f) / MWorld.view.get_scale(), color, depth);
        }

        public static void draw_circle(Vector3 pos, float rad, Color color, float width = 1f, bool scale_width = false, float depth = 0f)
        {
            draw_circle(new Vector2(pos.X, pos.Y), rad, color, width, scale_width, depth);
        }
        public static void draw_circle(Vector2 pos, float rad, Color color, float width = 1f, bool scale_width = false, float depth = 0f)
        {
            float a0, a1, num;
            num = Math.Min(rad, 100);
            for (int i = 0; i < (int)num; i++)
            {
                a0 = (float)i * 2.0f * (float)Math.PI / num;
                a1 = (float)(i + 1) * 2.0f * (float)Math.PI / num;
                draw_line(
                    pos + new Vector2((float)Math.Cos(a0) * rad, (float)Math.Sin(a0) * rad),
                    pos + new Vector2((float)Math.Cos(a1) * rad, (float)Math.Sin(a1) * rad),
                    color, width, scale_width, depth);
            }
        }

        public static void draw_string(string text, Vector2 pos, float angle, Color color, float alpha, float scale, float depth=0f)
        {
            batch.DrawString(sprite_font, text, pos, color * alpha, angle, Vector2.Zero, scale / MWorld.view.get_scale(), SpriteEffects.None, depth);
        }
    }
}
