/*
 * This file is part of Urban Race.
 * 
 * Urban Race is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Urban Race is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Urban Race.  If not, see <http://www.gnu.org/licenses/>.
 */

/*
 * @file Input.cs
 * @description Input manager implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace urbanrace
{
    class Input
    {
        public delegate void KeyboardCallback(Keys key);
        public delegate void PadMovedCallback(Vector2 leftPad, Vector2 rightPad, float leftTrigger, float rightTrigger);
        public delegate void PadButtonCallback(Buttons button);
        public delegate void MouseMovedCallback(Vector3 delta);
        public delegate void MouseButtonCallback(bool left, bool right, bool middle);
        
        protected static KeyboardState newKeyboard;
        protected static KeyboardState oldKeyboard;
        protected static MouseState newMouse;
        protected static MouseState oldMouse;
        protected static GamePadState newGamePad;
        protected static GamePadState oldGamePad;

        protected static KeyboardCallback keyPressedCallback;
        protected static KeyboardCallback keyReleasedCallback;
        protected static PadMovedCallback padMovedCallback;
        protected static PadButtonCallback buttonPressedCallback;
        protected static PadButtonCallback buttonReleasedCallback;
        protected static MouseMovedCallback mouseMovedCallback;
        protected static MouseButtonCallback mousePressedCallback;
        protected static MouseButtonCallback mouseReleasedCallback;

        public static IEnumerable<T> GetValues<T>()
        {
            return (from x in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public)
                    select (T)x.GetValue(null));
        } 

        public static void init()
        {
            keyPressedCallback = null;
            keyReleasedCallback = null;
            padMovedCallback = null;
            buttonPressedCallback = null;
            buttonReleasedCallback = null;
            mouseMovedCallback = null;
            mousePressedCallback = null;
            mouseReleasedCallback = null;
        }

        public static void update()
        {
            // Update new states
            newKeyboard = Keyboard.GetState();
            newMouse = Mouse.GetState();
            newGamePad = GamePad.GetState(PlayerIndex.One);

            // COMPARE NEW WITH OLD

            // Keyboard keyPressed and keyReleased
            foreach (Keys key in GetValues<Keys>())
            {
                if (oldKeyboard.IsKeyUp(key) && newKeyboard.IsKeyDown(key) && keyPressedCallback != null)
                    keyPressedCallback(key);
                else if (oldKeyboard.IsKeyDown(key) && newKeyboard.IsKeyUp(key) && keyReleasedCallback != null)
                    keyReleasedCallback(key);
            }

            // Gamepad padMoved
            
            // Get deltas
            Vector2 deltaLeftPad = new Vector2(newGamePad.ThumbSticks.Left.X - oldGamePad.ThumbSticks.Left.X,
                                               newGamePad.ThumbSticks.Left.Y - oldGamePad.ThumbSticks.Left.Y);

            Vector2 deltaRightPad = new Vector2(newGamePad.ThumbSticks.Right.X - oldGamePad.ThumbSticks.Right.X,
                                               newGamePad.ThumbSticks.Right.Y - oldGamePad.ThumbSticks.Right.Y);

            float deltaLeftTrigger = newGamePad.Triggers.Left - oldGamePad.Triggers.Left;
            float deltaRightTrigger = newGamePad.Triggers.Right - oldGamePad.Triggers.Right;

            if ((deltaLeftPad.LengthSquared() != 0.0f || deltaRightPad.LengthSquared() != 0.0f || deltaLeftTrigger != 0.0f || deltaRightTrigger != 0.0f) && padMovedCallback != null)
                padMovedCallback(deltaLeftPad, deltaRightPad, deltaLeftTrigger, deltaRightTrigger);

            // Gamepad buttonPressed and buttonReleased
            foreach (Buttons button in GetValues<Buttons>())
            {
                if (oldGamePad.IsButtonUp(button) && newGamePad.IsButtonDown(button) && buttonPressedCallback != null)
                    buttonPressedCallback(button);
                else if (oldGamePad.IsButtonDown(button) && newGamePad.IsButtonUp(button) && buttonReleasedCallback != null)
                    buttonReleasedCallback(button);
            }
            
            // Mouse moved
            Vector3 delta = new Vector3(newMouse.X - oldMouse.X,
                                        newMouse.Y - oldMouse.Y,
                                        newMouse.ScrollWheelValue - oldMouse.ScrollWheelValue);
            
            if (delta.LengthSquared() != 0 && mouseMovedCallback != null)
                mouseMovedCallback(delta);

            // Mouse button pressed/released
            bool leftPressed = oldMouse.LeftButton == ButtonState.Released && newMouse.LeftButton == ButtonState.Pressed;
            bool rightPressed = oldMouse.RightButton == ButtonState.Released && newMouse.RightButton == ButtonState.Pressed;
            bool middlePressed = oldMouse.MiddleButton == ButtonState.Released && newMouse.MiddleButton == ButtonState.Pressed;

            bool leftReleased = oldMouse.LeftButton == ButtonState.Pressed && newMouse.LeftButton == ButtonState.Released;
            bool rightReleased = oldMouse.RightButton == ButtonState.Pressed && newMouse.RightButton == ButtonState.Released;
            bool middleReleased = oldMouse.MiddleButton == ButtonState.Pressed && newMouse.MiddleButton == ButtonState.Released;

            if ((leftPressed || rightPressed || middlePressed) && mousePressedCallback != null)
                mousePressedCallback(leftPressed, rightPressed, middlePressed);

            if ((leftReleased || rightReleased || middleReleased) && mouseReleasedCallback != null)
                mouseReleasedCallback(leftReleased, rightReleased, middleReleased);

            // Update old states
            oldKeyboard = newKeyboard;
            oldMouse = newMouse;
            oldGamePad = newGamePad;
        }

        public static KeyboardState getKeyboard()
        {
            return newKeyboard;
        }

        public static MouseState getMouse()
        {
            return newMouse;
        }

        public static GamePadState getGamePad()
        {
            return newGamePad;
        }

        public static void setKeyPressedCallback(KeyboardCallback callback)
        {
            keyPressedCallback = callback;
        }

        public static void setKeyReleasedCallback(KeyboardCallback callback)
        {
            keyReleasedCallback = callback;
        }

        public static void setPadMovedCallback(PadMovedCallback callback)
        {
            padMovedCallback = callback;
        }

        public static void setButtonPressedCallback(PadButtonCallback callback)
        {
            buttonPressedCallback = callback;
        }

        public static void setButtonReleasedCallback(PadButtonCallback callback)
        {
            buttonReleasedCallback = callback;
        }

        public static void setMouseMovedCallback(MouseMovedCallback callback)
        {
            mouseMovedCallback = callback;
        }

        public static void setMousePressedCallback(MouseButtonCallback callback)
        {
            mousePressedCallback = callback;
        }

        public static void setMouseReleasedCallback(MouseButtonCallback callback)
        {
            mouseReleasedCallback = callback;
        }

        public static void clearCallbacks()
        {
            // Clear callbacks
            keyPressedCallback = null;
            keyReleasedCallback = null;
            padMovedCallback = null;
            buttonPressedCallback = null;
            buttonReleasedCallback = null;
            mouseMovedCallback = null;
            mousePressedCallback = null;
            mouseReleasedCallback = null;

            // Reset states
            oldMouse = newMouse;
            oldKeyboard = newKeyboard;
            oldGamePad = newGamePad;
        }
    }
}
