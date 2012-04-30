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