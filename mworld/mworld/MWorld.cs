using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using mworld.Parts;

namespace mworld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MWorld : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public static State state = new State();
        public static View view = new View();
        public static MouseData mouse = new MouseData();
        public static Grid grid = new Grid();
        public static PartSelector hovered_part = new PartSelector();
        public static PartSelector selected_part = new PartSelector();
        public static PartPathVertex hovered_vert = null;
        public static PartPathVertex selected_vert = null;
        public static Color part_color = new Color(20, 20, 20);
        public static Color back_color = new Color(240, 240, 240);
        public static float hud_rotation = 0;
        public static PotentialConnection potential_connection = null;
        public static SpriteFont sprite_font;

        public enum GameMode { Edit, Play };
        public static GameMode mode = GameMode.Edit;

        public static List<Part> part_list = new List<Part>();
        public static int part_list_index = 0;

        public MWorld()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            // Load the part list
            Generator generator = new Generator();
            generator.marbles.Add(new Marble());
            generator.marbles.Add(new Marble());
            generator.marbles.Add(new Marble());
            generator.marbles.Add(new Marble());
            generator.marbles.Add(new Marble());
            generator.marbles.Add(new Marble());
            generator.marbles.Add(new Marble());
            generator.marbles.Add(new Marble());
            
            part_list.Add(generator);
            part_list.Add(new Alternator());
            part_list.Add(new Trigger());
            part_list.Add(new Line());
            part_list.Add(new Spline2());
            part_list.Add(new Merger());

            // Set up graphics
            graphics.IsFullScreen = false;
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 700;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any component
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //
            Graphics.initialize(GraphicsDevice);
            this.IsMouseVisible = true;

            // Perform base class initialization
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Graphics.load_textures(Content);

            // Load the font
            sprite_font = Content.Load<SpriteFont>("Consolas");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gt">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gt)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Switch between play and edit modes
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                mode = GameMode.Play;
                selected_part.part = null;
                selected_vert = null;
            }
            else
            {
                mode = GameMode.Edit;
                if (state.marbles.Count() > 0)
                {
                    foreach (Marble marble in state.marbles)
                        marble.depath(false);
                    state.marbles.Clear();
                }
            }

            // TODO: FIGURE OUT HOW TO GET AN ACTUAL dt
            float dt = 0;

            // Update hud rotation
            hud_rotation += .01f;
            while (hud_rotation > 2f * (float)Math.PI) hud_rotation -= 2f * (float)Math.PI;

            // Update the mouse pos
            mouse.update(dt, view);

            // Control the view
            int scroll_wheel_diff = mouse.get_scroll_wheel_diff();
            if (mouse.right_down())
            {
                if (scroll_wheel_diff > 0) part_list_index++;
                else if (scroll_wheel_diff < 0) part_list_index--;

                if (part_list_index > part_list.Count - 1) part_list_index = 0;
                else if (part_list_index < 0) part_list_index = part_list.Count - 1;

                if (mouse.left_clicked())
                {
                    state.parts.Add(part_list[part_list_index].new_copy());
                }
            }
            else if (mouse.left_down())
            {
                if (selected_part.part != null)
                {
                    if (selected_part.part.can_rotate())
                    {
                        selected_part.part.angle += (float)scroll_wheel_diff * (float)Math.PI / (10 * 180);
                        if (scroll_wheel_diff != 0) selected_part.part.disconnect_verts();
                    }
                }
                else 
                {
                    view.target_pos -= 3 * mouse.get_world_pos_diff();
                    view.target_angle += (float)mouse.get_scroll_wheel_diff() * (float)Math.PI / (10 * 180);
                }
            }
            else
            {
                view.target_scale += (float)mouse.get_scroll_wheel_diff() / 1000.0f;
            }
            
            // Turn the grid on & off
            grid.on = Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Keyboard.GetState().IsKeyDown(Keys.RightControl);

            // Update the view
            view.update(dt, GraphicsDevice);

            // Update all parts
            foreach (Part part in state.parts)
                part.update(dt);

            // Update all marbles
            if (mode == GameMode.Play)
                foreach (Marble marble in state.marbles)
                    marble.update(dt);

            if (mode == GameMode.Edit)
            {
                // Look for a hovered vert
                hovered_vert = null;
                if (selected_part.part != null && selected_part.part.can_edit_endpoints())
                    foreach (PartPath path in selected_part.part.paths)
                        for (int i = 0; i < path.num_verts(); i += path.num_verts() - 1)
                        {
                            PartPathVertex vert = path.get_vert(i);
                            Vector3 pos3 = Vector3.Transform(vert.pos3(), selected_part.part.get_trans());
                            Vector2 pos = new Vector2(pos3.X, pos3.Y);
                            if ((pos - mouse.get_world_pos()).Length() < Graphics.get_tex("node").Width * .5 / view.get_scale())
                                hovered_vert = vert;
                        }

                // Look for a hovered part
                hovered_part.part = null;
                float r_min = -1;
                if (hovered_vert == null)
                    foreach (Part part in state.parts)
                        if ((r_min == -1 || part.get_r() < r_min) &&
                            (part.pos - mouse.get_world_pos()).Length() < part.get_r())
                        {
                            r_min = part.get_r();
                            hovered_part.part = part;
                        }

                // If mouse clicked, try to select a part
                if (mouse.left_clicked())
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                    {
                        if (selected_part.part != null && hovered_part.part != null)
                        {
                            if (selected_part.part.is_connected_to_part(hovered_part.part))
                                selected_part.part.disconnect_part(hovered_part.part);
                            else
                                selected_part.part.connect_part(hovered_part.part);
                        }
                    }
                    else
                    {
                        if (hovered_vert != null) selected_vert = hovered_vert;
                        else
                        {
                            hovered_vert = null; selected_vert = null;
                            if (hovered_part.part != null) selected_part.part = hovered_part.part;
                            else selected_part.part = null;
                        }
                    }
                }

                // If a part or a vert is selected, try to drag it around,
                // and look for potential connections
                if (mouse.left_down())
                {
                    potential_connection = null;
                    if (selected_vert != null)
                    {
                        PartPath.disconnect_vert(selected_vert);
                        Vector2 pos = mouse.get_world_pos();
                        if (grid.on) pos = grid.closest(pos);
                        else
                        {
                            PartPathVertex close_vert;
                            pos = state.closest_vert(pos, 20, selected_vert, out close_vert);
                            if (close_vert != null) potential_connection = new PotentialConnection(selected_vert, close_vert);
                        }

                        Vector3 pos3 = new Vector3(pos.X, pos.Y, 0);
                        Vector3 pos3_trans = Vector3.Transform(pos3, Matrix.Invert(selected_part.part.get_trans()));
                        Vector2 pos_trans = new Vector2(pos3_trans.X, pos3_trans.Y);
                        selected_vert.pos = pos_trans;
                    }
                    else if (selected_part.part != null)
                    {
                        Vector2 pos0 = selected_part.part.pos;
                        if (grid.on)
                        {
                            Vector2 pos = grid.closest(mouse.get_world_pos());
                            selected_part.part.pos = pos;
                            if (selected_part.part.can_rotate())
                                selected_part.part.angle = (float)Math.Atan2(pos.Y, pos.X) + .5f * (float)Math.PI;
                        }
                        else
                        {
                            selected_part.part.pos += mouse.get_world_pos_diff();
                        }
                        if (pos0 != selected_part.part.pos) selected_part.part.disconnect_verts();
                    }
                }

                // Try to make new connections
                else if (mouse.left_released())
                {
                    if (potential_connection != null)
                    {
                        PartPath.connect_verts(potential_connection);
                        potential_connection = null;
                    }
                }
            }

            // Update the grid
            grid.update(dt);

            // Update the selectors
            hovered_part.update(dt);
            selected_part.update(dt);
            
            // Update the base class
            base.Update(gt);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gt">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gt)
        {
            // Clear the main buffer
            GraphicsDevice.Clear(back_color);

            // Draw the background grid
            Graphics.begin(view);
            grid.draw(Color.DeepSkyBlue);
            Graphics.end();

            // Start drawing
            Graphics.begin(view);

            // Draw the planet
            Graphics.draw_circle(Vector2.Zero, 1500, part_color, 64, true);
            
            // Draw each part
            foreach (Part part in state.parts)
                part.draw(part_color);

            // Draw each marble
            foreach (Marble marble in state.marbles)
                marble.draw();

            // Finish drawing
            Graphics.end();

            // Draw the selectors
            if (mode == GameMode.Edit)
            {
                Graphics.begin(view);
                selected_part.draw(Color.OrangeRed);
                hovered_part.draw(Color.White);
                if (selected_vert != null && selected_part.part != null)
                {
                    Vector3 pos3 = Vector3.Transform(selected_vert.pos3(), selected_part.part.get_trans());
                    Vector2 pos = new Vector2(pos3.X, pos3.Y);
                    Graphics.draw_tex("node", pos, hud_rotation * 5, 1 / view.get_scale(), Color.OrangeRed, 1, 0);
                }
                if (hovered_vert != null && selected_part.part != null)
                {
                    Vector3 pos3 = Vector3.Transform(hovered_vert.pos3(), selected_part.part.get_trans());
                    Vector2 pos = new Vector2(pos3.X, pos3.Y);
                    Graphics.draw_tex("node", pos, hud_rotation * 5, 1 / view.get_scale(), Color.White, 1, 0);
                }
                Graphics.end();
            }

            // Draw the construction menu
            if (mode == GameMode.Edit)
            {
                Graphics.begin(view);
                if (mouse.right_down())
                {
                    Part part = part_list[part_list_index];
                    part.pos = mouse.get_world_pos();
                    part.draw(Color.Gray);

                    Graphics.draw_string(part.name(), part.pos + new Vector2(part.get_r(), 0), -view.get_angle(), Color.OrangeRed, 1, 1);
                }
                Graphics.end();
            }

            // Call the base draw function
            base.Draw(gt);
        }
    }
}
