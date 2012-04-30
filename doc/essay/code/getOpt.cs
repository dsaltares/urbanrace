public static float getOpt(string name)
{
	float value;

	if (settings.TryGetValue(name, out value))
		return value;
	else
		return 0.0f;
}