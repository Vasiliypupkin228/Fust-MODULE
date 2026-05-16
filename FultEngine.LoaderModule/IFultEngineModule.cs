using System;

namespace FultEngine.LoaderModule;

public interface IFultEngineModule
{
	string Name { get; }

	string Author { get; }

	Version Version { get; }

	void OnEnabled();

	void OnDisabled();

	Type GetConfigType();

	object GetDefaultConfig();

	void SetConfig(object config);
}
