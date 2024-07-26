using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class TopSortOrder : MonoBehaviour {

	void Start () {
		int mayorSortingOrder = 2;
		foreach (var gameobject in GameObject.FindObjectsOfType<Canvas>())
		{
			int shortOrder = gameobject.GetComponent<Canvas>().sortingOrder;
			mayorSortingOrder = mayorSortingOrder <= shortOrder ? shortOrder + 1 : mayorSortingOrder;
		}
		GetComponent<Canvas>().sortingOrder = mayorSortingOrder;
	}
}
