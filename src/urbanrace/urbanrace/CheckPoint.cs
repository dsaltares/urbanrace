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
 * @file Checkpoint.cs
 * @description Checkpoint implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace urbanrace
{
    class CheckPoint: GameObject, IComparable<CheckPoint>
    {
        public int number { get; protected set; }
        public int lapCounter { get; protected set; }
        
        public CheckPoint(UrbanRace game, Vector3 position, Quaternion orientation, int number, int laps)
            : base(game, "checkpoint", position, orientation, 1.0f)
        {
            this.type = Type.CHECKPOINT;
            this.state = State.NORMAL;
            this.number = number;
            this.lapCounter = laps;
        }

        public int CompareTo(CheckPoint other)
        {
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;

            // The comparison depends on the number order
            return number.CompareTo(other.number);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
        }

        public void passThrough()
        {
            --lapCounter;

            Log.log(Log.Type.INFO, "Checkpoint reached, " + lapCounter + " laps to go");

            if (lapCounter == 0)
                state = State.ERASE;
        }
    }
}
