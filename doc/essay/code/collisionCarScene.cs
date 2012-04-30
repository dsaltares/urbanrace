public void collisionCarScene(GameObject o1, GameObject o2)
{
	// Get objects
	Car car;
	GameObject sceneObject;

	if (o1.type == GameObject.Type.CAR)
	{
		car = (Car)o1;
		sceneObject = o2;
	}
	else
	{
		car = (Car)o2;
		sceneObject = o1;
	}

	OrientedBox obb1 = (OrientedBox)car.shape;
	OrientedBox obb2 = (OrientedBox)sceneObject.shape;

	// Crash sfx
	game.audioEngine.SetGlobalVariable("crashVolume", car.velocity.Length() / car.maxSpeed);
	carCrash = game.soundBank.GetCue("crash");
	carCrash.Play();

	// Restore oldPos
	car.position = car.oldPos;

	// Get closest point
	Vector3 q = CollisionManager.closestPointToOBB(obb1.center + obb1.transform.Translation, obb2);

	// Get normal vector
	Vector3 normal = obb1.center + obb1.transform.Translation - q;
	normal.Normalize();

	// Change car velocity to bounce
	float angle = (float)Math.Atan2(normal.Y - car.velocity.Y, normal.X - car.velocity.X);
	Vector3 newDirection = new Vector3((float)System.Math.Sin(-angle), (float)System.Math.Cos(-angle), 0.0f);
	newDirection.Normalize();
	car.velocity = newDirection * car.velocity.Length() * 0.85f;
}