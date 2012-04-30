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