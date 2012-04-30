public static bool getAABBAABBCollision(Shape s1, Shape s2)
{
	// Safe type conversion
	AxisAlignedBox aabb1 = (AxisAlignedBox)s1;
	AxisAlignedBox aabb2 = (AxisAlignedBox)s2;

	// Collision test (axis separating theorem)
	return aabb1.max.X > aabb2.min.X &&
		   aabb1.min.X < aabb2.max.X &&
		   aabb1.max.Y > aabb2.min.Y &&
		   aabb1.min.Y < aabb2.max.Y &&
		   aabb1.max.Z > aabb2.min.Z &&
		   aabb1.min.Z < aabb2.max.Z;
}