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
 * @file Utility.cs
 * @description Utility auxiliar methods
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace urbanrace
{
    class Utility
    {
        public static Vector3 Multiply(Vector3 v, Matrix m)
        {
            return
                new Vector3(v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31 + m.M41,
                            v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32 + m.M42,
                            v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33 + m.M43);
        }

        public static Matrix3 Abs(Matrix3 m3)
        {
            Matrix3 absMatrix = new Matrix3(0);

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    absMatrix[r, c] = Math.Abs(m3[r, c]);
                }
            }

            return absMatrix;
        }

        public static string timeToString(int miliseconds)
        {
            int minutes = miliseconds / 1000 / 60;
            int seconds = (miliseconds - (minutes * 60000)) / 1000;
            int cent = (miliseconds - (minutes * 60000) - (seconds * 1000)) / 10;

            if (seconds < 0) seconds = 0;
            if (cent < 0) cent = 0;

            return String.Format("{0,2}", minutes) + ":" + String.Format("{0,2}", seconds) + ":" + String.Format("{0,2}", cent);
        }
    }
}
