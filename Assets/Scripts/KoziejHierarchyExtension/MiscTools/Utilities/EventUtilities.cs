using System;
using System.Collections.Generic;
using UnityEngine;

namespace AWI {
	 public static class EventUtilities {
		  public static bool IsCoping {
				get {
					 return Event.current != null && Event.current.type == EventType.ExecuteCommand && (Event.current.commandName == "Duplicate" || Event.current.commandName == "Paste");
				}
		  }
	 }
}