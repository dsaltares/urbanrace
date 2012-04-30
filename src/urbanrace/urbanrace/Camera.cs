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
 * @file Camera.cs
 * @description Camera implementation that follows the Car
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


namespace urbanrace
{
    public class Camera {

        protected UrbanRace game;

        public Matrix view { get; set; }
        public Matrix projection { get; protected set; }

        public float angle;
        public float rotation;
        public float maxRotation;
        public float maxAngularAcceleration;

        // Following properties
        public Vector3 position { get; protected set; }
        public float height { get; protected set; }
        public float distance { get; protected set; }
        public float aheadDistance { get; protected set; }
        
        
        public Camera(UrbanRace game)
        {
            // Reference to the car we follow
            this.game = game;

            
            // Movement properties
            this.height = Settings.getOpt("height");
            this.distance = Settings.getOpt("distance");
            this.aheadDistance = Settings.getOpt("aheadDistance");

            this.angle = 0.0f;
            this.rotation = 0.0f;
            this.maxRotation = Settings.getOpt("maxRotation");
            this.maxAngularAcceleration = Settings.getOpt("maxAngularAcceleration");

            // Initial position
            view = Matrix.CreateLookAt(new Vector3(1.0f, 1.0f, 1.0f),
                                                   new Vector3(0.0f, 0.0f, 0.0f),
                                                   Vector3.UnitY);

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                             (float)game.Window.ClientBounds.Width / (float)game.Window.ClientBounds.Height,
                                                             1,
                                                             1000);
        }

        public void setInitialPosition(Car car)
        {
            angle = car.angle;
            Vector3 translation = new Vector3((float)System.Math.Sin(angle), (float)System.Math.Cos(angle), 0.0f);
            translation *= distance;
            position = car.position - translation;

            game.camera.view = Matrix.CreateLookAt(new Vector3(position.X, height, -position.Y),
                                                   new Vector3(car.position.X, height, -car.position.Y),
                                                   Vector3.UnitY);
        }

        public void update(GameTime gameTime, Car car)
        {
            // Calculate target angular acceleration
            float angular = car.angle - angle;

            // Clamp angular acceleration
            if (Math.Abs(angular) > maxAngularAcceleration)
            {
                angular /= Math.Abs(angular);
                angular *= maxAngularAcceleration;
            }

            if (Math.Abs(angular) < 0.022f) 
            {
                rotation = 0.0f;
                angular = 0.0f;
                angle = car.angle;
            }

            // Add angular to rotation
            rotation += angular * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            // Limit rotation
            if (Math.Abs(rotation) > maxRotation)
            {
                rotation /= Math.Abs(rotation);
                rotation *= maxRotation;
            }

            // Add rotation to angle
            angle += rotation * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            Vector3 target = new Vector3((float)System.Math.Sin(-angle), (float)System.Math.Cos(-angle), 0.0f);
            target *= distance;
            target = car.position - target;
            position = target;

            // Look ahead
            Vector3 lookAt = new Vector3((float)System.Math.Sin(-car.angle), (float)System.Math.Cos(-car.angle), 0.0f);
            lookAt.Normalize();
            lookAt *= aheadDistance;
            lookAt += car.position;
            

            // Update camera view
            view = Matrix.CreateLookAt(new Vector3(position.X, height, -position.Y),
                                                   new Vector3(lookAt.X, height, -lookAt.Y),
                                                   Vector3.UnitY);
        }
    }
}
