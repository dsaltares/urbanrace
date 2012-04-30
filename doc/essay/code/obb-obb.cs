public static bool getOBBOBBCollision(Shape s1, Shape s2)
{
	OrientedBox o1 = (OrientedBox)s1;
	OrientedBox o2 = (OrientedBox)s2;

	// Matrix to transform other OBB into my reference to allow me to be treated as an AABB
	Matrix toMe = o2.transform * Matrix.Invert(o1.transform);

	Vector3 centerOther = Utility.Multiply(o2.center, toMe);
	Vector3 extentsOther = o2.extent;
	Vector3 separation = centerOther - o1.center;

	Matrix3 rotations = new Matrix3(toMe);
	Matrix3 absRotations = Utility.Abs(rotations);

	float r, r0, r1, r01;

	// Test case 1 - X axis

	r = Math.Abs(separation.X);
	r1 = Vector3.Dot(extentsOther, absRotations.Column(0));
	r01 = o1.extent.X + r1;

	if (r > r01) return false;

	// Test case 1 - Y axis

	r = Math.Abs(separation.Y);
	r1 = Vector3.Dot(extentsOther, absRotations.Column(1));
	r01 = o1.extent.Y + r1;

	if (r > r01) return false;

	// Test case 1 - Z axis

	r = Math.Abs(separation.Z);
	r1 = Vector3.Dot(extentsOther, absRotations.Column(2));
	r01 = o1.extent.Z + r1;

	if (r > r01) return false;

	// Test case 2 - X axis

	r = Math.Abs(Vector3.Dot(rotations.Row(0), separation));
	r0 = Vector3.Dot(o1.extent, absRotations.Row(0));
	r01 = r0 + extentsOther.X;

	if (r > r01) return false;

	// Test case 2 - Y axis

	r = Math.Abs(Vector3.Dot(rotations.Row(1), separation));
	r0 = Vector3.Dot(o1.extent, absRotations.Row(1));
	r01 = r0 + extentsOther.Y;

	if (r > r01) return false;

	// Test case 2 - Z axis

	r = Math.Abs(Vector3.Dot(rotations.Row(2), separation));
	r0 = Vector3.Dot(o1.extent, absRotations.Row(2));
	r01 = r0 + extentsOther.Z;

	if (r > r01) return false;

	// Test case 3 # 1

	r = Math.Abs(separation.Z * rotations[0, 1] - separation.Y * rotations[0, 2]);
	r0 = o1.extent.Y * absRotations[0, 2] + o1.extent.Z * absRotations[0, 1];
	r1 = extentsOther.Y * absRotations[2, 0] + extentsOther.Z * absRotations[1, 0];
	r01 = r0 + r1;

	if (r > r01) return false;

	// Test case 3 # 2

	r = Math.Abs(separation.Z * rotations[1, 1] - separation.Y * rotations[1, 2]);
	r0 = o1.extent.Y * absRotations[1, 2] + o1.extent.Z * absRotations[1, 1];
	r1 = extentsOther.X * absRotations[2, 0] + extentsOther.Z * absRotations[0, 0];
	r01 = r0 + r1;

	if (r > r01) return false;

	// Test case 3 # 3

	r = Math.Abs(separation.Z * rotations[2, 1] - separation.Y * rotations[2, 2]);
	r0 = o1.extent.Y * absRotations[2, 2] + o1.extent.Z * absRotations[2, 1];
	r1 = extentsOther.X * absRotations[1, 0] + extentsOther.Y * absRotations[0, 0];
	r01 = r0 + r1;

	if (r > r01) return false;

	// Test case 3 # 4

	r = Math.Abs(separation.X * rotations[0, 2] - separation.Z * rotations[0, 0]);
	r0 = o1.extent.X * absRotations[0, 2] + o1.extent.Z * absRotations[0, 0];
	r1 = extentsOther.Y * absRotations[2, 1] + extentsOther.Z * absRotations[1, 1];
	r01 = r0 + r1;

	if (r > r01) return false;

	// Test case 3 # 5

	r = Math.Abs(separation.X * rotations[1, 2] - separation.Z * rotations[1, 0]);
	r0 = o1.extent.X * absRotations[1, 2] + o1.extent.Z * absRotations[1, 0];
	r1 = extentsOther.X * absRotations[2, 1] + extentsOther.Z * absRotations[0, 1];
	r01 = r0 + r1;

	if (r > r01) return false;

	// Test case 3 # 6

	r = Math.Abs(separation.X * rotations[2, 2] - separation.Z * rotations[2, 0]);
	r0 = o1.extent.X * absRotations[2, 2] + o1.extent.Z * absRotations[2, 0];
	r1 = extentsOther.X * absRotations[1, 1] + extentsOther.Y * absRotations[0, 1];
	r01 = r0 + r1;

	if (r > r01) return false;

	// Test case 3 # 7

	r = Math.Abs(separation.Y * rotations[0, 0] - separation.X * rotations[0, 1]);
	r0 = o1.extent.X * absRotations[0, 1] + o1.extent.Y * absRotations[0, 0];
	r1 = extentsOther.Y * absRotations[2, 2] + extentsOther.Z * absRotations[1, 2];
	r01 = r0 + r1;

	if (r > r01) return false;

	// Test case 3 # 8

	r = Math.Abs(separation.Y * rotations[1, 0] - separation.X * rotations[1, 1]);
	r0 = o1.extent.X * absRotations[1, 1] + o1.extent.Y * absRotations[1, 0];
	r1 = extentsOther.X * absRotations[2, 2] + extentsOther.Z * absRotations[0, 2];
	r01 = r0 + r1;

	if (r > r01) return false;

	// Test case 3 # 9

	r = Math.Abs(separation.Y * rotations[2, 0] - separation.X * rotations[2, 1]);
	r0 = o1.extent.X * absRotations[2, 1] + o1.extent.Y * absRotations[2, 0];
	r1 = extentsOther.X * absRotations[1, 2] + extentsOther.Y * absRotations[0, 2];
	r01 = r0 + r1;

	if (r > r01) return false;

	return true;  // No separating axis, then we have intersection
}
