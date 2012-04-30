public static void update()
{
	// Update new states
	newKeyboard = Keyboard.GetState();
	newMouse = Mouse.GetState();
	newGamePad = GamePad.GetState(PlayerIndex.One);

	// COMPARE NEW WITH OLD

	// Keyboard keyPressed and keyReleased
	foreach (Keys key in GetValues<Keys>())
	{
		if (oldKeyboard.IsKeyUp(key) && newKeyboard.IsKeyDown(key) && keyPressedCallback != null)
			keyPressedCallback(key);
		else if (oldKeyboard.IsKeyDown(key) && newKeyboard.IsKeyUp(key) && keyReleasedCallback != null)
			keyReleasedCallback(key);
	}

	// Gamepad padMoved
	
	// Get deltas
	Vector2 deltaLeftPad = new Vector2(newGamePad.ThumbSticks.Left.X - oldGamePad.ThumbSticks.Left.X,
									   newGamePad.ThumbSticks.Left.Y - oldGamePad.ThumbSticks.Left.Y);

	Vector2 deltaRightPad = new Vector2(newGamePad.ThumbSticks.Right.X - oldGamePad.ThumbSticks.Right.X,
									   newGamePad.ThumbSticks.Right.Y - oldGamePad.ThumbSticks.Right.Y);

	float deltaLeftTrigger = newGamePad.Triggers.Left - oldGamePad.Triggers.Left;
	float deltaRightTrigger = newGamePad.Triggers.Right - oldGamePad.Triggers.Right;

	if ((deltaLeftPad.LengthSquared() != 0.0f ||
	     deltaRightPad.LengthSquared() != 0.0f ||
		 deltaLeftTrigger != 0.0f ||
		 deltaRightTrigger != 0.0f) &&
		 padMovedCallback != null)
		padMovedCallback(deltaLeftPad, deltaRightPad, deltaLeftTrigger, deltaRightTrigger);

	// Gamepad buttonPressed and buttonReleased
	foreach (Buttons button in GetValues<Buttons>())
	{
		if (oldGamePad.IsButtonUp(button) && newGamePad.IsButtonDown(button) && buttonPressedCallback != null)
			buttonPressedCallback(button);
		else if (oldGamePad.IsButtonDown(button) && newGamePad.IsButtonUp(button) && buttonReleasedCallback != null)
			buttonReleasedCallback(button);
	}
	
	// Mouse moved
	Vector3 delta = new Vector3(newMouse.X - oldMouse.X,
								newMouse.Y - oldMouse.Y,
								newMouse.ScrollWheelValue - oldMouse.ScrollWheelValue);
	
	if (delta.LengthSquared() != 0 && mouseMovedCallback != null)
		mouseMovedCallback(delta);

	// Mouse button pressed/released
	bool leftPressed = oldMouse.LeftButton == ButtonState.Released && newMouse.LeftButton == ButtonState.Pressed;
	bool rightPressed = oldMouse.RightButton == ButtonState.Released && newMouse.RightButton == ButtonState.Pressed;
	bool middlePressed = oldMouse.MiddleButton == ButtonState.Released && newMouse.MiddleButton == ButtonState.Pressed;

	bool leftReleased = oldMouse.LeftButton == ButtonState.Pressed && newMouse.LeftButton == ButtonState.Released;
	bool rightReleased = oldMouse.RightButton == ButtonState.Pressed && newMouse.RightButton == ButtonState.Released;
	bool middleReleased = oldMouse.MiddleButton == ButtonState.Pressed && newMouse.MiddleButton == ButtonState.Released;

	if ((leftPressed || rightPressed || middlePressed) && mousePressedCallback != null)
		mousePressedCallback(leftPressed, rightPressed, middlePressed);

	if ((leftReleased || rightReleased || middleReleased) && mouseReleasedCallback != null)
		mouseReleasedCallback(leftReleased, rightReleased, middleReleased);

	// Update old states
	oldKeyboard = newKeyboard;
	oldMouse = newMouse;
	oldGamePad = newGamePad;
}