using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using UnityEngine;

namespace FultEngine.API.Libraries.Audio;

public class AudioPositionTracker : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class UpdatePosition_d_8 : IEnumerator<object>, IDisposable, IEnumerator
	{
		private int __1__state;

		private object __2__current;

		public AudioPositionTracker __4__this;

		private Vector3 position;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return __2__current;
			}
		}

		[DebuggerHidden]
		public UpdatePosition_d_8(int __1__state)
		{
			this.__1__state = __1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			__1__state = -2;
		}

		private bool MoveNext()
		{
			switch (__1__state)
			{
			default:
				return false;
			case 0:
				__1__state = -1;
				break;
			case 1:
				__1__state = -1;
				break;
			}
			if ((Object)(object)__4__this._audioPlayer != (Object)null && (Object)(object)__4__this._speaker != (Object)null && (Object)(object)__4__this._targetTransform != (Object)null)
			{
				position = __4__this._targetTransform.position + __4__this._offset;
				((Component)__4__this._audioPlayer).transform.position = position;
				__4__this._speaker.Position = position;
				__2__current = null;
				__1__state = 1;
				return true;
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private Transform _targetTransform;

	private AudioPlayer _audioPlayer;

	private Speaker _speaker;

	private Vector3 _offset;

	public void StartTracking(Player player, AudioPlayer audioPlayer, Speaker speaker)
	{
		StartTracking((player != null) ? player.Transform : null, audioPlayer, speaker, Vector3.zero);
	}

	public void StartTracking(GameObject gameObject, AudioPlayer audioPlayer, Speaker speaker)
	{
		StartTracking(((Object)(object)gameObject != (Object)null) ? gameObject.transform : null, audioPlayer, speaker, Vector3.zero);
	}

	public void StartTracking(Transform target, AudioPlayer audioPlayer, Speaker speaker, Vector3 offset)
	{
		((MonoBehaviour)this).StopAllCoroutines();
		_targetTransform = target;
		_audioPlayer = audioPlayer;
		_speaker = speaker;
		_offset = offset;
		if ((Object)(object)_targetTransform != (Object)null && (Object)(object)_audioPlayer != (Object)null && (Object)(object)_speaker != (Object)null)
		{
			((MonoBehaviour)this).StartCoroutine(UpdatePosition());
		}
	}

	public void StopTracking()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		_targetTransform = null;
		_audioPlayer = null;
		_speaker = null;
	}

	[IteratorStateMachine(typeof(UpdatePosition_d_8))]
	private IEnumerator UpdatePosition()
	{
		return new UpdatePosition_d_8(0)
		{
			__4__this = this
		};
	}
}
