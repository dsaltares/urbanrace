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