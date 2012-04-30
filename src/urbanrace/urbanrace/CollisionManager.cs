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
 * @file CollisionManager.cs
 * @description Detects and provides collision callbacks
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using System.Xml.Linq;

namespace urbanrace
{
    class CollisionManager
    {
        public delegate void CollisionCallback(GameObject o1, GameObject o2);
        public enum CallbackType { BEGIN, DURING, END };

        protected List<GameObject> objects;
        protected List<KeyValuePair<GameObject, GameObject>> collidingObjects;
        protected Dictionary<KeyValuePair<GameObject.Type, GameObject.Type>, CollisionCallback> callbacksBegin;
        protected Dictionary<KeyValuePair<GameObject.Type, GameObject.Type>, CollisionCallback> callbacksDuring;
        protected Dictionary<KeyValuePair<GameObject.Type, GameObject.Type>, CollisionCallback> callbacksEnd;
        protected float checkRadius;
        protected static Dictionary<string, Shape> shapes;

        public CollisionManager()
        {
            this.objects = new List<GameObject>();
            this.collidingObjects = new List<KeyValuePair<GameObject, GameObject>>();
            this.callbacksBegin = new Dictionary<KeyValuePair<GameObject.Type, GameObject.Type>, CollisionCallback>();
            this.callbacksDuring = new Dictionary<KeyValuePair<GameObject.Type, GameObject.Type>, CollisionCallback>();
            this.callbacksEnd = new Dictionary<KeyValuePair<GameObject.Type, GameObject.Type>, CollisionCallback>();
            this.checkRadius = Settings.getOpt("colCheckRadius"); ;
        }

        public static void initShapeCatalog(string rootDir)
        {
            // Create object shapes
            shapes = new Dictionary<string, Shape>();
            
            // Load xml file
            XDocument doc = XDocument.Load(rootDir + "\\XML\\shapes.xml");

            foreach (XElement node in doc.Element("shapes").Descendants("shape"))
            {
                string name = node.Attribute("name").Value;
                string type = node.Attribute("type").Value;

                if (type == "aabb")
                {
                    Vector3 min = new Vector3((float)XmlConvert.ToDouble(node.Element("min").Attribute("x").Value),
                                              (float)XmlConvert.ToDouble(node.Element("min").Attribute("y").Value),
                                              (float)XmlConvert.ToDouble(node.Element("min").Attribute("z").Value));

                    Vector3 max = new Vector3((float)XmlConvert.ToDouble(node.Element("max").Attribute("x").Value),
                                              (float)XmlConvert.ToDouble(node.Element("max").Attribute("y").Value),
                                              (float)XmlConvert.ToDouble(node.Element("max").Attribute("z").Value));

                    shapes[name] = new AxisAlignedBox(min, max);
                }
                else if (type == "obb")
                {
                    Vector3 min = new Vector3((float)XmlConvert.ToDouble(node.Element("min").Attribute("x").Value),
                                              (float)XmlConvert.ToDouble(node.Element("min").Attribute("y").Value),
                                              (float)XmlConvert.ToDouble(node.Element("min").Attribute("z").Value));

                    Vector3 max = new Vector3((float)XmlConvert.ToDouble(node.Element("max").Attribute("x").Value),
                                              (float)XmlConvert.ToDouble(node.Element("max").Attribute("y").Value),
                                              (float)XmlConvert.ToDouble(node.Element("max").Attribute("z").Value));

                    shapes[name] = new OrientedBox(min, max);
                }
                else if (type == "sphere")
                {
                    Vector3 center = new Vector3((float)XmlConvert.ToDouble(node.Element("center").Attribute("x").Value),
                                                 (float)XmlConvert.ToDouble(node.Element("center").Attribute("y").Value),
                                                 (float)XmlConvert.ToDouble(node.Element("center").Attribute("z").Value));

                    float radius = (float)XmlConvert.ToDouble(node.Element("radius").Attribute("value").Value);

                    shapes[name] = new Sphere(center, radius);
                }
                else
                    Log.log(Log.Type.WARNING, "Unknown shape type-> name: " + name + " type: " + type);
            }
        }

        public static Shape getShape(string name)
        {
            Shape shape;
            
            if (shapes.TryGetValue(name, out shape))
                return shape.copy();
            else
                return null;
        }

        public void addObject(GameObject gameObject)
        {
            objects.Add(gameObject);
        }

        public void removeObject(GameObject gameObject)
        {
            objects.Remove(gameObject);
        }

        public void removeAllObjects()
        {
            objects.Clear();
        }

