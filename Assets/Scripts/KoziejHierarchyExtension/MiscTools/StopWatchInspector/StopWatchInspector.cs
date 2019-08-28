using UnityEngine;
using System.Diagnostics;
using System;

namespace AWI {
	 [Serializable]
	 public class StopWatchInspector {
		  private Stopwatch stopwatch;
		  private int m_Tickets;
		  private int m_Miliseconds;
		  private int m_Fps;
		  
		  public int tickets { get { return m_Tickets; } }
		  public int miliseconds { get { return m_Miliseconds; } }
		  public int fps { get { return m_Fps; } }

		  public void Start() {
				if(stopwatch == null) {
					 stopwatch = new Stopwatch();
				}
				stopwatch.Reset();
				stopwatch.Start();
		  }

		  public void Stop() {
				if(stopwatch == null) {
					 m_Fps = 0;
					 m_Miliseconds = 0;
					 m_Tickets = 0;
					 return;
				}
				stopwatch.Stop();
				m_Tickets = (int)stopwatch.ElapsedTicks;
				m_Miliseconds = (int)stopwatch.Elapsed.TotalMilliseconds;
				if(m_Miliseconds > 0) {
					 m_Fps = (int)(1000 / m_Miliseconds);
				} else {
					 m_Fps = int.MaxValue;
				}
		  }
	 }
}
