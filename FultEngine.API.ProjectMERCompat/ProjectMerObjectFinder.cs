using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using UnityEngine;

namespace FultEngine.API.ProjectMERCompat;

public static class ProjectMerObjectFinder
{
	[CompilerGenerated]
	private sealed class GetSpawnedObjects_d_2 : IEnumerable<MapEditorObject>, IEnumerable, IEnumerator<MapEditorObject>, IDisposable, IEnumerator
	{
		private int __1__state;

		private MapEditorObject __2__current;

		private int __l__initialThreadId;

		private HashSet<MapEditorObject> returned;

		private IEnumerator<MapEditorObject> __s__2;

		private MapEditorObject obj;

		private IEnumerator<MapSchematic> __s__4;

		private MapSchematic map;

		private IEnumerator<MapEditorObject> __s__6;

		private MapEditorObject obj;

		MapEditorObject IEnumerator<MapEditorObject>.Current
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
		public GetSpawnedObjects_d_2(int __1__state)
		{
			this.__1__state = __1__state;
			__l__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = __1__state;
			switch (num)
			{
			case -3:
			case 1:
				try
				{
				}
				finally
				{
					__m__Finally1();
				}
				break;
			case -5:
			case -4:
			case 2:
				try
				{
					if (num == -5 || num == 2)
					{
						try
						{
						}
						finally
						{
							__m__Finally3();
						}
					}
				}
				finally
				{
					__m__Finally2();
				}
				break;
			}
			returned = null;
			__s__2 = null;
			obj = null;
			__s__4 = null;
			map = null;
			__s__6 = null;
			obj = null;
			__1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				switch (__1__state)
				{
				default:
					return false;
				case 0:
					__1__state = -1;
					returned = new HashSet<MapEditorObject>();
					if (MapUtils.UntitledMap?.SpawnedObjects != null)
					{
						__s__2 = MapUtils.UntitledMap.SpawnedObjects.Where((MapEditorObject x) => (Object)(object)x != (Object)null).GetEnumerator();
						__1__state = -3;
						goto IL_00f2;
					}
					goto IL_010e;
				case 1:
					__1__state = -3;
					goto IL_00ea;
				case 2:
					{
						__1__state = -5;
						goto IL_0209;
					}
					IL_0209:
					obj = null;
					goto IL_0211;
					IL_0211:
					if (__s__6.MoveNext())
					{
						obj = __s__6.Current;
						if (returned.Add(obj))
						{
							__2__current = obj;
							__1__state = 2;
							return true;
						}
						goto IL_0209;
					}
					__m__Finally3();
					__s__6 = null;
					map = null;
					goto IL_0234;
					IL_00ea:
					obj = null;
					goto IL_00f2;
					IL_0234:
					if (__s__4.MoveNext())
					{
						map = __s__4.Current;
						__s__6 = map.SpawnedObjects.Where((MapEditorObject x) => (Object)(object)x != (Object)null).GetEnumerator();
						__1__state = -5;
						goto IL_0211;
					}
					__m__Finally2();
					__s__4 = null;
					return false;
					IL_010e:
					if (MapUtils.LoadedMaps == null)
					{
						return false;
					}
					__s__4 = MapUtils.LoadedMaps.Values.Where((MapSchematic x) => x?.SpawnedObjects != null).GetEnumerator();
					__1__state = -4;
					goto IL_0234;
					IL_00f2:
					if (__s__2.MoveNext())
					{
						obj = __s__2.Current;
						if (returned.Add(obj))
						{
							__2__current = obj;
							__1__state = 1;
							return true;
						}
						goto IL_00ea;
					}
					__m__Finally1();
					__s__2 = null;
					goto IL_010e;
				}
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			return this.MoveNext();
		}

		private void __m__Finally1()
		{
			__1__state = -1;
			if (__s__2 != null)
			{
				__s__2.Dispose();
			}
		}

		private void __m__Finally2()
		{
			__1__state = -1;
			if (__s__4 != null)
			{
				__s__4.Dispose();
			}
		}

		private void __m__Finally3()
		{
			__1__state = -4;
			if (__s__6 != null)
			{
				__s__6.Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<MapEditorObject> IEnumerable<MapEditorObject>.GetEnumerator()
		{
			if (__1__state == -2 && __l__initialThreadId == Environment.CurrentManagedThreadId)
			{
				__1__state = 0;
				return this;
			}
			return new GetSpawnedObjects_d_2(0);
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<MapEditorObject>)this).GetEnumerator();
		}
	}

	public static IEnumerable<MapEditorObject> SpawnedObjects => GetSpawnedObjects();

	[IteratorStateMachine(typeof(GetSpawnedObjects_d_2))]
	public static IEnumerable<MapEditorObject> GetSpawnedObjects()
	{
		return new GetSpawnedObjects_d_2(-2);
	}

	public static IEnumerable<SchematicObject> GetSpawnedSchematics()
	{
		return GetSpawnedObjects().OfType<SchematicObject>();
	}
}
