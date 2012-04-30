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
 * @file Terrain.cs
 * @description Terrain implementation (bad performance as of now)
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Xml.Linq;

namespace urbanrace
{
    class Terrain
    {
        protected UrbanRace game;
        protected string file;
        protected Vector3 position;
        protected Quaternion orientation;
        protected float scale;
        protected VertexPositionNormalTexture[] vertices;
        protected short[] indices;
        protected VertexBuffer myVertexBuffer;
        protected IndexBuffer myIndexBuffer;
        protected Vector2 min;
        protected Vector2 max;
        protected Texture2D terrainTexture;
        
        public Terrain(UrbanRace game, string file, Vector3 position, Quaternion orientation, float scale)
        {
            this.game = game;
            this.file = file;
            this.position = new Vector3(position.X, position.Z, position.Y);
            this.orientation = orientation;
            this.scale = scale;
            this.vertices = null;
            this.indices = null;
            this.min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            this.max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

            parseFile();
            calculateNormals();
            setupTexture();
            copyToBuffers();
        }

        public void draw()
        {
            BasicEffect effect = new BasicEffect(game.GraphicsDevice);
            
            effect.TextureEnabled = true;
            effect.Texture = terrainTexture;
            effect.EnableDefaultLighting();

            effect.World = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(orientation) * Matrix.CreateTranslation(position);
            effect.View = game.camera.view;
            effect.Projection = game.camera.projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
               
                game.GraphicsDevice.Indices = myIndexBuffer;
                game.GraphicsDevice.SetVertexBuffer(myVertexBuffer);
                game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);                
            }
        }

        protected void parseFile()
        {
            XDocument doc = XDocument.Load(game.Content.RootDirectory + "\\XML\\" + file + ".mesh.xml");

            XElement facesNode = doc.Element("mesh").Element("submeshes").Element("submesh").Element("faces");
            int numFaces = XmlConvert.ToInt32(facesNode.Attribute("count").Value);

            indices = new short[numFaces * 3];

            IList<XElement> faces = facesNode.Elements().ToList();

            for (int i = 0; i < numFaces * 3; i += 3)
            {
                indices[i] = XmlConvert.ToInt16(faces[i/3].Attribute("v1").Value);
                indices[i + 1] = XmlConvert.ToInt16(faces[i / 3].Attribute("v2").Value);
                indices[i + 2] = XmlConvert.ToInt16(faces[i / 3].Attribute("v3").Value);
            }

            // Vertices nodes
            XElement verticesNode = doc.Element("mesh").Element("submeshes").Element("submesh").Element("geometry").Element("vertexbuffer");

            IList<XElement> verticesList = verticesNode.Elements().ToList();
            int numVertices = verticesList.Count;
            vertices = new VertexPositionNormalTexture[numVertices];

            for (int i = 0; i < numVertices; ++i)
            {
                XElement positionNode = verticesList[i].Element("position");
                vertices[i].Position = new Vector3((float)XmlConvert.ToDouble(positionNode.Attribute("x").Value),
                                                   (float)XmlConvert.ToDouble(positionNode.Attribute("z").Value),
                                                   (float)XmlConvert.ToDouble(positionNode.Attribute("y").Value));
            }
        }

        protected void calculateNormals()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indices.Length; i += 3)
            {
                int index1 = indices[i];
                int index2 = indices[i + 1];
                int index3 = indices[i + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
        }

        protected void copyToBuffers()
        {
            myVertexBuffer = new VertexBuffer(game.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            myVertexBuffer.SetData(vertices);

            myIndexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            myIndexBuffer.SetData(indices);
        }

        protected void setupTexture()
        {            
            // Get min and max coordinates
            for (int i = 0; i < vertices.Length; ++i)
            {
                if (vertices[i].Position.X < min.X)
                    min.X = vertices[i].Position.X;
                else if (vertices[i].Position.X > max.X)
                    max.X = vertices[i].Position.X;

                if (vertices[i].Position.Z < min.Y)
                    min.Y = vertices[i].Position.Z;
                else if (vertices[i].Position.Z > max.Y)
                    max.Y = vertices[i].Position.Z;
            }

            Log.log(Log.Type.INFO, "Terrain max: " + max.ToString());
            Log.log(Log.Type.INFO, "Terrain min: " + min.ToString());

            // Calculate terrain width and height
            float width = Math.Abs(max.X - min.X);
            float height = Math.Abs(max.Y - min.Y);

            Log.log(Log.Type.INFO, "Terrain size: " + new Vector2(width, height).ToString());

            // Load texture
            terrainTexture = game.Content.Load<Texture2D>("Images\\" + file);

            // Asign texture coordinates 
            for (int i = 0; i < vertices.Length; ++i)
            {
                vertices[i].TextureCoordinate = new Vector2((vertices[i].Position.X + width / 2.0f) / width, (vertices[i].Position.Y + height / 2)/ height);
                //Log.log(Log.Type.INFO, "Text coord: (" + (vertices[i].Position.X + width / 2.0f) / width + ", " + (vertices[i].Position.Y + height / 2) / height + ")");

                if (vertices[i].TextureCoordinate.X < 0.0f || vertices[i].TextureCoordinate.X > 1.0f ||
                    vertices[i].TextureCoordinate.Y < 0.0f || vertices[i].TextureCoordinate.Y > 1.0f)
                    Log.log(Log.Type.WARNING, "Wrong UV: (" + vertices[i].TextureCoordinate.X + ", " + vertices[i].TextureCoordinate.Y + ")");
            }
        }
    }
}
