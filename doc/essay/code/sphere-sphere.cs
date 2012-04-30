public static bool getSphereSphereCollision(Shape s1, Shape s2)
{
	// Safe type conversion
	Sphere sphere1 = (Sphere)s1;
	Sphere sphere2 = (Sphere)s2;

	// Collision test
	return (sphere1.center - sphere2.center).LengthSquared() <=
		   (sphere1.radius + sphere2.radius) * (sphere1.radius + sphere2.radius);
}