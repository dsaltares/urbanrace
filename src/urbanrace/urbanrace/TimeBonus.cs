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
 * @file TimeBonus.cs
 * @description Time bonus implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace urbanrace
{
    class TimeBonus: GameObject
    {
        protected Vector3 velocity;
        protected float maxZ;
        protected float minZ;
        protected float rotation;

        public int seconds { get; protected set; }

        public TimeBonus(UrbanRace game, Vector3 position, Quaternion orientation, float scale, int seconds)
            : base(game, "timebonus", position, orientation, scale)
        {
            this.type = Type.TIMEBONUS;
            this.state = State.NORMAL;
            this.velocity = new Vector3(0.0f, 0.0f, 3.0f);
            this.maxZ = 2.5f;
            this.minZ = 1.0f;
            this.rotation = 1.0f;
            this.seconds = seconds;
        }

        public override void update(GameTime gameTime)
        {
            // Bounce and rotation effect
            position += velocity * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            orientation *= Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, rotation * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

            if (position.Z < minZ)
            {
                position.Z = minZ;
                velocity *= -1.0f;
            }
            else if (position.Z > maxZ)
            {
                position.Z = maxZ;
                velocity *= -1.0f;
            }

            // Rotation
            orientation *= Quaternion.CreateFromYawPitchRoll(rotation * gameTime.ElapsedGameTime.Seconds, 0.0f, 0.0f);

            base.update(gameTime);
        }
    }
}
