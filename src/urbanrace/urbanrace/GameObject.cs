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
 * @file GameObject.cs
 * @description Game objects implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace urbanrace
{
    public class GameObject
    {
        public enum Type { BASIC, CAR, SCENEOBJECT, CHECKPOINT, TIMEBONUS };
        public enum State { NORMAL, ERASE };
        
        protected UrbanRace game;
        public Vector3 position;
        public Quaternion orientation;
        public float scale;

        public Model model {get; protected set;}
        public Shape shape { get; protected set; }
        public Type type { get; set; }
        public State state { get; set; }
        

        public GameObject(UrbanRace game, String modelName, Vector3 position, Quaternion orientation, float scale)
        {   
            this.game = game;
            this.model = game.Content.Load<Model>("Models\\" + modelName);
            this.position = position;
            this.orientation = orientation;
            this.scale = scale;
            this.type = Type.BASIC;
            this.state = State.NORMAL;
            this.shape = CollisionManager.getShape(modelName);
        }

        public virtual void update(GameTime gameTime)
        {
            // Upate shape's transform
            if (shape != null)
                shape.transform = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(orientation) * Matrix.CreateTranslation(position);
        }

        public virtual void draw()
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = game.camera.projection;
                    be.View = game.camera.view;
                    be.World = Matrix.CreateScale(scale) *
                               Matrix.CreateFromQuaternion(orientation)*
                               Matrix.CreateTranslation(position) *
                               mesh.ParentBone.Transform;
                }

                mesh.Draw();
            }
        }

        public static bool getCollision(GameObject o1, GameObject o2)
        {
            if (o1.shape == null || o2.shape == null)
            {
                //Log.log(Log.Type.WARNING, "Trying to check collision between objects with no shape");
                return false;
            }
            else
            {
                return Shape.getCollision(o1.shape, o2.shape);
            }
        }
    }
}
