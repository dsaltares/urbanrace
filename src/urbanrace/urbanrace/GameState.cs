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
 * @file GameState.cs
 * @description Game state implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace urbanrace
{
    class GameState: State
    {
        public string levelName { get; set; }
        public int laps { get; set; }
        protected int lapsDone { get; set; }
        public double remainingTime { get; protected set; }
        public double totalTime { get; set; }
        public double playTime { get; protected set; }

        protected CollisionManager collisionManager;
        protected Level level;
        protected Car car;
        protected List<CheckPoint> checkPoints;
        protected List<TimeBonus> timeBonuses;
        protected List<GameObject> sceneObjects;
        protected List<GameObject> geometry;
        protected SkyBox skyBox;
        protected Terrain terrain;
        
        protected int nextCheckPoint;

        // BackgroundWorker terrainWorker;

        // Audio
        Cue pickTime;
        Cue carCrash;
        Song song;

        // GUI
        SpriteFont font;
        SpriteFont retroFont;
        Texture2D speedPanel;
        Texture2D timePanel;
        SpriteBatch spriteBatch;
        Vector2 speedPanelPos;
        Vector2 speedTextPos;
        Vector2 lapsPanelPos;
        Vector2 lapsText1Pos;
        Vector2 lapsText2Pos;
        Vector2 timePanelPos;
        Vector2 timeTextPos;
  
        public GameState (UrbanRace game): base(game)
        {
            // Initialise attributes and properties
            this.type = Type.GAME;
            this.laps = 1;
            this.levelName = "level.xml";
            this.level = null;
            this.car = null;
            this.checkPoints = new List<CheckPoint>();
            this.timeBonuses = new List<TimeBonus>();
            this.sceneObjects = new List<GameObject>();
            this.geometry = new List<GameObject>();
            this.skyBox = null;
            this.terrain = null;
            this.collisionManager = new CollisionManager();
            this.font = null;
            this.speedPanel = null;
            this.nextCheckPoint = 0;

            // Audio
            this.song = null;
            this.carCrash = null;
            this.pickTime = null;

            // GUI
            speedPanelPos = new Vector2(Settings.getOpt("gameSpeedPanelX"), Settings.getOpt("gameSpeedPanelY"));
            speedTextPos = new Vector2(Settings.getOpt("gameSpeedTextX"), Settings.getOpt("gameSpeedTextY"));
            lapsPanelPos = new Vector2(Settings.getOpt("gameLapsPanelX"), Settings.getOpt("gameLapsPanelY"));
            lapsText1Pos = new Vector2(Settings.getOpt("gameLapsText1X"), Settings.getOpt("gameLapsText1Y"));
            lapsText2Pos = new Vector2(Settings.getOpt("gameLapsText2X"), Settings.getOpt("gameLapsText2Y"));
            timePanelPos = new Vector2(Settings.getOpt("gameTimePanelX"), Settings.getOpt("gameTimePanelY"));
            timeTextPos = new Vector2(Settings.getOpt("gameTimeTextX"), Settings.getOpt("gameTimeTextY"));

            // Time (in ms)
            this.remainingTime = 0.0;
            this.totalTime = 0.0;
            this.playTime = 0.0;

            // Configure callbacks
            collisionManager.addCallback(GameObject.Type.CAR, GameObject.Type.SCENEOBJECT, collisionCarScene, CollisionManager.CallbackType.DURING);
            collisionManager.addCallback(GameObject.Type.CAR, GameObject.Type.TIMEBONUS, collisionCarBonus, CollisionManager.CallbackType.BEGIN);
            collisionManager.addCallback(GameObject.Type.CAR, GameObject.Type.CHECKPOINT, collisionCarCheckPoint, CollisionManager.CallbackType.BEGIN);

            // Load shapes
            CollisionManager.initShapeCatalog(game.Content.RootDirectory);

            this.spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public override void update (GameTime gameTime)
        {
            if (loaded)
            {
                // Update time
                remainingTime -= gameTime.ElapsedGameTime.Milliseconds;
                playTime += gameTime.ElapsedGameTime.Milliseconds;
                
                // Update car
                car.update(gameTime);
                
                // Update time bonuses
                foreach (TimeBonus timeBonus in timeBonuses)
                {
                    timeBonus.update(gameTime);
                }

                // Update check points
                foreach (CheckPoint checkpoint in checkPoints)
                {
                    checkpoint.update(gameTime);
                }

                // Update scene objects
                foreach (GameObject sceneObject in sceneObjects)
                {
                    sceneObject.update(gameTime);
                }

                // Update camera position (following car)
                game.camera.update(gameTime, car);

                // Collision detection
                collisionManager.checkCollisions();

                // Erase dead elements
                removeErasedCheckPoints();
                removeErasedTimeBonuses();

                // Game state changes
                if (remainingTime <= 0)
                {
                    // Stop engine
                    car.engine.Stop(AudioStopOptions.Immediate);

                    // Defeat
                    game.changeState(State.Type.DEFEAT);
                }

                else if (checkPoints.Count == 0)
                {
                    // Stop engine
                    car.engine.Stop(AudioStopOptions.Immediate);

                    // Check possible record
                    VictoryState victoryState = (VictoryState)(game.states[Type.VICTORY]);
                    victoryState.recordTime = playTime;

                    if (TrackManager.setNewRecord(levelName, playTime))
                    {
                        victoryState.isRecord = true;
                        TrackManager.saveTracks(game);
                    }
                    else
                        victoryState.isRecord = false;

                    // Victory
                    game.changeState(State.Type.VICTORY);
                }
            }
        }


        public override void draw(GameTime gameTime)
        {
            if (loaded)
            {
                // Draw car
                car.draw();

                // Draw time bonuses
                foreach (TimeBonus timeBonus in timeBonuses)
                {
                    timeBonus.draw();
                }

                // Draw scene objects
                foreach (GameObject sceneObject in sceneObjects)
                {
                    sceneObject.draw();
                }

                // Draw arbitrary geometry
                foreach (GameObject geometryObject in geometry)
                {
                    geometryObject.draw();
                }

                // Draw chekpoints (debug only)
                //foreach (CheckPoint checkpoint in checkPoints)
                //{
                //    checkpoint.draw();
                //}

                // Draw skybox if there is any
                skyBox.draw();

                // Draw terrain
                //terrain.draw();

                // Draw speed panel
                spriteBatch.Begin();
                spriteBatch.Draw(speedPanel, speedPanelPos, Color.White);
                spriteBatch.End();

                spriteBatch.Begin();
                spriteBatch.DrawString(font, "000 KM-H", speedTextPos, Color.Gray);
                spriteBatch.End();

                spriteBatch.Begin();
                spriteBatch.DrawString(font, String.Format("{0,3}", (int)car.velocity.Length() * 3) + " KM-H", speedTextPos, Color.Red);
                spriteBatch.End();

                // Draw time panel
                spriteBatch.Begin();
                spriteBatch.Draw(timePanel, timePanelPos, Color.White);
                spriteBatch.End();

                spriteBatch.Begin();
                spriteBatch.DrawString(font, "00:00:00", timeTextPos, Color.Gray);
                spriteBatch.End();

                spriteBatch.Begin();
                spriteBatch.DrawString(font, Utility.timeToString((int)remainingTime), timeTextPos, Color.Red);
                spriteBatch.End();

                // Draw laps panel
                spriteBatch.Begin();
                spriteBatch.Draw(timePanel, lapsPanelPos, Color.White);
                spriteBatch.End();

                spriteBatch.Begin();
                spriteBatch.DrawString(retroFont, "Laps", lapsText1Pos, Color.Red);
                spriteBatch.End();

                spriteBatch.Begin();
                spriteBatch.DrawString(font, "0-0", lapsText2Pos, Color.Gray);
                spriteBatch.End();

                spriteBatch.Begin();
                spriteBatch.DrawString(font, "" + lapsDone + "-" + laps, lapsText2Pos, Color.Red);
                spriteBatch.End();
            }
        }

        public override void  load()
        {
            Log.log(Log.Type.INFO, "Loading state game type " + type);
            
            // Parse level and create game objects
            level = new Level(this, levelName);
            level.load();

            // Sort check point list
            checkPoints.Sort();

            // Create car with level data
            car = new Car(game, level.carPosition, level.carOrientation);

            // Add gameobjects to collision manager
            collisionManager.addObject(car);

            foreach (TimeBonus timeBonus in timeBonuses)
            {
                collisionManager.addObject(timeBonus);
            }

            foreach (CheckPoint checkPoint in checkPoints)
            {
                collisionManager.addObject(checkPoint);
            }

            foreach (GameObject sceneObject in sceneObjects)
            {
                collisionManager.addObject(sceneObject);
            }

            font = game.Content.Load<SpriteFont>("Fonts\\LED");
            retroFont = game.Content.Load<SpriteFont>("Fonts\\MenuBig");
            speedPanel = game.Content.Load<Texture2D>("Images\\speedPanel");
            timePanel = game.Content.Load<Texture2D>("Images\\speedPanel");

            // Time (in ms)
            this.remainingTime = totalTime * 1000.0;
            this.playTime = 0.0f;

            // Song
            song = game.Content.Load<Song>("Audio\\game");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(song);

            // Set input callbacks
            Input.setKeyPressedCallback(keyPressed);
            Input.setKeyReleasedCallback(keyReleased);

            game.camera.setInitialPosition(car);

            // Next checkpoint
            nextCheckPoint = 0;

            lapsDone = 1;

            loaded = true;
        }

        public override void unload()
        {
            // Remove objects from collision manager
            collisionManager.removeAllObjects();
            
            // Set every game element to null so we can save memory
            car = null;
            level = null;
            checkPoints.Clear();
            timeBonuses.Clear();
            sceneObjects.Clear();
            geometry.Clear();
            skyBox = null;
            terrain = null;

            font = null;
            retroFont = null;
            speedPanel = null;
            timePanel = null;

            MediaPlayer.Stop();
            song = null;
            carCrash = null;
            pickTime = null;

            // Clear input callbacks
            Input.clearCallbacks();

            loaded = false;
        }

        private void removeErasedTimeBonuses()
        {
            for (int i = 0; i < timeBonuses.Count; )
            {
                if (timeBonuses[i].state == GameObject.State.ERASE)
                {
                    collisionManager.removeObject(timeBonuses[i]);
                    timeBonuses.RemoveAt(i);
                }
                else{
                    ++i;
                }
            }
        }

        private void removeErasedCheckPoints()
        {
            for (int i = 0; i < checkPoints.Count; )
            {
                if (checkPoints[i].state == GameObject.State.ERASE)
                {
                    collisionManager.removeObject(checkPoints[i]);
                    checkPoints.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
        }

        public void addGameObject(String modelName, Vector3 position, Quaternion orientation, float scale)
        {
            GameObject gameObject = new GameObject(game, modelName, position, orientation, scale);
            gameObject.type = GameObject.Type.SCENEOBJECT;
            sceneObjects.Add(gameObject);
        }

        public void addGeometry(String modelName, Vector3 position, Quaternion orientation, float scale)
        {
            geometry.Add(new GameObject(game, modelName, position, orientation, scale));
        }

        public void addTimeBonus(Vector3 position, Quaternion orientation, float scale, int seconds)
        {
            timeBonuses.Add(new TimeBonus(game, position, orientation, scale, seconds));
        }

        public void addCheckPoint(Vector3 position, Quaternion orientation, int number)
        {
            checkPoints.Add(new CheckPoint(game, position, orientation, number, laps));
        }

        public void setSkyBox(Vector3 position, Quaternion orientation, float scale)
        {
            skyBox = new SkyBox(game, position, orientation, scale);
        }

        public void setTerrain(string file, Vector3 position, Quaternion orientation, float scale)
        {
            terrain = new Terrain(game, file, position, orientation, scale);
        }

        public void collisionCarScene(GameObject o1, GameObject o2)
        {
            // Get objects
            Car car;
            GameObject sceneObject;

            if (o1.type == GameObject.Type.CAR)
            {
                car = (Car)o1;
                sceneObject = o2;
            }
            else
            {
                car = (Car)o2;
                sceneObject = o1;
            }

            OrientedBox obb1 = (OrientedBox)car.shape;
            OrientedBox obb2 = (OrientedBox)sceneObject.shape;

            // Crash sfx
            game.audioEngine.SetGlobalVariable("crashVolume", car.velocity.Length() / car.maxSpeed);
            carCrash = game.soundBank.GetCue("crash");
            carCrash.Play();

            // Restore oldPos
            car.position = car.oldPos;

            // Get closest point
            Vector3 q = CollisionManager.closestPointToOBB(obb1.center + obb1.transform.Translation, obb2);

            // Get normal vector
            Vector3 normal = obb1.center + obb1.transform.Translation - q;
            normal.Normalize();

            // Change car velocity to bounce
            float angle = (float)Math.Atan2(normal.Y - car.velocity.Y, normal.X - car.velocity.X);
            Vector3 newDirection = new Vector3((float)System.Math.Sin(-angle), (float)System.Math.Cos(-angle), 0.0f);
            newDirection.Normalize();
            car.velocity = newDirection * car.velocity.Length() * 0.85f;

            // Get angle between normal and car direction
            Vector2 carDirection = new Vector2((float)System.Math.Sin(-car.angle), (float)System.Math.Cos(-car.angle));
            Vector2 normal2D = new Vector2(normal.X, normal.Y);

            float angleNormalCar = (float)Math.Atan2(normal2D.Y - carDirection.Y, normal2D.X - carDirection.X);
        }

        public void collisionCarBonus(GameObject o1, GameObject o2)
        {
            // Get time bonus
            TimeBonus timeBonus;

            if (o1.type == GameObject.Type.TIMEBONUS)
                timeBonus = (TimeBonus)o1;
            else
                timeBonus = (TimeBonus)o2;

            // Play sound
            pickTime = game.soundBank.GetCue("picktime");
            pickTime.Play();

            // Add time
            remainingTime += timeBonus.seconds * 1000;

            // Delete timebonus
            timeBonus.state = GameObject.State.ERASE;
        }

        public void collisionCarCheckPoint(GameObject o1, GameObject o2)
        {
            // Get checkpoint
            CheckPoint checkpoint;

            if (o1.type == GameObject.Type.CHECKPOINT)
                checkpoint = (CheckPoint)o1;
            else
                checkpoint = (CheckPoint)o2;

            if (checkPoints.Count() > 0 && checkpoint == checkPoints[nextCheckPoint])
            {
                checkpoint.passThrough();
                if (checkpoint.state != GameObject.State.ERASE)
                    ++nextCheckPoint;

                // If we reach the end, we start again
                if (nextCheckPoint == checkPoints.Count)
                {
                    nextCheckPoint = 0;
                    ++lapsDone;
                }
            }
        }

        public override void keyPressed(Keys key)
        {
            if (key == Keys.Escape)
            {
                // Stop engine
                car.engine.Stop(AudioStopOptions.Immediate);

                // Change state (to menu)
                game.changeState(Type.MENU);
            }
        }

        public override void keyReleased(Keys key)
        {

        }   
    }
}

