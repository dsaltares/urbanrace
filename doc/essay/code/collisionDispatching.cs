public delegate bool CollisionTest (Shape s1, Shape s2);

// ...

protected static Dictionary<KeyValuePair<Type, Type>, CollisionTest> collisionTests;

// ...

public static bool getCollision(Shape s1, Shape s2)
{
	KeyValuePair<Shape.Type, Shape.Type> key = new KeyValuePair<Shape.Type, Shape.Type>(s1.type, s2.type);
	CollisionTest collisionTest;

	if (collisionTests.TryGetValue(key, out collisionTest))
		return collisionTest(s1, s2);
	else
		return false;
}