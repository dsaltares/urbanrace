public static void loadSettings(string filename)
{
	settings = new Dictionary<string, float>();

	XDocument doc = XDocument.Load(filename);

	foreach (XElement option in doc.Element("options").Descendants("option"))
		settings[option.Attribute("name").Value] = (float)XmlConvert.ToDouble(option.Attribute("value").Value);
}