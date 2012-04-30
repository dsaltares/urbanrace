public static Shape getShape(string name)
{
	Shape shape;
	
	if (shapes.TryGetValue(name, out shape))
		return shape.copy();
	else
		return null;
}