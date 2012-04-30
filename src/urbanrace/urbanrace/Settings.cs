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
 * @file Settings.cs
 * @description Options system
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace urbanrace
{
    class Settings
    {
        static Dictionary<string, float> settings;
        
        public static void loadSettings(string filename)
        {
            settings = new Dictionary<string, float>();

            XDocument doc = XDocument.Load(filename);

            foreach (XElement option in doc.Element("options").Descendants("option"))
                settings[option.Attribute("name").Value] = (float)XmlConvert.ToDouble(option.Attribute("value").Value);
        }

        public static float getOpt(string name)
        {
            float value;
            
            if (settings.TryGetValue(name, out value))
                return value;
            else return
                0.0f;
        }
    }
}
