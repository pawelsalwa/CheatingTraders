using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AWI {
	 [Serializable]
	 public class DebugField {
	 }

	 [Serializable]
	 public class DebugFieldFloat : DebugField {
		  [NonSerialized] public float value;
	 }

	 [Serializable]
	 public class DebugFieldBool : DebugField {
		  [NonSerialized] public bool value;
	 }

	 [Serializable]
	 public class DebugFieldString : DebugField {
		  [NonSerialized] public string value;
	 }
}
