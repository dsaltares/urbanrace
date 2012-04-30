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
 * @file Car.cs
 * @description Car implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace urbanrace
{
    public class Car: GameObject
    {
        public Cue engine { get; set; } 
        
        // Restore position
        public Vector3 oldPos { get; set; }
        public float oldAngle { get; set; }
        
        // Linear physics
        public Vector3 velocity { get; set; }
        public float maxSpeed {get; protected set; }
        public float maxAcceleration { get; protected set; }
        protected float brake;

        // Angular physics
        public float angle { get; protected set; }
        public float rotation { get; set; }
        public float angular { get; set; }
        public float maxAngularAcceleration { get; protected set; }

        protected bool visible;

        // Environment
        protected float friction;     
        
        public Car(UrbanRace game, Vector3 position, Quaternion orientation)
            : base(game, "Lamborghini", position, orientation, 1.0f)
        {
            this.type = Type.CAR;
            this.state = State.NORMAL;

            this.oldPos = this.position;
            this.oldAngle = 0.0f;

            // Physics properties
            this.velocity = Vector3.Zero;
            this.maxSpeed = Settings.getOpt("carMaxSpeed");
            this.maxAcceleration = Settings.getOpt("carMaxAcceleration");
            this.brake = Settings.getOpt("carBrake");
            this.rotation = 0.0f;
            this.angular = 0.0f;
            this.maxAngularAcceleration = Settings.getOpt("carMaxAngularAcceleration");
            this.friction = Settings.getOpt("carFriction");
            this.visible = false;
            this.angle = QuaternionToEuler2(orientation).Z;

            // Sound
            this.engine = game.soundBank.GetCue("engine");
            this.engine.Play();
            game.audioEngine.SetGlobalVariable("engineVolume", 0.0f);
            game.audioEngine.SetGlobalVariable("idleVolume", 1.0f);
        }

        public override void update(GameTime gameTime)
        {
            // Update old position and angle
            oldPos = position;
            oldAngle = angle;
            
            Vector3 linear = Vector3.Zero;

            // Calculate forces
            
            // Friction
            Vector3 frictionForce = new Vector3();
            
            if (velocity != Vector3.Zero)
            {
                frictionForce = -velocity;
                frictionForce.Normalize();
                frictionForce *= friction;
            }

            // Input
            KeyboardState keyboard = Input.getKeyboard();

            if (keyboard.IsKeyDown(Keys.Up))
            {
                linear = new Vector3((float)System.Math.Sin(-angle), (float)System.Math.Cos(-angle), 0.0f);
                linear *= maxAcceleration;
            }

            else if (keyboard.IsKeyDown(Keys.Down))
            {
                linear = -new Vector3((float)System.Math.Sin(-angle), (float)System.Math.Cos(-angle), 0.0f);
                linear *= brake;
            }

            if (keyboard.IsKeyDown(Keys.Left) && linear != Vector3.Zero)
                angular -= maxAngularAcceleration;

            else if (keyboard.IsKeyDown(Keys.Right) && linear != Vector3.Zero)
                angular += maxAngularAcceleration;

            if (keyboard.IsKeyDown(Keys.Down))
                angular *= -1;

            // Integrate physics
            linear += frictionForce;

            // Update velocity and angle
            velocity += limitVector(linear, maxAcceleration);
            rotation = System.Math.Min(angular, maxAngularAcceleration);

            // Limit velocity
            velocity = limitVector(velocity, maxSpeed);

            // Update position
            position += velocity * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            angle -= rotation * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            // Correct graphical rotation
            orientation = Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, angle);

            // Reset angular
            angular = 0.0f;

            // Update sound effect according to car movement
            if (velocity.LengthSquared() > 1.0f)
            {
                game.audioEngine.SetGlobalVariable("engineVolume", 1.0f);
                game.audioEngine.SetGlobalVariable("idleVolume", 0.0f);
                game.audioEngine.SetGlobalVariable("enginePitch", velocity.Length() / maxSpeed);
            }
            else
            {
                game.audioEngine.SetGlobalVariable("engineVolume", 0.0f);
                game.audioEngine.SetGlobalVariable("idleVolume", 1.0f);
            }

            base.update(gameTime);
        }


        protected Vector3 limitVector(Vector3 v, float max)
        {
            if (v.LengthSquared() > max * max)
            {
                v.Normalize();
                v *= max;
            }

            return v;
        }

        public Vector3 QuaternionToEuler2(Quaternion q)
        {
            Vector3 euler = new Vector3();

            float sqx = q.X * q.X;
            float sqy = q.Y * q.Y;
            float sqz = q.Z * q.Z;
            float sqw = q.W * q.W;

            float unit = sqx + sqy + sqz + sqw;
            float test = (q.X * q.W - q.Y * q.Z);

            // Handle singularity
            if (test > 0.4999999f * unit)
            {
                euler.X = MathHelper.PiOver2;
                euler.Y = 2.0f * (float)System.Math.Atan2(q.Y, q.W);
                euler.Z = 0;
            }
            else if (test < -0.4999999f * unit)
            {
                euler.X = -MathHelper.PiOver2;
                euler.Y = 2.0f * (float)System.Math.Atan2(q.Y, q.W);
                euler.Z = 0;
            }
            else
            {
                float ey_Y = 2 * (q.X * q.Z + q.Y * q.W);
                float ey_X = 1 - 2 * (sqy + sqx);
                float ez_Y = 2 * (q.X * q.Y + q.Z * q.W);
                float ez_X = 1 - 2 * (sqx + sqz);
                euler.X = (float)System.Math.Asin(2 * test);
                euler.Y = (float)System.Math.Atan2(ey_Y, ey_X);
                euler.Z = (float)System.Math.Atan2(ez_Y, ez_X);
            }

            return euler;
        }
    }
}

