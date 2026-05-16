using System;
using System.Collections.Generic;
using System.IO;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using MEC;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using UnityEngine;

namespace FultEngine.Module.Pets;

public class Plugin : IFultEngineModule
{
	private Config _config;

	private string DataPath = Path.Combine(Paths.Plugins, "FULT-ENGINE", "Data", "Pets");

	public string Name { get; } = "Pets";


	public string Author { get; } = "FUST";


	public Version Version { get; } = new Version(1, 4, 1);


	public static Plugin Instance { get; private set; }

	public Dictionary<string, PetData> PetsData { get; set; } = new Dictionary<string, PetData>();


	public Dictionary<string, string> PlayerPets { get; set; } = new Dictionary<string, string>();


	public Dictionary<Player, SchematicObject> AttachedPets { get; set; } = new Dictionary<Player, SchematicObject>();


	public void OnEnabled()
	{
		if (_config != null && _config.IsEnabled)
		{
			Instance = this;
			Directory.CreateDirectory(DataPath);
			LoadPetsData();
			LoadPlayerPets();
			Player.Spawned += (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
			Player.Died += (CustomEventHandler<DiedEventArgs>)OnDied;
			Player.ChangingRole += (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
			Player.Left += (CustomEventHandler<LeftEventArgs>)OnLeft;
		}
	}

	public void OnDisabled()
	{
		if (_config != null && _config.IsEnabled)
		{
			SavePlayerPets();
			Player.Spawned -= (CustomEventHandler<SpawnedEventArgs>)OnSpawned;
			Player.Died -= (CustomEventHandler<DiedEventArgs>)OnDied;
			Player.ChangingRole -= (CustomEventHandler<ChangingRoleEventArgs>)OnChangingRole;
			Player.Left -= (CustomEventHandler<LeftEventArgs>)OnLeft;
			Instance = null;
		}
	}

	private void OnSpawned(SpawnedEventArgs e)
	{
		if (e.Player.IsAlive)
		{
			Timing.CallDelayed(_config.SpawnDelay, (Action)delegate
			{
				AttachPet(e.Player);
			});
		}
	}

	private void OnDied(DiedEventArgs e)
	{
		DetachPet(e.Player);
	}

	private void OnChangingRole(ChangingRoleEventArgs e)
	{
		DetachPet(e.Player);
	}

	private void OnLeft(LeftEventArgs e)
	{
		DetachPet(((JoinedEventArgs)e).Player);
		SavePlayerPets();
	}

	internal void AttachPet(Player player)
	{
		if (player == (Player)null)
		{
			Log.Error("[FULT-ENGINE.Pets] Player is null in AttachPet.");
			return;
		}
		if (!PlayerPets.TryGetValue(player.UserId, out var value))
		{
			Log.Debug("[FULT-ENGINE.Pets] No pet assigned for player " + player.UserId + ".");
			return;
		}
		if (!PetsData.TryGetValue(value, out var value2))
		{
			Log.Error("[FULT-ENGINE.Pets] Pet data not found for " + value + ".");
			return;
		}
		if (_config == null)
		{
			Log.Error("[FULT-ENGINE.Pets] Config is null in AttachPet.");
			return;
		}
		DetachPet(player);
		Vector3 val = (Vector3)(((_003F?)_config.PetOffset) ?? value2.Offset);
		Quaternion val2 = (Quaternion)(((_003F?)_config.PetRotation) ?? value2.Rotation);
		Vector3 val3 = (Vector3)(((_003F?)_config.PetScale) ?? value2.Scale);
		try
		{
			SchematicObject val4 = ObjectSpawner.SpawnSchematic(value2.SchematicName, player.Position + val, (Quaternion?)(player.Rotation * val2), (Vector3?)val3, (SchematicObjectDataList)null);
			if ((Object)(object)val4 == (Object)null)
			{
				Log.Error("[FULT-ENGINE.Pets] Не удалось заспавнить схематику '" + value2.SchematicName + "' через MapEditorReborn.");
				return;
			}
			if ((Object)(object)player.Transform == (Object)null)
			{
				Log.Error("[FULT-ENGINE.Pets] Player Transform is null.");
				((MapEditorObject)val4).Destroy();
				return;
			}
			((Component)val4).transform.SetParent(player.Transform);
			((Component)val4).transform.localPosition = val;
			((Component)val4).transform.localRotation = val2;
			((Component)val4).transform.localScale = val3;
			Collider[] componentsInChildren = ((Component)val4).gameObject.GetComponentsInChildren<Collider>(true);
			foreach (Collider val5 in componentsInChildren)
			{
				val5.enabled = false;
			}
			AttachedPets[player] = val4;
		}
		catch (Exception arg)
		{
			Log.Error($"[FULT-ENGINE.Pets] Ошибка AttachPet для {player.Nickname}: {arg}");
		}
	}

	internal void DetachPet(Player player)
	{
		if (AttachedPets.TryGetValue(player, out var value))
		{
			((MapEditorObject)value).Destroy();
			AttachedPets.Remove(player);
		}
	}

	private void LoadPetsData()
	{
		string path = Path.Combine(DataPath, "pets.txt");
		if (!File.Exists(path))
		{
			return;
		}
		string[] array = File.ReadAllLines(path);
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				string[] array3 = text.Split(new char[1] { '|' });
				if (array3.Length >= 6)
				{
					string key = array3[0].Trim();
					string schematic = array3[1].Trim();
					string desc = array3[2].Trim();
					string[] array4 = array3[3].Split(new char[1] { ',' });
					Vector3 offset = (Vector3)((array4.Length == 3) ? new Vector3(float.Parse(array4[0]), float.Parse(array4[1]), float.Parse(array4[2])) : Vector3.up);
					string[] array5 = array3[4].Split(new char[1] { ',' });
					Quaternion rotation = (Quaternion)((array5.Length == 4) ? new Quaternion(float.Parse(array5[0]), float.Parse(array5[1]), float.Parse(array5[2]), float.Parse(array5[3])) : Quaternion.identity);
					string[] array6 = array3[5].Split(new char[1] { ',' });
					Vector3 scale = (Vector3)((array6.Length == 3) ? new Vector3(float.Parse(array6[0]), float.Parse(array6[1]), float.Parse(array6[2])) : Vector3.one);
					PetsData[key] = new PetData(schematic, desc)
					{
						Offset = offset,
						Rotation = rotation,
						Scale = scale
					};
				}
			}
		}
	}

