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
 * @file Level.cs
 * @description Level implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Linq;

namespace urbanrace
{
    class Level
    {
        public string levelName { get; protected set; }
        public Vector3 carPosition { get; protected set; }
        public Quaternion carOrientation { get; protected set; }
        public Vector3 cameraPosition { get; protected set; }

        protected GameState gameState;

        public Level(GameState gameState, string levelName)
        {
            this.gameState = gameState;
            this.levelName = levelName;
            this.carPosition = Vector3.Zero;
            this.carOrientation = Quaternion.Identity;
            this.carPosition = Vector3.Zero;
        }

        public void load()
        {
            // Open xml file
            Log.log(Log.Type.INFO, "Loading level " + gameState.game.Content.RootDirectory + "\\XML\\" + levelName);
            XDocument doc = XDocument.Load(gameState.game.Content.RootDirectory + "\\XML\\" + levelName);

            foreach (XElement node in doc.Element("scene").Element("nodes").Descendants("node"))
            {
                string name = node.Attribute("name").Value;
                string[] nameParts = name.Split('.');

                XElement positionNode = node.Element("position");
                XElement quaternionNode = node.Element("quaternion");
                XElement scaleNode = node.Element("scale");

                // Get position
                Vector3 position = new Vector3((float)XmlConvert.ToDouble(positionNode.Attribute("x").Value),
                                               (float)XmlConvert.ToDouble(positionNode.Attribute("y").Value),
                                               (float)XmlConvert.ToDouble(positionNode.Attribute("z").Value));

                // Get scale
                float scale = (float)XmlConvert.ToDouble(scaleNode.Attribute("x").Value);

                // Get orientation
                Quaternion orientation = new Quaternion((float)XmlConvert.ToDouble(quaternionNode.Attribute("x").Value),
                                                        (float)XmlConvert.ToDouble(quaternionNode.Attribute("y").Value),
                                                        (float)XmlConvert.ToDouble(quaternionNode.Attribute("z").Value),
                                                        (float)XmlConvert.ToDouble(quaternionNode.Attribute("w").Value));


                if (nameParts.Length >= 2 && nameParts[0] == "scene")
                {
                    gameState.addGameObject(nameParts[1], position, orientation, scale);
                }
                if (nameParts.Length >= 2 && nameParts[0] == "geometry")
                {
                    gameState.addGeometry(nameParts[1], position, orientation, scale);
                }
                else if (nameParts.Length >= 2 && nameParts[0] == "time")
                {
                    gameState.addTimeBonus(position, orientation, scale, XmlConvert.ToInt32(nameParts[1]));
                }
                else if (nameParts.Length >= 2 && nameParts[0] == "checkpoint")
                {
                    gameState.addCheckPoint(position, orientation, XmlConvert.ToInt32(nameParts[1]));
                }
                else if (nameParts.Length == 1 && nameParts[0] == "car")
                {
                    carPosition = position;
                    carOrientation = orientation;
                }
                else if (nameParts.Length == 1 && nameParts[0] == "skybox")
                {
                    gameState.setSkyBox(position, orientation, scale);
                }
                else if (nameParts.Length == 2 && nameParts[0] == "terrain")
                {
                    gameState.setTerrain(nameParts[1], position, orientation, scale);
                }
                else if (nameParts.Length == 1 && nameParts[0] == "Camera")
                {
                    cameraPosition = position;
                }
            }
        }
    }
}
