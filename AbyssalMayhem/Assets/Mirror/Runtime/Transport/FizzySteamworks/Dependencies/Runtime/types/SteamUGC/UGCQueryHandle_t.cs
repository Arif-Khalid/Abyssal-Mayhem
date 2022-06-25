// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2019 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

// This file is automatically generated.
// Changes to this file will be reverted when you update Steamworks.NET

#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
	#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS

using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

namespace Steamworks {
	[System.Serializable]
	public struct UGCQueryHandle_t : System.IEquatable<UGCQueryHandle_t>, System.IComparable<UGCQueryHandle_t> {
		public static readonly UGCQueryHandle_t Invalid = new UGCQueryHandle_t(0xffffffffffffffff);
		public ulong m_UGCQueryHandle;

		public UGCQueryHandle_t(ulong value) {
			m_UGCQueryHandle = value;
		}

		public override string ToString() {
			return m_UGCQueryHandle.ToString();
		}

		public override bool Equals(object other) {
			return other is UGCQueryHandle_t && this == (UGCQueryHandle_t)other;
		}

		public override int GetHashCode() {
			return m_UGCQueryHandle.GetHashCode();
		}

		public static bool operator ==(UGCQueryHandle_t x, UGCQueryHandle_t y) {
			return x.m_UGCQueryHandle == y.m_UGCQueryHandle;
		}

		public static bool operator !=(UGCQueryHandle_t x, UGCQueryHandle_t y) {
			return !(x == y);
		}

		public static explicit operator UGCQueryHandle_t(ulong value) {
			return new UGCQueryHandle_t(value);
		}

		public static explicit operator ulong(UGCQueryHandle_t that) {
			return that.m_UGCQueryHandle;
		}

		public bool Equals(UGCQueryHandle_t other) {
			return m_UGCQueryHandle == other.m_UGCQueryHandle;
		}

		public int CompareTo(UGCQueryHandle_t other) {
			return m_UGCQueryHandle.CompareTo(other.m_UGCQueryHandle);
		}
	}
}

#endif // !DISABLESTEAMWORKS