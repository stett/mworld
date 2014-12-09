using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace mworld
{
    public class MouseData
    {
        //
        // Members
        //

        private Vector2 screen_pos;
        private Vector2 world_pos;
        private Vector2 world_pos_diff;
        private int scroll_wheel;
        private int scroll_wheel_diff;
        private bool left_is_down;
        private bool right_is_down;
        private bool left_is_pressed;
        private bool right_is_pressed;
        private bool left_is_released;
        private bool right_is_released;

        //
        // 'Tors
        //

        public MouseData()
        {
            screen_pos = new Vector2();
            world_pos = new Vector2();
            world_pos_diff = new Vector2();
            scroll_wheel = 0;
            scroll_wheel_diff = 0;
        }

        //
        // Methods
        //

        public void update(float dt, View view)
        {
            screen_pos.X = Mouse.GetState().X;
            screen_pos.Y = Mouse.GetState().Y;

            Vector2 new_world_pos = Vector2.Transform(screen_pos, Matrix.Invert(view.get_trans()));
            world_pos_diff = new_world_pos - world_pos;
            world_pos = new_world_pos;

            int new_scroll_wheel = Mouse.GetState().ScrollWheelValue;
            scroll_wheel_diff = new_scroll_wheel - scroll_wheel;
            scroll_wheel = new_scroll_wheel;

            left_is_pressed = left_is_released = false;
            right_is_pressed = right_is_released = false;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!left_is_down)
                {
                    left_is_pressed = true;
                }
                left_is_down = true;
            }
            else
            {
                if (left_is_down)
                {
                    left_is_released = true;
                }
                left_is_down = false;
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                if (!right_is_down)
                {
                    right_is_pressed = true;
                }
                right_is_down = true;
            }
            else
            {
                if (right_is_down)
                {
                    right_is_released = true;
                }
                right_is_down = false;
            }
        }

        public Vector2 get_screen_pos()
        {
            return screen_pos;
        }

        public Vector2 get_world_pos()
        {
            return world_pos;
        }

        public Vector2 get_world_pos_diff()
        {
            return world_pos_diff;
        }

        public int get_scroll_wheel()
        {
            return scroll_wheel;
        }

        public int get_scroll_wheel_diff()
        {
            return scroll_wheel_diff;
        }

        public bool left_down()
        {
            return left_is_down;
        }

        public bool right_down()
        {
            return right_is_down;
        }

        public bool left_clicked()
        {
            return left_is_pressed;
        }

        public bool right_clicked()
        {
            return right_is_pressed;
        }

        public bool left_released()
        {
            return left_is_released;
        }

        public bool right_released()
        {
            return right_is_released;
        }
    }
}
