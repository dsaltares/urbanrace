protected override void Update(GameTime gameTime)
{
	// Update input system
	Input.update();

	// Current state update
	currentState.update(gameTime);

	// Update audio
	audioEngine.Update();

	base.Update(gameTime);
}

protected override void Draw(GameTime gameTime)
{
	// Set backgroundd color
	GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

	// Prepare 3D rendering
	GraphicsDevice.BlendState = BlendState.Opaque;
	GraphicsDevice.DepthStencilState = DepthStencilState.Default;

	// Current state draw
	currentState.draw(gameTime);

	base.Draw(gameTime);
}