        public void checkCollisions()
        {
            // For every pair of objects (without checking them twice)
            for (int i = 0; i < objects.Count; ++i)
            {
                for (int j = i + 1; j < objects.Count; ++j)
                {
                    // Within radius
                    if ((objects[i].position - objects[j].position).LengthSquared() < checkRadius * checkRadius)
                    {
                        // Check callback existence
                        CollisionCallback callbackBegin;
                        CollisionCallback callbackDuring;
                        CollisionCallback callbackEnd;
                        bool begin = callbacksBegin.TryGetValue(new KeyValuePair<GameObject.Type, GameObject.Type>(objects[i].type, objects[j].type), out callbackBegin);
                        bool during = callbacksDuring.TryGetValue(new KeyValuePair<GameObject.Type, GameObject.Type>(objects[i].type, objects[j].type), out callbackDuring);
                        bool end = callbacksEnd.TryGetValue(new KeyValuePair<GameObject.Type, GameObject.Type>(objects[i].type, objects[j].type), out callbackEnd);
                    
                        // If any callback
                        if (begin || during || end)
                        {
                            // If were colliding
                            if (wereColliding(objects[i], objects[j]))
                            {
                                // If they collide
                                if (GameObject.getCollision(objects[i], objects[j]))
                                {
                                    // If there is during callback
                                    if (during)
                                        callbackDuring(objects[i], objects[j]);
                                }
                                // If they dont collid
                                else
                                {
                                    // Remove from colliding
                                    collidingObjects.Remove(new KeyValuePair<GameObject, GameObject>(objects[i], objects[j]));
                                    collidingObjects.Remove(new KeyValuePair<GameObject, GameObject>(objects[j], objects[i]));

                                    // If there is end callback
                                    if (end)
                                        callbackEnd(objects[i], objects[j]);
                                }
                            }
                            // if they werent colliding and they collide
                            else if (GameObject.getCollision(objects[i], objects[j]))
                            {
                                // Add to colliding objects
                                collidingObjects.Add(new KeyValuePair<GameObject, GameObject>(objects[i], objects[j]));
                                collidingObjects.Add(new KeyValuePair<GameObject, GameObject>(objects[j], objects[i]));

                                // If begin callback
                                if (begin)
                                    callbackBegin(objects[i], objects[j]);

                                // If during callback
                                if (during)
                                    callbackDuring(objects[i], objects[j]);
                            }
                        }
                    }
                }   
            }
        }

        protected bool wereColliding(GameObject o1, GameObject o2)
        {
            return collidingObjects.Contains(new KeyValuePair<GameObject, GameObject>(o1, o2)) ||
                   collidingObjects.Contains(new KeyValuePair<GameObject, GameObject>(o2, o1));
        }

        public void addCallback(GameObject.Type typeA, GameObject.Type typeB, CollisionCallback callback, CallbackType callbackType)
        {
            switch (callbackType)
            {
                case CallbackType.BEGIN:
                    callbacksBegin[new KeyValuePair<GameObject.Type, GameObject.Type>(typeA, typeB)] = callback;
                    callbacksBegin[new KeyValuePair<GameObject.Type, GameObject.Type>(typeB, typeA)] = callback;
                    break;
                case CallbackType.DURING:
                    callbacksDuring[new KeyValuePair<GameObject.Type, GameObject.Type>(typeA, typeB)] = callback;
                    callbacksDuring[new KeyValuePair<GameObject.Type, GameObject.Type>(typeB, typeA)] = callback;
                    break;
                case CallbackType.END:
                    callbacksEnd[new KeyValuePair<GameObject.Type, GameObject.Type>(typeA, typeB)] = callback;
                    callbacksEnd[new KeyValuePair<GameObject.Type, GameObject.Type>(typeB, typeA)] = callback;
                    break;
            } 
        }

        public void removeAllCollisionCallbacks()
        {
            callbacksBegin.Clear();
            callbacksDuring.Clear();
            callbacksEnd.Clear();
        }

        public void removeCallback(GameObject.Type typeA, GameObject.Type typeB, CallbackType callbackType)
        {
            switch (callbackType)
            {
                case CallbackType.BEGIN:
                    callbacksBegin.Remove(new KeyValuePair<GameObject.Type, GameObject.Type>(typeA, typeB));
                    callbacksBegin.Remove(new KeyValuePair<GameObject.Type, GameObject.Type>(typeB, typeA));
                    break;
                case CallbackType.DURING:
                    callbacksDuring.Remove(new KeyValuePair<GameObject.Type, GameObject.Type>(typeA, typeB));
                    callbacksDuring.Remove(new KeyValuePair<GameObject.Type, GameObject.Type>(typeB, typeA));
                    break;
                case CallbackType.END:
                    callbacksEnd.Remove(new KeyValuePair<GameObject.Type, GameObject.Type>(typeA, typeB));
                    callbacksEnd.Remove(new KeyValuePair<GameObject.Type, GameObject.Type>(typeB, typeA));
                    break;
            }
        }


        // Closest point tests
        public static Vector3 closestPointToOBB(Vector3 p, OrientedBox obb)
        {
            Vector3 q = new Vector3();
            float v;
            
            // Transform point into OBB local coordinates so we can treat OBB as an AABB
            Vector3 transformedP = Utility.Multiply(p, Matrix.Invert(obb.transform));

            // Closest point on AABB to point
            v = transformedP.X;
            if (v < obb.min.X) v = obb.min.X;
            if (v > obb.max.X) v = obb.max.X;
            q.X = v;

            v = transformedP.Y;
            if (v < obb.min.Y) v = obb.min.Y;
            if (v > obb.max.Y) v = obb.max.Y;
            q.Y = v;

            v = transformedP.Z;
            if (v < obb.min.Z) v = obb.min.Z;
            if (v > obb.max.Z) v = obb.max.Z;
            q.Z = v;

            // Transform closest point in world coordinates
            q = Utility.Multiply(q, obb.transform);

            return q;
        }
    }
}
