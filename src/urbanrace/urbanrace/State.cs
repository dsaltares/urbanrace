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
 * @file State.cs
 * @description Abstract state interface
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace urbanrace
{
    public abstract class State
    {
        public enum Type { MENU, GAME, VICTORY, DEFEAT };

        public UrbanRace game { get; protected set; }
        public bool loaded { get; protected set; }
        public Type type { get; protected set; }

        public State(UrbanRace game)
        {
            this.game = game;
            this.loaded = false;
        }
        
        public abstract void update(GameTime gameTime);
        public abstract void draw(GameTime gameTime);
        public abstract void load();
        public abstract void unload();

        public virtual void keyPressed(Keys key) { }
        public virtual void keyReleased(Keys key) { }
    }
}
