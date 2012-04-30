public void update(GameTime gameTime, Car car)
{
	// Calculate target angular acceleration
	float angular = car.angle - angle;

	// Clamp angular acceleration
	if (Math.Abs(angular) > maxAngularAcceleration)
	{
		angular /= Math.Abs(angular);
		angular *= maxAngularAcceleration;
	}

	if (Math.Abs(angular) < 0.022f) 
	{
		rotation = 0.0f;
		angular = 0.0f;
		angle = car.angle;
	}

	// Add angular to rotation
	rotation += angular * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

	// Limit rotation
	if (Math.Abs(rotation) > maxRotation)
	{
		rotation /= Math.Abs(rotation);
		rotation *= maxRotation;
	}

	// Add rotation to angle
	angle += rotation * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

	Vector3 target = new Vector3((float)System.Math.Sin(-angle), (float)System.Math.Cos(-angle), 0.0f);
	target *= distance;
	target = car.position - target;
	position = target;

	// Look ahead
	Vector3 lookAt = new Vector3((float)System.Math.Sin(-car.angle), (float)System.Math.Cos(-car.angle), 0.0f);
	lookAt.Normalize();
	lookAt *= aheadDistance;
	lookAt += car.position;
	

	// Update camera view
	view = Matrix.CreateLookAt(new Vector3(position.X, height, -position.Y),
										   new Vector3(lookAt.X, height, -lookAt.Y),
										   Vector3.UnitY);
}