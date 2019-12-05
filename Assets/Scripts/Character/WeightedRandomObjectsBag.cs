using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary> adds several weighted objects to a pool, then draws random of them based on their weights</summary>
/// <typeparam name="T"> type of objects to store and draw </typeparam>
public class WeightedRandomObjectsBag<T> {
	
	private struct Entry {
		public int weightOfObj;
		public T weightedObj;
	}

	private List<Entry> entries = new List<Entry>();
	private int totalWeight;

	public void AddWeightedObject(T newWeightedObj, int weight) {
		totalWeight += weight;
		entries.Add(new Entry { weightedObj = newWeightedObj, weightOfObj = weight });
	}

	public T GetRandomWeightedObject() {
		int randomInt = Random.Range(0, totalWeight);
		return entries.FirstOrDefault(obj => obj.weightOfObj >= randomInt).weightedObj;
	}
}
