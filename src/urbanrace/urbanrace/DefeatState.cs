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
 * @file DefeatState.cs
 * @description Defeat state implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace urbanrace
{
    class DefeatState: State
    {
        public enum Options
        {
            RETRY,
            MENU
        }

        protected SpriteBatch spriteBatch;

        protected Texture2D background;
        protected Texture2D button;
        protected Texture2D pressedButton;
        protected Texture2D selectedButton;
        protected SpriteFont retroFont;

        protected Vector2 retryButtonPos;
        protected Vector2 retryTextPos;
        protected Vector2 menuButtonPos;
        protected Vector2 menuTextPos;

        protected Options selected;

        // Sound
        protected Cue optionSelected;
        protected Cue optionOver;
        protected Song song;

        public DefeatState(UrbanRace game): base(game)
        {
            this.type = Type.DEFEAT;

            this.spriteBatch = new SpriteBatch(game.GraphicsDevice);
            this.background = null;
            this.button = null;
            this.pressedButton = null;
            this.selectedButton = null;
            this.retroFont = null;

            retryButtonPos = new Vector2(Settings.getOpt("defeatRetryButtonX"), Settings.getOpt("defeatRetryButtonY"));
            retryTextPos = new Vector2(Settings.getOpt("defeatRetryTextX"), Settings.getOpt("defeatRetryTextY"));
            menuButtonPos = new Vector2(Settings.getOpt("defeatMenuButtonX"), Settings.getOpt("defeatMenuButtonY"));
            menuTextPos = new Vector2(Settings.getOpt("defeatMenuTextX"), Settings.getOpt("defeatMenuTextY"));

            // Audio
            optionOver = null;
            optionSelected = null;
            song = null;
        }

        public override void update(GameTime gameTime)
        {
        }

        public override void draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0.0f, 0.0f), Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            if (selected == Options.RETRY && Input.getKeyboard().IsKeyDown(Keys.Enter))
                spriteBatch.Draw(pressedButton, retryButtonPos, Color.White);
            else if (selected == Options.RETRY)
                spriteBatch.Draw(selectedButton, retryButtonPos, Color.White);
            else
                spriteBatch.Draw(button, retryButtonPos, Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(retroFont, "Retry", retryTextPos, Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            if (selected == Options.MENU && Input.getKeyboard().IsKeyDown(Keys.Enter))
                spriteBatch.Draw(pressedButton, menuButtonPos, Color.White);
            else if (selected == Options.MENU)
                spriteBatch.Draw(selectedButton, menuButtonPos, Color.White);
            else
                spriteBatch.Draw(button, menuButtonPos, Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(retroFont, "Return to menu", menuTextPos, Color.White);
            spriteBatch.End();
        }

        public override void load()
        {
            Log.log(Log.Type.INFO, "Loading state defeat type " + type);
            
            this.background = game.Content.Load<Texture2D>("Images\\defeatBackground");
            this.button = game.Content.Load<Texture2D>("Images\\button");
            this.selectedButton = game.Content.Load<Texture2D>("Images\\selectedButton");
            this.pressedButton = game.Content.Load<Texture2D>("Images\\pressedButton");
            this.retroFont = game.Content.Load<SpriteFont>("Fonts\\MenuBig");

            // Song
            song = game.Content.Load<Song>("Audio\\defeat");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(song);

            // Set input callbacks
            Input.setKeyReleasedCallback(keyReleased);
            Input.setKeyPressedCallback(keyPressed);

            this.selected = Options.RETRY;

            loaded = true;
        }

        public override void unload()
        {
            this.background = null;
            this.button = null;
            this.pressedButton = null;
            this.selectedButton = null;
            this.retroFont = null;

            MediaPlayer.Stop();
            this.song = null;
            this.optionOver = null;
            this.optionSelected = null;

            Input.clearCallbacks();

            loaded = false;
        }

        public override void keyPressed(Keys key)
        {
            if (key == Keys.Enter)
            {
                optionSelected = game.soundBank.GetCue("optionselected");
                optionSelected.Play();

                if (selected == Options.MENU)
                    game.changeState(Type.MENU);
                else
                    game.changeState(Type.GAME);
            }

            if (key == Keys.Up)
            {
                selected = selected == Options.MENU ? Options.RETRY : Options.MENU;
                optionOver = game.soundBank.GetCue("optionover");
                optionOver.Play();
            }
            else if (key == Keys.Down)
            {
                selected = selected == Options.MENU ? Options.RETRY : Options.MENU;
                optionOver = game.soundBank.GetCue("optionover");
                optionOver.Play();
            }
        }
    }
}
