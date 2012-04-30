class Input
{
	public delegate void KeyboardCallback(Keys key);
	public delegate void PadMovedCallback(Vector2 leftPad, Vector2 rightPad, float leftTrigger, float rightTrigger);
	public delegate void PadButtonCallback(Buttons button);
	public delegate void MouseMovedCallback(Vector3 delta);
	public delegate void MouseButtonCallback(bool left, bool right, bool middle);
	
	// ...
}