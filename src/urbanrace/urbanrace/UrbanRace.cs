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
 * @file UrbanRace.cs
 * @description UrbanRace game implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

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
using System.Xml;

namespace urbanrace
{
    public class UrbanRace : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public AudioEngine audioEngine;
        public WaveBank waveBank;
        public SoundBank soundBank;

        public Camera camera { get; protected set; }


        public Dictionary<State.Type, State> states { get; protected set; }
        protected State currentState;

        public UrbanRace()
        {
            // Init XNA graphics
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Init log system
            Log.init();

            // Load configuration
            Settings.loadSettings(Content.RootDirectory + "\\XML\\settings.xml");

            // Set window size
            graphics.PreferredBackBufferHeight = (int)Settings.getOpt("windowHeight");
            graphics.PreferredBackBufferWidth = (int)Settings.getOpt("windowWidth");
            Window.Title = "Urban Race v" + (int)Settings.getOpt("gameV1") + "." + (int)Settings.getOpt("gameV2");

            // Init input system
            Input.init();
        }

        protected override void Initialize()
        {
            // Init shape collision dispatching
            Shape.initialiseCollisionTests();

            // Load tracks
            TrackManager.loadTracks(this);

            // Camera
            camera = new Camera(this);
            
            // States initialization
            states = new Dictionary<State.Type, State>();
            states[State.Type.MENU] = new MenuState(this);
            states[State.Type.GAME] = new GameState(this);
            states[State.Type.VICTORY] = new VictoryState(this);
            states[State.Type.DEFEAT] = new DefeatState(this);

            currentState = states[State.Type.MENU];

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Audio
            audioEngine = new AudioEngine("Content\\Audio\\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");

            // Current state
            currentState.load();
        }

        protected override void UnloadContent()
        {
            // Unload current state
            currentState.unload();
        }

        protected override void Update(GameTime gameTime)
        {
            // Update input system
            Input.update();
            
            // Current state update
            currentState.update(gameTime);

            // Update audio
            audioEngine.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Set backgroundd color
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            // Prepare 3D rendering
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Current state draw
            currentState.draw(gameTime);

            base.Draw(gameTime);
        }

        public void changeState(State.Type type)
        {
            // Unload current state's content
            currentState.unload();

            // Assign new current state
            currentState = states[type];

            // Load next state's content
            currentState.load();
        }
    }
}
