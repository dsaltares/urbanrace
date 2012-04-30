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
 * @file Shape.cs
 * @description Shape implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace urbanrace
{
    public abstract class Shape
    {
        public enum Type {Sphere = 0, AABB = 1, OBB = 2};
        public delegate bool CollisionTest (Shape s1, Shape s2);

        public Type type { get; protected set; }
        public Matrix transform { get; set; }
        public static int numTypes { get; protected set; }
        protected static Dictionary<KeyValuePair<Type, Type>, CollisionTest> collisionTests;

        public Shape()
        {
            this.transform = new Matrix();
        }

        public abstract Shape copy();

        public static void initialiseCollisionTests()
        {
            // Create and initialise collision test table
            collisionTests = new Dictionary<KeyValuePair<Type, Type>, CollisionTest>();

            // Populate table
            collisionTests[new KeyValuePair<Type, Type>(Type.Sphere, Type.Sphere)] = new CollisionTest(getSphereSphereCollision);
            collisionTests[new KeyValuePair<Type, Type>(Type.Sphere, Type.AABB)] = new CollisionTest(getSphereAABBCollision);
            collisionTests[new KeyValuePair<Type, Type>(Type.Sphere, Type.OBB)] = new CollisionTest(getSphereOBBCollision);
            collisionTests[new KeyValuePair<Type, Type>(Type.AABB, Type.Sphere)] = new CollisionTest(getSphereAABBCollision);
            collisionTests[new KeyValuePair<Type, Type>(Type.AABB, Type.AABB)] = new CollisionTest(getAABBAABBCollision);
            collisionTests[new KeyValuePair<Type, Type>(Type.AABB, Type.OBB)] = new CollisionTest(getAABBOBBCollision);
            collisionTests[new KeyValuePair<Type, Type>(Type.OBB, Type.Sphere)] = new CollisionTest(getSphereOBBCollision);
            collisionTests[new KeyValuePair<Type, Type>(Type.OBB, Type.AABB)] = new CollisionTest(getAABBOBBCollision);
            collisionTests[new KeyValuePair<Type, Type>(Type.OBB, Type.OBB)] = new CollisionTest(getOBBOBBCollision);
        }

        public static void addCollisionTest(Shape.Type t1, Shape.Type t2, CollisionTest test)
        {
            collisionTests[new KeyValuePair<Type, Type>(t1, t2)] = new CollisionTest(test);
        }

        public static bool getCollision(Shape s1, Shape s2)
        {
            KeyValuePair<Shape.Type, Shape.Type> key = new KeyValuePair<Shape.Type, Shape.Type>(s1.type, s2.type);
            CollisionTest collisionTest;

            if (collisionTests.TryGetValue(key, out collisionTest))
                return collisionTest(s1, s2);
            else
                return false;
        }

        public static bool getSphereSphereCollision(Shape s1, Shape s2)
        {
            // Safe type conversion
            Sphere sphere1 = (Sphere)s1;
            Sphere sphere2 = (Sphere)s2;

            // Collision test
            return (sphere1.center - sphere2.center).LengthSquared() <= (sphere1.radius + sphere2.radius) * (sphere1.radius + sphere2.radius);
        }

        public static bool getSphereAABBCollision(Shape s1, Shape s2)
        {
            Sphere sphere;
            AxisAlignedBox aabb;
            
            // Safe type conversion
            if (s1.type == Shape.Type.Sphere)
            {
                sphere = (Sphere)s1;
                aabb = (AxisAlignedBox)s2;
            }
            else
            {
                sphere = (Sphere)s2;
                aabb = (AxisAlignedBox)s1;
            }
            
            // Test
            float s = 0.0f;
            float d = 0.0f;

            // Check if the sphere is inside the AABB
            bool centerInsideAABB = (sphere.center.X <= aabb.max.X && 
                                     sphere.center.X <= aabb.min.X && 
                                     sphere.center.Y <= aabb.max.Y && 
                                     sphere.center.Y <= aabb.min.Y && 
                                     sphere.center.Z <= aabb.max.Z && 
                                     sphere.center.Z <= aabb.min.Z);

            if (centerInsideAABB)
            {
                return true;
            }

            // Check if the sphere and the AABB intersect
            if (sphere.center.X < aabb.min.X) {
                s = sphere.center.X - aabb.min.X;
                d += s * s;
            }
            else if (sphere.center.X > aabb.max.X) {
                s = sphere.center.X - aabb.max.X;
                d += s * s;
            }

            if (sphere.center.Y < aabb.min.Y) {
                s = sphere.center.Y - aabb.min.Y;
                d += s * s;
            }
            else if (sphere.center.Y > aabb.max.Y) {
                s = sphere.center.Y - aabb.max.Y;
                d += s * s;
            }

            if (sphere.center.Z < aabb.min.Z) {
                s = sphere.center.Z - aabb.min.Z;
                d += s * s;
            }
            else if (sphere.center.Z > aabb.max.Z) {
                s = sphere.center.Z - aabb.max.Z;
                d += s * s;
            }

            return d <= sphere.radius * sphere.radius;
        }

        public static bool getSphereOBBCollision(Shape s1, Shape s2)
        {
            return false;
        }

        public static bool getAABBAABBCollision(Shape s1, Shape s2)
        {
            // Safe type conversion
            AxisAlignedBox aabb1 = (AxisAlignedBox)s1;
            AxisAlignedBox aabb2 = (AxisAlignedBox)s2;

            // Collision test (axis separating theorem)
            return aabb1.max.X > aabb2.min.X &&
                   aabb1.min.X < aabb2.max.X &&
                   aabb1.max.Y > aabb2.min.Y &&
                   aabb1.min.Y < aabb2.max.Y &&
                   aabb1.max.Z > aabb2.min.Z &&
                   aabb1.min.Z < aabb2.max.Z;
        }

        public static bool getAABBOBBCollision(Shape s1, Shape s2)
        {
            AxisAlignedBox aabb;
            OrientedBox obb;
            
            // Safe type conversion
            if (s1.type == Shape.Type.AABB)
            {
                aabb = (AxisAlignedBox)s1;
                obb = (OrientedBox)s2;
            }
            else
            {
                aabb = (AxisAlignedBox)s2;
                obb = (OrientedBox)s1;
            }

            OrientedBox o2 = new OrientedBox(aabb.min, aabb.max);

            return getOBBOBBCollision(obb, o2);
        }

        public static bool getOBBOBBCollision(Shape s1, Shape s2)
        {
            OrientedBox o1 = (OrientedBox)s1;
            OrientedBox o2 = (OrientedBox)s2;

            // Matrix to transform other OBB into my reference to allow me to be treated as an AABB
            Matrix toMe = o2.transform * Matrix.Invert(o1.transform);

            Vector3 centerOther = Utility.Multiply(o2.center, toMe);
            Vector3 extentsOther = o2.extent;
            Vector3 separation = centerOther - o1.center;

            Matrix3 rotations = new Matrix3(toMe);
            Matrix3 absRotations = Utility.Abs(rotations);

            float r, r0, r1, r01;

            //--- Test case 1 - X axis

            r = Math.Abs(separation.X);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(0));
            r01 = o1.extent.X + r1;

            if (r > r01) return false;

            //--- Test case 1 - Y axis

            r = Math.Abs(separation.Y);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(1));
            r01 = o1.extent.Y + r1;

            if (r > r01) return false;

            //--- Test case 1 - Z axis

            r = Math.Abs(separation.Z);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(2));
            r01 = o1.extent.Z + r1;

            if (r > r01) return false;

            //--- Test case 2 - X axis

            r = Math.Abs(Vector3.Dot(rotations.Row(0), separation));
            r0 = Vector3.Dot(o1.extent, absRotations.Row(0));
            r01 = r0 + extentsOther.X;

            if (r > r01) return false;

            //--- Test case 2 - Y axis

            r = Math.Abs(Vector3.Dot(rotations.Row(1), separation));
            r0 = Vector3.Dot(o1.extent, absRotations.Row(1));
            r01 = r0 + extentsOther.Y;

            if (r > r01) return false;

            //--- Test case 2 - Z axis

            r = Math.Abs(Vector3.Dot(rotations.Row(2), separation));
            r0 = Vector3.Dot(o1.extent, absRotations.Row(2));
            r01 = r0 + extentsOther.Z;

            if (r > r01) return false;

            //--- Test case 3 # 1

            r = Math.Abs(separation.Z * rotations[0, 1] - separation.Y * rotations[0, 2]);
            r0 = o1.extent.Y * absRotations[0, 2] + o1.extent.Z * absRotations[0, 1];
            r1 = extentsOther.Y * absRotations[2, 0] + extentsOther.Z * absRotations[1, 0];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 2

            r = Math.Abs(separation.Z * rotations[1, 1] - separation.Y * rotations[1, 2]);
            r0 = o1.extent.Y * absRotations[1, 2] + o1.extent.Z * absRotations[1, 1];
            r1 = extentsOther.X * absRotations[2, 0] + extentsOther.Z * absRotations[0, 0];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 3

            r = Math.Abs(separation.Z * rotations[2, 1] - separation.Y * rotations[2, 2]);
            r0 = o1.extent.Y * absRotations[2, 2] + o1.extent.Z * absRotations[2, 1];
            r1 = extentsOther.X * absRotations[1, 0] + extentsOther.Y * absRotations[0, 0];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 4

            r = Math.Abs(separation.X * rotations[0, 2] - separation.Z * rotations[0, 0]);
            r0 = o1.extent.X * absRotations[0, 2] + o1.extent.Z * absRotations[0, 0];
            r1 = extentsOther.Y * absRotations[2, 1] + extentsOther.Z * absRotations[1, 1];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 5

            r = Math.Abs(separation.X * rotations[1, 2] - separation.Z * rotations[1, 0]);
            r0 = o1.extent.X * absRotations[1, 2] + o1.extent.Z * absRotations[1, 0];
            r1 = extentsOther.X * absRotations[2, 1] + extentsOther.Z * absRotations[0, 1];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 6

            r = Math.Abs(separation.X * rotations[2, 2] - separation.Z * rotations[2, 0]);
            r0 = o1.extent.X * absRotations[2, 2] + o1.extent.Z * absRotations[2, 0];
            r1 = extentsOther.X * absRotations[1, 1] + extentsOther.Y * absRotations[0, 1];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 7

            r = Math.Abs(separation.Y * rotations[0, 0] - separation.X * rotations[0, 1]);
            r0 = o1.extent.X * absRotations[0, 1] + o1.extent.Y * absRotations[0, 0];
            r1 = extentsOther.Y * absRotations[2, 2] + extentsOther.Z * absRotations[1, 2];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 8

            r = Math.Abs(separation.Y * rotations[1, 0] - separation.X * rotations[1, 1]);
            r0 = o1.extent.X * absRotations[1, 1] + o1.extent.Y * absRotations[1, 0];
            r1 = extentsOther.X * absRotations[2, 2] + extentsOther.Z * absRotations[0, 2];
            r01 = r0 + r1;

            if (r > r01) return false;

            //--- Test case 3 # 9

            r = Math.Abs(separation.Y * rotations[2, 0] - separation.X * rotations[2, 1]);
            r0 = o1.extent.X * absRotations[2, 1] + o1.extent.Y * absRotations[2, 0];
            r1 = extentsOther.X * absRotations[1, 2] + extentsOther.Y * absRotations[0, 2];
            r01 = r0 + r1;

            if (r > r01) return false;

            return true;  // No separating axis, then we have intersection
        }
    }

    class Sphere: Shape
    {
        public Vector3 center { get; set; }
        public float radius { get; set; }

        public Sphere(Vector3 center, float radius): base()
        {
            this.center = center;
            this.radius = radius;
            this.type = Shape.Type.Sphere;
        }

        public Sphere(Sphere s): base()
        {
            center = s.center;
            radius = s.radius;
            type = s.type;
            transform = s.transform;
        }

        public override Shape copy()
        {
            return new Sphere(this);
        }
    }

    class AxisAlignedBox : Shape
    {
        public Vector3 min { get; set; }
        public Vector3 max { get; set; }

        public AxisAlignedBox(Vector3 min, Vector3 max): base()
        {
            this.min = min;
            this.max = max;
            this.type = Shape.Type.AABB;
        }

        public AxisAlignedBox(AxisAlignedBox a): base()
        {
            min = a.min;
            max = a.max;
            type = a.type;
            transform = a.transform;
        }

        public override Shape copy()
        {
            return new AxisAlignedBox(this);
        }
    }

    class OrientedBox : Shape
    {
        protected Vector3 _min;
        protected Vector3 _max;

        public Vector3 min
        {
            get { return _min; }
            set { _min = value; updateFromMinMax(); }
        }

        public Vector3 max
        {
            get { return _max; }
            set { _max = value; updateFromMinMax(); }
        }

        public Vector3 center { get; protected set; }
        public Vector3 extent { get; protected set; }

        public OrientedBox(Vector3 min, Vector3 max): base()
        {
            this.type = Type.OBB;
            this._min = min;
            this._max = max;

            updateFromMinMax();
        }

        public OrientedBox(OrientedBox o): base()
        {
            this.type = Type.OBB;
            center = o.center;
            extent = o.extent;
            transform = o.transform;
            min = o.min;
            max = o.max;
        }

        public override Shape copy()
        {
            return new OrientedBox(this);
        }

        protected void updateFromMinMax()
        {
            center = (min + max) * 0.5f;
            extent = (max - min) * 0.5f;
        }
    }
}