	internal void SavePetsData()
	{
		string path = Path.Combine(DataPath, "pets.txt");
		using StreamWriter streamWriter = new StreamWriter(path);
		foreach (KeyValuePair<string, PetData> petsDatum in PetsData)
		{
			PetData value = petsDatum.Value;
			streamWriter.WriteLine($"{petsDatum.Key}|{value.SchematicName}|{value.Description}|{value.Offset.x},{value.Offset.y},{value.Offset.z}|{value.Rotation.x},{value.Rotation.y},{value.Rotation.z},{value.Rotation.w}|{value.Scale.x},{value.Scale.y},{value.Scale.z}");
		}
	}

	private void LoadPlayerPets()
	{
		string path = Path.Combine(DataPath, "playerpets.txt");
		if (!File.Exists(path))
		{
			return;
		}
		string[] array = File.ReadAllLines(path);
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				string[] array3 = text.Split(new char[1] { '|' });
				if (array3.Length == 2)
				{
					PlayerPets[array3[0].Trim()] = array3[1].Trim();
				}
			}
		}
	}

	internal void SavePlayerPets()
	{
		string path = Path.Combine(DataPath, "playerpets.txt");
		using StreamWriter streamWriter = new StreamWriter(path);
		foreach (KeyValuePair<string, string> playerPet in PlayerPets)
		{
			streamWriter.WriteLine(playerPet.Key + "|" + playerPet.Value);
		}
	}

	public Type GetConfigType()
	{
		return typeof(Config);
	}

	public object GetDefaultConfig()
	{
		return new Config
		{
			IsEnabled = true,
			SpawnDelay = 0.1f,
			PetOffset = Vector3.up,
			PetRotation = Quaternion.identity,
			PetScale = Vector3.one
		};
	}

	public void SetConfig(object config)
	{
		_config = (config as Config) ?? new Config();
	}
}
