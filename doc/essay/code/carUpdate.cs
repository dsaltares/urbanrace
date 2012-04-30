public override void update(GameTime gameTime)
{
	// Update old position and angle
	oldPos = position;
	oldAngle = angle;
	
	Vector3 linear = Vector3.Zero;

	// Calculate forces
	
	// Friction
	Vector3 frictionForce = new Vector3();
	
	if (velocity != Vector3.Zero)
	{
		frictionForce = -velocity;
		frictionForce.Normalize();
		frictionForce *= friction;
	}

	// Input
	KeyboardState keyboard = Input.getKeyboard();

	if (keyboard.IsKeyDown(Keys.Up))
	{
		linear = new Vector3((float)System.Math.Sin(-angle), (float)System.Math.Cos(-angle), 0.0f);
		linear *= maxAcceleration;
	}

	else if (keyboard.IsKeyDown(Keys.Down))
	{
		linear = -new Vector3((float)System.Math.Sin(-angle), (float)System.Math.Cos(-angle), 0.0f);
		linear *= brake;
	}

	if (keyboard.IsKeyDown(Keys.Left) && linear != Vector3.Zero)
		angular -= maxAngularAcceleration;

	else if (keyboard.IsKeyDown(Keys.Right) && linear != Vector3.Zero)
		angular += maxAngularAcceleration;

	if (keyboard.IsKeyDown(Keys.Down))
		angular *= -1;

	// Integrate physics
	linear += frictionForce;

	// Update velocity and angle
	velocity += limitVector(linear, maxAcceleration);
	rotation = System.Math.Min(angular, maxAngularAcceleration);

	// Limit velocity
	velocity = limitVector(velocity, maxSpeed);

	// Update position
	position += velocity * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
	angle -= rotation * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

	// Correct graphical rotation
	orientation = Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, angle);

	// Reset angular
	angular = 0.0f;

	// Update sound effect according to car movement
	if (velocity.LengthSquared() > 1.0f)
	{
		game.audioEngine.SetGlobalVariable("engineVolume", 1.0f);
		game.audioEngine.SetGlobalVariable("idleVolume", 0.0f);
		game.audioEngine.SetGlobalVariable("enginePitch", velocity.Length() / maxSpeed);
	}
	else
	{
		game.audioEngine.SetGlobalVariable("engineVolume", 0.0f);
		game.audioEngine.SetGlobalVariable("idleVolume", 1.0f);
	}

	base.update(gameTime);
}