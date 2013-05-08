/// <summary>
/// Binary heap.
/// 
/// What the fuck. It's a fucking binary-heap.
/// 
/// </summary>

using System.Collections.Generic;

public class BinaryHeap<T>
{
	public BinaryHeap(){}
	
	public void Insert(T entry, float metricValue)
	{
		BinaryHeapEntry<T> newEntry = new BinaryHeapEntry<T>();
		newEntry.m_object 			= entry;
		newEntry.m_metricValue 		= metricValue;
		
		m_heapEntries.Add(newEntry);
		
		for(int pos = m_heapEntries.Count - 1; pos >= 1 && metricValue < m_heapEntries[pos / 2].m_metricValue; pos = pos / 2 )
		{
			BinaryHeapEntry<T> tmp = m_heapEntries[pos / 2];
			m_heapEntries[pos / 2] = m_heapEntries[pos];
			m_heapEntries[pos] = tmp;
		}
	}
	
	public T RemoveTop()
	{
		T top = m_heapEntries[0].m_object;
		if(m_heapEntries.Count == 1)
		{
			m_heapEntries.RemoveAt(0);
			return top;
		}
		
		// Chuck the lowest head entry at the top
		m_heapEntries[0] = m_heapEntries[m_heapEntries.Count - 1];
		m_heapEntries.RemoveAt (m_heapEntries.Count - 1);
		
		float currentCost = m_heapEntries[0].m_metricValue;
		int currentIndex = 0;
		
		bool lowestFound = false;
		
		while((currentIndex + 1) * 2 < m_heapEntries.Count && !lowestFound)
		{
			int index1 = (currentIndex + 1) * 2;
			int index2 = (currentIndex + 1) * 2 - 1;
			
			float cost1 		= m_heapEntries[index1].m_metricValue;
			float cost2 		= m_heapEntries[index2].m_metricValue;
			
			if(currentCost < cost1 && currentCost < cost2)
			{
				lowestFound = true;
				continue;
			}
			
			int switchIndex = cost1 < cost2 ? index1 : index2;
		
			BinaryHeapEntry<T> tmp = m_heapEntries[switchIndex];
			m_heapEntries[switchIndex] = m_heapEntries[currentIndex];
			m_heapEntries[currentIndex] = tmp;
			
			currentIndex = switchIndex;
		}
		
		return top;
	}
	
	public T GetTop()
	{
		return m_heapEntries[0].m_object;	
	}
		
	public List<string> OutputTree()
	{
		List<string> outputStrings = new List<string>();
		
		foreach(var entry in m_heapEntries)
		{
			outputStrings.Add(entry.m_metricValue.ToString());
		}
		
		return outputStrings;
	}
	
	public bool HasItems()
	{
		return m_heapEntries.Count != 0;
	}
	
	public void Reset()
	{
		m_heapEntries.Clear();	
	}
	
	private List<BinaryHeapEntry<T>> m_heapEntries = new List<BinaryHeapEntry<T>>();
}

public class BinaryHeapEntry<T>
{
	public T m_object;
	public float m_metricValue;
}
