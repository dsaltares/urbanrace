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
 * @file TrackManager.cs
 * @description Track manager implementation
 * @author David Saltares <david.saltares@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using System.Xml;

namespace urbanrace
{
    class Track
    {
        public string name { get; protected set; }
        public string filename { get; protected set; }
        public string song { get; protected set; }
        public string imageFile { get; protected set; }
        public Texture2D image { get; protected set; }
        public double record { get; set; }
        public int laps { get; protected set; }
        public double time { get; protected set; }

        public Track(string name, string filename, string imageFile, Texture2D image, double record, int laps, double time, string song)
        {
            this.name = name;
            this.filename = filename;
            this.imageFile = imageFile;
            this.image = image;
            this.record = record;
            this.laps = laps;
            this.time = time;
            this.song = song;
        }
    }

    class TrackManager
    {
        public static List<Track> tracks { get; protected set; }

        public static void loadTracks(UrbanRace game)
        {
            // Create track list
            tracks = new List<Track>();
            
            // Load xml file with tracks
            XDocument doc = XDocument.Load(game.Content.RootDirectory + "\\XML\\tracks.xml");

            foreach (XElement node in doc.Element("tracks").Descendants("track"))
            {
                string name = node.Attribute("name").Value;
                string filename = node.Attribute("file").Value;
                string song = node.Attribute("song").Value;
                int laps = XmlConvert.ToInt32(node.Attribute("laps").Value);
                double time = XmlConvert.ToDouble(node.Attribute("time").Value);
                double record = XmlConvert.ToDouble(node.Attribute("record").Value);
                string imageFile = node.Attribute("image").Value;
                Texture2D image = game.Content.Load<Texture2D>("Images\\" + imageFile);

                tracks.Add(new Track(name, filename, imageFile, image, record, laps, time, song));
            }
        }

        public static void saveTracks(UrbanRace game)
        {
            // Create new document
            XDocument doc = new XDocument();

            // Create tracks node
            XElement tracksNode = new XElement("tracks");

            foreach (Track track in tracks)
            {
                // Create new track xml element
                XElement trackNode = new XElement("track");

                // Add attributes
                trackNode.SetAttributeValue("name", track.name);
                trackNode.SetAttributeValue("file", track.filename);
                trackNode.SetAttributeValue("song", track.song);
                trackNode.SetAttributeValue("laps", track.laps);
                trackNode.SetAttributeValue("time", track.time);
                trackNode.SetAttributeValue("record", track.record);
                trackNode.SetAttributeValue("image", track.imageFile);

                // Attach track node to tracks node
                tracksNode.Add(trackNode);
            }

            // Attack tracks node to doc
            doc.Add(tracksNode);

            Log.log(Log.Type.INFO, "Saving xml file with record");

            // Save xml file
            doc.Save(game.Content.RootDirectory + "\\XML\\tracks.xml");
        }

        public static bool setNewRecord(string filename, double record)
        {
            for (int i = 0; i < tracks.Count; ++i)
            {
                if (tracks[i].filename == filename && (tracks[i].record == 0.0 || tracks[i].record > record))
                {
                    tracks[i].record = record;
                    return true;
                }
            }
            return false;
        }
    }
}
