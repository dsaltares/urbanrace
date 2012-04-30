public void changeState(State.Type type)
{
	// Unload current state's content
	currentState.unload();

	// Assign new current state
	currentState = states[type];

	// Load next state's content
	currentState.load();
}