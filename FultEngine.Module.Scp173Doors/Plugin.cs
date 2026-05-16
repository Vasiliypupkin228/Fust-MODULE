using System;
using System.Linq;
using System.Reflection;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using FultEngine.LoaderModule;
using UnityEngine;

namespace FultEngine.Module.Scp173Doors;

public sealed class Plugin : IFultEngineModule
{
	public string Name => "Scp173Doors";

	public string Author => "FUST";

	public Version Version => new Version(1, 0, 0);

	public void OnEnabled()
	{
		Player.InteractingDoor += (CustomEventHandler<InteractingDoorEventArgs>)OnInteractingDoor;
	}

	public void OnDisabled()
	{
		Player.InteractingDoor -= (CustomEventHandler<InteractingDoorEventArgs>)OnInteractingDoor;
	}

	private void OnInteractingDoor(InteractingDoorEventArgs ev)
	{
		if (!(ev.Player == (Player)null) && ev.Door != null && (int)ev.Player.Role.Type <= 0)
		{
			if (IsGateOrForbiddenDoor(ev.Door))
			{
				ev.IsAllowed = false;
				return;
			}
			ev.IsAllowed = false;
			TryBreakDoor(ev.Door, ev.Player);
		}
	}

	private static bool IsGateOrForbiddenDoor(Door door)
	{
		DoorType type = door.Type;
		string text = ((object)(DoorType)(ref type)).ToString();
		string text2 = (((Object)(object)door.Base != (Object)null) ? (((Object)door.Base).name ?? string.Empty) : string.Empty);
		string text3 = (text + " " + text2).ToLowerInvariant();
		return text3.Contains("gate") || text3.Contains("checkpoint") || text3.Contains("elevator") || text3.Contains("bulk") || text3.Contains("blast") || text3.Contains("surface") || text3.Contains("warhead") || text3.Contains("nuke") || text3.Contains("hid");
	}

	private static bool TryBreakDoor(Door door, Player player)
	{
		if (door == null)
		{
			return false;
		}
		if (TryInvokeBreakLikeMethod(door, player))
		{
			return true;
		}
		object @base = door.Base;
		if (@base != null && TryInvokeBreakLikeMethod(@base, player))
		{
			return true;
		}
		try
		{
			MethodInfo methodInfo = @base?.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault((MethodInfo m) => m.Name.IndexOf("Damage", StringComparison.OrdinalIgnoreCase) >= 0);
			if (methodInfo != null && TryInvokeWithGeneratedArguments(methodInfo, @base, player, 9999f))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Log.Warn($"[Scp173Doors] Не удалось выбить дверь {door.Type}: {ex.Message}");
		}
		return false;
	}

	private static bool TryInvokeBreakLikeMethod(object target, Player player)
	{
		if (target == null)
		{
			return false;
		}
		string[] array = new string[6] { "Break", "ServerBreak", "Destroy", "ServerDestroy", "Shatter", "ServerShatter" };
		MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		string[] array2 = array;
		foreach (string name in array2)
		{
			foreach (MethodInfo item in methods.Where((MethodInfo m) => string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase)))
			{
				if (TryInvokeWithGeneratedArguments(item, target, player, 9999f))
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool TryInvokeWithGeneratedArguments(MethodInfo method, object target, Player player, float damage)
	{
		try
		{
			ParameterInfo[] parameters = method.GetParameters();
			object[] array = new object[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				if (parameterType == typeof(float))
				{
					array[i] = damage;
					continue;
				}
				if (parameterType == typeof(int))
				{
					array[i] = (int)damage;
					continue;
				}
				if (parameterType == typeof(bool))
				{
					array[i] = true;
					continue;
				}
				if (parameterType == typeof(ReferenceHub))
				{
					array[i] = player.ReferenceHub;
					continue;
				}
				if (parameterType.IsEnum)
				{
					array[i] = GetEnumValue(parameterType);
					continue;
				}
				if (!parameterType.IsValueType)
				{
					array[i] = null;
					continue;
				}
				return false;
			}
			method.Invoke(target, array);
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static object GetEnumValue(Type enumType)
	{
		Array values = Enum.GetValues(enumType);
		foreach (object item in values)
		{
			string text = item.ToString();
			if (text.IndexOf("Scp", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("Server", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("Unknown", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return item;
			}
		}
		return (values.Length > 0) ? values.GetValue(0) : Activator.CreateInstance(enumType);
	}

	public Type GetConfigType()
	{
		return null;
	}

	public object GetDefaultConfig()
	{
		return null;
	}

	public void SetConfig(object config)
	{
	}
}
