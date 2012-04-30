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
 * @file Matrix3.cs
 * @description 3x3 matrix implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace urbanrace
{
    struct Matrix3
    {
        public Matrix3(float f)
        {
            this.Elements = new float[3, 3];
        }

        public Matrix3(Matrix m)
        {
            this.Elements = new float[3, 3];

            this.Elements[0, 0] = m.M11;
            this.Elements[0, 1] = m.M12;
            this.Elements[0, 2] = m.M13;

            this.Elements[1, 0] = m.M21;
            this.Elements[1, 1] = m.M22;
            this.Elements[1, 2] = m.M23;

            this.Elements[2, 0] = m.M31;
            this.Elements[2, 1] = m.M32;
            this.Elements[2, 2] = m.M33;
        }

        public Matrix3(Matrix3 m)
        {
            this.Elements = new float[3, 3];

            this.Elements[0, 0] = m[0, 0];
            this.Elements[0, 1] = m[0, 1];
            this.Elements[0, 2] = m[0, 2];

            this.Elements[1, 0] = m[1, 0];
            this.Elements[1, 1] = m[1, 1];
            this.Elements[1, 2] = m[1, 2];

            this.Elements[2, 0] = m[2, 0];
            this.Elements[2, 1] = m[2, 1];
            this.Elements[2, 2] = m[2, 2];
        }

        public float this[int row, int column]
        {
            get
            {
                if (row > 2 || column > 2)
                    throw new IndexOutOfRangeException();

                return this.Elements[row, column];
            }
            set
            {
                if (row > 2 || column > 2)
                    throw new IndexOutOfRangeException();

                this.Elements[row, column] = value;
            }
        }


        public Vector3 Row(int row)
        {
            return new Vector3(this.Elements[row, 0],
                               this.Elements[row, 1],
                               this.Elements[row, 2]);
        }

        public Vector3 Column(int column)
        {
            return new Vector3(this.Elements[0, column],
                               this.Elements[1, column],
                               this.Elements[2, column]);
        }

        float[,] Elements;
    }
}
