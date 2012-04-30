public static bool getSphereAABBCollision(Shape s1, Shape s2)
{
	Sphere sphere;
	AxisAlignedBox aabb;
	
	// Safe type conversion
	if (s1.type == Shape.Type.Sphere)
	{
		sphere = (Sphere)s1;
		aabb = (AxisAlignedBox)s2;
	}
	else
	{
		sphere = (Sphere)s2;
		aabb = (AxisAlignedBox)s1;
	}
	
	// Test
	float s = 0.0f;
	float d = 0.0f;

	// Check if the sphere is inside the AABB
	bool centerInsideAABB = (sphere.center.X <= aabb.max.X && 
							 sphere.center.X <= aabb.min.X && 
							 sphere.center.Y <= aabb.max.Y && 
							 sphere.center.Y <= aabb.min.Y && 
							 sphere.center.Z <= aabb.max.Z && 
							 sphere.center.Z <= aabb.min.Z);

	if (centerInsideAABB)
	{
		return true;
	}

	// Check if the sphere and the AABB intersect
	if (sphere.center.X < aabb.min.X) {
		s = sphere.center.X - aabb.min.X;
		d += s * s;
	}
	else if (sphere.center.X > aabb.max.X) {
		s = sphere.center.X - aabb.max.X;
		d += s * s;
	}

	if (sphere.center.Y < aabb.min.Y) {
		s = sphere.center.Y - aabb.min.Y;
		d += s * s;
	}
	else if (sphere.center.Y > aabb.max.Y) {
		s = sphere.center.Y - aabb.max.Y;
		d += s * s;
	}

	if (sphere.center.Z < aabb.min.Z) {
		s = sphere.center.Z - aabb.min.Z;
		d += s * s;
	}
	else if (sphere.center.Z > aabb.max.Z) {
		s = sphere.center.Z - aabb.max.Z;
		d += s * s;
	}

	return d <= sphere.radius * sphere.radius;
}