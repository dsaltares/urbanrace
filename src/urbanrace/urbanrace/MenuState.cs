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
 * @file MenuState.cs
 * @description Menu state implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using Microsoft.Xna.Framework.Input;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace urbanrace
{
    class MenuState: State
    {
        // Background image
        protected Texture2D background;
        protected Vector2 backgroundPos;

        // GUI
        protected Vector2 trackPos;
        protected Texture2D leftArrow;
        protected Texture2D pressedLeftArrow;
        protected Texture2D rightArrow;
        protected Texture2D pressedRightArrow;
        protected Vector2 rightArrowPos;
        protected Vector2 leftArrowPos;
        protected bool rightPressed;
        protected bool leftPressed;

        // Text
        protected SpriteFont font;
        protected SpriteFont bigFont;
        protected Vector2 selectTrackPos;
        protected Vector2 trackNamePos;
        protected Vector2 recordPos;

        // Draw sprites
        protected SpriteBatch spriteBatch;

        // Selected track
        protected int selectedTrack;

        // Sound
        protected Cue optionSelected;
        protected Cue optionOver;
        protected Song song;
        
        public MenuState(UrbanRace game): base(game)
        {
            this.type = Type.MENU;

            // Create objects
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            // Positions should be relative to windows size
            backgroundPos = new Vector2(0.0f, 0.0f);
            trackPos = new Vector2(Settings.getOpt("menuTrackPanelX"), Settings.getOpt("menuTrackPanelY"));
            leftArrowPos = new Vector2(Settings.getOpt("menuLeftArrowX"), Settings.getOpt("menuLeftArrowY"));
            selectTrackPos = new Vector2(Settings.getOpt("menuSelectTrackX"), Settings.getOpt("menuSelectTrackY"));
            trackNamePos = new Vector2(Settings.getOpt("menuTrackNameX"), Settings.getOpt("menuTrackNameY"));
            recordPos = new Vector2(Settings.getOpt("menuRecordX"), Settings.getOpt("menuRecordY"));
            rightArrowPos = new Vector2(Settings.getOpt("menuRightArrowX"), Settings.getOpt("menuRightArrowY"));

            // Arrows pressed
            leftPressed = false;
            rightPressed = false;

            // Audio
            optionOver = null;
            optionSelected = null;
            song = null;

            // Initialize
            selectedTrack = 0;            
        }

        public override void update(GameTime gameTime)
        {
        }

        public override void draw(GameTime gameTime)
        {
            // Background
            spriteBatch.Begin();
            spriteBatch.Draw(background, backgroundPos, Color.White);
            spriteBatch.End();

            // Track panel
            spriteBatch.Begin();
            spriteBatch.Draw(TrackManager.tracks[selectedTrack].image, trackPos, Color.White);
            spriteBatch.End();

            // Left arrow
            spriteBatch.Begin();
            if (leftPressed)
                spriteBatch.Draw(pressedLeftArrow, leftArrowPos, Color.White);
            else
                spriteBatch.Draw(leftArrow, leftArrowPos, Color.White);
            spriteBatch.End();
            
            // Right arrow
            spriteBatch.Begin();
            if (rightPressed)
                spriteBatch.Draw(pressedRightArrow, rightArrowPos, Color.White);
            else
                spriteBatch.Draw(rightArrow, rightArrowPos, Color.White);
            spriteBatch.End();
            
            // Select a track
            spriteBatch.Begin();
            spriteBatch.DrawString(bigFont, "Select track", selectTrackPos, Color.White);
            spriteBatch.End();

            // Track name
            spriteBatch.Begin();
            spriteBatch.DrawString(font, TrackManager.tracks[selectedTrack].name, trackNamePos, Color.White);
            spriteBatch.End();

            // Track récord
            spriteBatch.Begin();
            if (TrackManager.tracks[selectedTrack].record == 0.0)
                spriteBatch.DrawString(font, "Record: (not set)", recordPos, Color.White);
            else
                spriteBatch.DrawString(font, "Record: " + Utility.timeToString((int)TrackManager.tracks[selectedTrack].record), recordPos, Color.White);
            spriteBatch.End();
        }

        public override void load()
        {
            Log.log(Log.Type.INFO, "Loading state menu type " + type);
            
            // Background
            background = game.Content.Load<Texture2D>("Images\\menuBackground");

            // Arrows
            leftArrow = game.Content.Load<Texture2D>("Images\\leftArrow");
            pressedLeftArrow = game.Content.Load<Texture2D>("Images\\pressedLeftArrow");
            rightArrow = game.Content.Load<Texture2D>("Images\\rightArrow");
            pressedRightArrow = game.Content.Load<Texture2D>("Images\\pressedRightArrow");

            // Font
            font = game.Content.Load<SpriteFont>("Fonts\\Menu");
            bigFont = game.Content.Load<SpriteFont>("Fonts\\MenuBig");

            // Song
            song = game.Content.Load<Song>("Audio\\menu");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(song);

            // Set callbacks
            Input.setKeyPressedCallback(new Input.KeyboardCallback(keyPressed));
            Input.setKeyReleasedCallback(new Input.KeyboardCallback(keyReleased));
            
            loaded = true;
        }

        public override void unload()
        {
            // Set objects to null so they can bre freed
            background = null;
            leftArrow = null;
            pressedLeftArrow = null;
            rightArrow = null;
            pressedRightArrow = null;
            optionOver = null;
            optionSelected = null;

            // Music
            MediaPlayer.Stop();
            song = null;

            // Clear input callbacks
            Input.clearCallbacks();
            
            loaded = false;
        }

        public override void keyPressed(Keys key)
        {
            if (key == Keys.Left)
            {
                // Left arrow is pressed
                leftPressed = true;
                optionOver = game.soundBank.GetCue("optionover");
                optionOver.Play();
            }
            else if (key == Keys.Right)
            {
                // Right arrow is pressed
                rightPressed = true;
                optionOver = game.soundBank.GetCue("optionover");
                optionOver.Play();
            }
            else if (key == Keys.Enter)
            {
                // Select track
                optionSelected = game.soundBank.GetCue("optionselected");
                optionSelected.Play();
                GameState gameState = (GameState)game.states[Type.GAME];
                gameState.levelName = TrackManager.tracks[selectedTrack].filename;
                gameState.laps = TrackManager.tracks[selectedTrack].laps;
                Log.log(Log.Type.INFO, "Time for this track: " + TrackManager.tracks[selectedTrack].time);
                gameState.totalTime = TrackManager.tracks[selectedTrack].time;
                game.changeState(Type.GAME);
            }

        }

        public override void keyReleased(Keys key)
        {
            if (key == Keys.Left)
            {
                // Left arrow is released
                --selectedTrack;
                if (selectedTrack < 0)
                    selectedTrack = TrackManager.tracks.Count - 1;
                leftPressed = false;
            }
            else if (key == Keys.Right)
            {
                // Right arrow is released
                ++selectedTrack;
                if (selectedTrack == TrackManager.tracks.Count)
                    selectedTrack = 0;
                rightPressed = false;
            }
        }
    }
}
