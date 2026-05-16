using System.ComponentModel;
using Exiled.API.Interfaces;

namespace FultEngineMvp;

public sealed class Config : IConfig
{
	public bool IsEnabled { get; set; } = true;


	public bool Debug { get; set; } = false;


	[Description("Право игрока, который может запускать MVP музыку после конца раунда. Если RestrictMusicToRoundMvp = true, это право может быть обходом для админов/донатеров.")]
	public string Permission { get; set; } = "Specific.MVP";


	[Description("Если true, MVP музыка доступна только автоматически выбранному MVP раунда. Игрок с Permission сможет обойти ограничение, если PermissionCanBypassMvpRestriction = true.")]
	public bool RestrictMusicToRoundMvp { get; set; } = true;


	[Description("Может ли Permission обходить ограничение RestrictMusicToRoundMvp.")]
	public bool PermissionCanBypassMvpRestriction { get; set; } = true;


	[Description("Показывать всем Broadcast/Hint в конце раунда, кто MVP и за что он получил MVP.")]
	public bool BroadcastMvpReasonOnRoundEnd { get; set; } = true;


	[Description("Показывать причину MVP в сообщении, когда игрок включает MVP трек.")]
	public bool ShowMvpReasonWhenPlayingTrack { get; set; } = true;


	[Description("Сколько секунд показывать сообщение о MVP раунда.")]
	public float MvpReasonBroadcastSeconds { get; set; } = 10f;


	[Description("Fallback-причина, если игрок получил MVP вручную/правом, но статистика по нему не найдена.")]
	public string ManualMvpReasonFallback { get; set; } = "MVP выдан вручную администрацией";


	[Description("Очки MVP за одно убийство.")]
	public int MvpPointsPerKill { get; set; } = 25;


	[Description("Дополнительные очки MVP за убийство SCP.")]
	public int MvpBonusPerScpKill { get; set; } = 100;


	[Description("Очки MVP за каждые 100 нанесённого урона.")]
	public int MvpPointsPer100Damage { get; set; } = 2;


	[Description("Очки MVP за побег.")]
	public int MvpBonusForEscape { get; set; } = 45;


	[Description("Очки MVP за выживание до конца раунда.")]
	public int MvpBonusForSurvival { get; set; } = 20;


	[Description("Штраф очков MVP за смерть.")]
	public int MvpPenaltyPerDeath { get; set; } = 10;


	[Description("Минимальный нанесённый урон, чтобы показывать его в причине MVP.")]
	public float MinDamageToShowInReason { get; set; } = 100f;


	[Description("Папка с .ogg треками. Если оставить пустым, будет: EXILED/Plugins/FULT-ENGINE/MVPMusic")]
	public string MusicFolderPath { get; set; } = string.Empty;


	[Description("Имя архива в папке FULT-ENGINE. Авто-распаковка отключена: распакуй .ogg вручную в папку MVPMusic.")]
	public string MusicZipName { get; set; } = "MVPMusic.zip";


	[Description("ID выпадающего списка Server Specific. ID не должен конфликтовать с другими SS настройками.")]
	public int MusicDropdownId { get; set; } = 7010;


	[Description("ID кнопки/переключателя запуска в Server Specific. ID не должен конфликтовать с другими SS настройками.")]
	public int PlayToggleId { get; set; } = 7011;


	[Description("Название выпадающего списка Server Specific.")]
	public string DropdownLabel { get; set; } = "\ud83c\udfb5 Выбор MVP трека";


	[Description("Название переключателя запуска Server Specific.")]
	public string PlayToggleLabel { get; set; } = "▶ Включить выбранный MVP трек";


	[Description("Подсказка у списка треков.")]
	public string DropdownHint { get; set; } = "Выбери музыку, которую хочешь включить после окончания раунда.";


	[Description("Подсказка у кнопки запуска.")]
	public string PlayToggleHint { get; set; } = "После окончания раунда переключи на Да, чтобы запустить выбранную музыку.";


	[Description("Можно ли запускать музыку только после RoundEnded.")]
	public bool OnlyAfterRoundEnd { get; set; } = true;


	[Description("Окно в секундах после конца раунда, когда разрешено включать MVP трек. 0 или меньше = до рестарта раунда.")]
	public int RoundEndWindowSeconds { get; set; } = 45;


	[Description("Разрешить только один MVP трек за раунд.")]
	public bool OneTrackPerRound { get; set; } = true;


	[Description("ОПАСНО: если true, выбор трека в dropdown после конца раунда сразу запускает музыку. Рекомендую false, чтобы не было случайных запусков и нагрузки.")]
	public bool PlayOnDropdownChangeAfterRoundEnd { get; set; } = false;


	[Description("Останавливать прошлый MVP трек перед запуском нового.")]
	public bool StopPreviousTrackBeforePlay { get; set; } = true;


	[Description("Громкость глобального Speaker. Лучше держать 0.55-0.75, чтобы не ловить клиппинг/белый шум у клиентов.")]
	public float Volume { get; set; } = 0.65f;


	[Description("Префикс внутренних clipName, чтобы не конфликтовать с другими звуками.")]
	public string ClipPrefix { get; set; } = "mvp_";


	[Description("SAFE AUDIO: грузить .ogg только при запуске выбранного трека, а не все 76 файлов на старте. Очень рекомендуется true.")]
	public bool LazyLoadAudioClips { get; set; } = true;


	[Description("SAFE AUDIO: максимальная длительность проигрывания MVP трека. После этого AudioPlayer принудительно уничтожается. 0 или меньше = не ограничивать.")]
	public float MaxTrackSeconds { get; set; } = 22f;


	[Description("SAFE AUDIO: минимальная пауза между попытками запуска от одного игрока, чтобы не спамили Server Specific callback.")]
	public float PlayerPlayCooldownSeconds { get; set; } = 2.5f;


	[Description("SAFE AUDIO: минимальная глобальная пауза между стартами треков на сервере.")]
	public float GlobalPlayCooldownSeconds { get; set; } = 3f;


	[Description("SAFE AUDIO: максимальный размер одного .ogg файла в мегабайтах. 0 или меньше = не проверять.")]
	public float MaxTrackFileSizeMb { get; set; } = 5f;


	[Description("SAFE AUDIO: AddClip(..., destroyOnEnd: true). Не оставляет клипы висеть в AudioPlayer после конца проигрывания.")]
	public bool DestroyClipOnEnd { get; set; } = true;


	[Description("SAFE AUDIO: уничтожать AudioPlayer при завершении/очистке клипов.")]
	public bool DestroyPlayerWhenAllClipsPlayed { get; set; } = true;


	[Description("SAFE AUDIO: при отключении/новом раунде чистить все MVP AudioPlayer несколько раз. Полезно против зависших звуков.")]
	public bool AggressiveAudioCleanup { get; set; } = true;


	[Description("Сообщение игроку, который может выбрать MVP музыку после конца раунда.")]
	public string RoundEndMessage { get; set; } = "<b><color=#ffd166>\ud83c\udfa7 MVP музыка доступна!</color></b>\n<size=23>Открой <color=#70e000>Настройки → Server Specific</color>, выбери трек и нажми <color=#70e000>Да</color>.</size>\n<size=20>Запасная команда: <color=#00d4ff>.mvp list</color> / <color=#00d4ff>.mvp 1</color></size>";

}
