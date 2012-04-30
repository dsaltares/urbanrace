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
 * @file Skybox.cs
 * @description Skybox implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace urbanrace
{
    class SkyBox
    {
        protected UrbanRace game;
        protected Vector3 position;
        protected Quaternion orientation;
        float scale;
        protected VertexPositionTexture[][] verts;
        protected Texture2D[] textures;
        protected VertexBuffer[] vertexBuffers;
        protected BasicEffect effect;

        public SkyBox(UrbanRace game, Vector3 position, Quaternion orientation, float scale)
        {
            this.game = game;
            this.position = new Vector3(position.X, position.Y, position.Z);
            this.orientation = orientation;
            this.scale = scale;
            
            // Initialize vertices
            verts = new VertexPositionTexture[6][];
            for (int i = 0; i < 6; ++i)
                verts[i] = new VertexPositionTexture[4];

            verts[0][0] = new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(0, 0));
            verts[0][1] = new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(1, 0));
            verts[0][2] = new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(0, 1));
            verts[0][3] = new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(1, 1));

            verts[1][0] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(0, 0));
            verts[1][1] = new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(1, 0));
            verts[1][2] = new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(0, 1));
            verts[1][3] = new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(1, 1));

            verts[2][0] = new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(0, 0));
            verts[2][1] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 0));
            verts[2][2] = new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(0, 1));
            verts[2][3] = new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(1, 1));

            verts[3][0] = new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(0, 0));
            verts[3][1] = new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(1, 0));
            verts[3][2] = new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(0, 1));
            verts[3][3] = new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(1, 1));

            verts[4][0] = new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(0, 0));
            verts[4][1] = new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(1, 0));
            verts[4][2] = new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(0, 1));
            verts[4][3] = new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(1, 1));

            verts[5][0] = new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(0, 0));
            verts[5][1] = new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(1, 0));
            verts[5][2] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(0, 1));
            verts[5][3] = new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(1, 1));

            // Textures
            textures = new Texture2D[6];
            textures[0] = game.Content.Load<Texture2D>("Images\\back");
            textures[1] = game.Content.Load<Texture2D>("Images\\front");
            textures[2] = game.Content.Load<Texture2D>("Images\\right");
            textures[3] = game.Content.Load<Texture2D>("Images\\left");
            textures[4] = game.Content.Load<Texture2D>("Images\\bottom");
            textures[5] = game.Content.Load<Texture2D>("Images\\top");

            // Set vertex data in VertexBuffer
            vertexBuffers = new VertexBuffer[6];
            vertexBuffers[0] = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionTexture), verts[0].Length, BufferUsage.None);
            vertexBuffers[0].SetData(verts[0]);
            vertexBuffers[1] = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionTexture), verts[1].Length, BufferUsage.None);
            vertexBuffers[1].SetData(verts[1]);
            vertexBuffers[2] = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionTexture), verts[2].Length, BufferUsage.None);
            vertexBuffers[2].SetData(verts[2]);
            vertexBuffers[3] = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionTexture), verts[3].Length, BufferUsage.None);
            vertexBuffers[3].SetData(verts[3]);
            vertexBuffers[4] = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionTexture), verts[4].Length, BufferUsage.None);
            vertexBuffers[4].SetData(verts[4]);
            vertexBuffers[5] = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionTexture), verts[5].Length, BufferUsage.None);
            vertexBuffers[5].SetData(verts[5]);

            // Initialize the BasicEffect
            effect = new BasicEffect(game.GraphicsDevice);
        }

        public void draw()
        {
            //Set object and camera info
            effect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
            effect.View = game.camera.view;
            effect.Projection = game.camera.projection;
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            
            for (int i = 0; i < 6; ++i)
            {
                game.GraphicsDevice.SetVertexBuffer(vertexBuffers[i]);
                effect.Texture = textures[i];

                // Begin effect and draw for each pass
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    game.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                        (PrimitiveType.TriangleStrip, verts[i], 0, 2);
                }
            }
        }
    }
}
