using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FultEngine.Module.DestroyCamer;

public static class AudioController
{
	[CompilerGenerated]
	private sealed class PlayCameraAudioLoop_d_0 : IEnumerator<float>, IDisposable, IEnumerator
	{
		private int __1__state;

		private float __2__current;

		public GameObject cameraGO;

		public Vector3 cameraPos;

		float IEnumerator<float>.Current
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
		public PlayCameraAudioLoop_d_0(int __1__state)
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
			if (__1__state != 0)
			{
				return false;
			}
			__1__state = -1;
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

	[IteratorStateMachine(typeof(PlayCameraAudioLoop_d_0))]
	public static IEnumerator<float> PlayCameraAudioLoop(GameObject cameraGO, Vector3 cameraPos)
	{
		return new PlayCameraAudioLoop_d_0(0)
		{
			cameraGO = cameraGO,
			cameraPos = cameraPos
		};
	}
}
