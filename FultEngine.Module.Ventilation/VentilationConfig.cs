using System.Collections.Generic;
using Exiled.API.Enums;

namespace FultEngine.Module.Ventilation;

public class VentilationConfig
{
	public bool IsEnabled { get; set; } = true;


	public bool Debug { get; set; } = false;


	public bool AllowScp { get; set; } = false;


	public bool ShowNoOptionsHint { get; set; } = true;


	public string EntrySchematicName { get; set; } = "VENT_ENTRY";


	public string GoBackMarkerName { get; set; } = "GOBACKTOSURFACE";


	public List<string> ExitMarkerNames { get; set; } = new List<string> { "EXIT", "EXIT2", "EXIT3" };


	public Vector3Config VentilationCenter { get; set; } = new Vector3Config(-3.98f, 301.168f, 15.605f);


	public float EntryInteractionRadius { get; set; } = 6.5f;


	public float ExitInteractionRadius { get; set; } = 2.25f;


	public float EntryLookRayDistance { get; set; } = 5f;


	public float ExitLookRayDistance { get; set; } = 2.6f;


	public float MenuUpdateInterval { get; set; } = 0.55f;


	public float MarkerCacheRefreshSeconds { get; set; } = 30f;


	public float InitialMarkerCacheDelay { get; set; } = 2.5f;


	public int MaxParentScanDepth { get; set; } = 7;


	public bool EnableDeepSchematicScan { get; set; } = true;


	public bool DeepScanOnlyWhenMarkerMissing { get; set; } = true;


	public bool EnableNearbyColliderFallback { get; set; } = true;


	public bool UseNearbyFallbackOnlyWhenCacheMissing { get; set; } = false;


	public float NearbyFallbackCooldown { get; set; } = 0.9f;


	public float EnterTeleportYOffset { get; set; } = 0.35f;


	public float ExitTeleportYOffset { get; set; } = 0.35f;


	public float ReturnPositionYOffset { get; set; } = 0.15f;


	public float RoomCenterYOffset { get; set; } = 1.15f;


	public bool DoubleTeleportFix { get; set; } = true;


	public bool VentAmbienceEnabled { get; set; } = true;


	public string VentAmbienceClipName { get; set; } = "VentAmbience";


	public float VentAmbienceVolume { get; set; } = 0.55f;


	public float VentAmbienceMaxDistance { get; set; } = 0.15f;


	public bool ForceStopVentAmbienceOnCleanup { get; set; } = true;


	public float ActionCooldown { get; set; } = 1.25f;


	public float RoundExitSelectDelay { get; set; } = 2f;


	public float LoadingSeconds { get; set; } = 1.8f;


	public float LoadingUpdateInterval { get; set; } = 0.12f;


	public int LoadingBarSegments { get; set; } = 18;


	public bool CancelLoadingOnMove { get; set; } = true;


	public float MaxMoveDuringLoading { get; set; } = 1.65f;


	public bool UseRandomMapRooms { get; set; } = true;


	public bool UniqueRandomTargetsPerRound { get; set; } = true;


	public List<ZoneType> AllowedRandomZones { get; set; } = new List<ZoneType>
	{
		(ZoneType)1,
		(ZoneType)2,
		(ZoneType)4
	};


	public List<CustomExitTargetConfig> CustomExitTargets { get; set; } = new List<CustomExitTargetConfig>
	{
		new CustomExitTargetConfig
		{
			Name = "LCZ пример",
			Position = new Vector3Config(0f, 1f, 0f)
		},
		new CustomExitTargetConfig
		{
			Name = "HCZ пример",
			Position = new Vector3Config(10f, 1f, 10f)
		},
		new CustomExitTargetConfig
		{
			Name = "EZ пример",
			Position = new Vector3Config(-10f, 1f, -10f)
		}
	};


	public string MenuTitle { get; set; } = "Вентиляция";


	public string MenuFooter { get; set; } = "↑/↓ — навигация | Enter — выбрать | V — закрыть";


	public string LoadingText { get; set; } = "Синхронизация доступа...";


	public string EnterLabel { get; set; } = "<color=#8a72ff>Войти в вентиляцию</color>";


	public string ExitLabelPrefix { get; set; } = "<color=#8a72ff>Выйти через</color>";


	public string GoBackLabel { get; set; } = "<color=#8a72ff>Вернуться к месту входа</color>";


	public int MenuY { get; set; } = 870;


	public int MenuX { get; set; } = 0;


	public int LoadingY { get; set; } = 870;


	public int LoadingX { get; set; } = 0;


	public int NotifyY { get; set; } = 139;


	public int NotifyX { get; set; } = 0;

}
