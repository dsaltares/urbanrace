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
 * @file Log.cs
 * @description Log implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace urbanrace
{
    public static class Log
    {
        public enum Type { ERROR = 0, WARNING = 1, INFO = 2 };

        static public bool active { get; set; }

        public static void init()
        {
            active = true;

#if WINDOWS
            StreamWriter stream = new StreamWriter(new FileStream("log.html", FileMode.Create, FileAccess.Write));
            stream.WriteLine("<h1>Urban Race Log</h1>");
            stream.WriteLine("<p>Log started at " + System.DateTime.Now.ToLongTimeString() + "</p>");
            stream.Close();
#endif
        }

        public static void log(Type type, string message)
        {
            if (active)
            {
                string colorCode = "";

                switch (type)
                {
                    case Type.ERROR: colorCode = "<span style='color: #ff0000;'>"; break;
                    case Type.INFO: colorCode = "<span style='color: #0008f0;'>"; break;
                    case Type.WARNING: colorCode = "<span style='color: #ffad00;'>"; break; 
                }

                message = colorCode + System.DateTime.Now.ToLongTimeString() + ": " + message + "</spam> </br>";
                output(message);
            }
        }

        private static void output(string message)
        {
#if WINDOWS
            try
            {
                StreamWriter textOut = new StreamWriter(new FileStream("log.html", FileMode.Append, FileAccess.Write));
                textOut.WriteLine(message);
                textOut.Close();
            }
            catch (System.Exception e)
            {
                string error = e.Message;
            }
#endif
        }
    }
}
