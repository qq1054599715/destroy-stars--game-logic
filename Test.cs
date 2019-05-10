using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	public GameLogic GameLogic;

	public List<GameLogic.Cell> Cells;
	// Use this for initialization
	void Start () {
		GameLogic = new GameLogic(10,10,5);
		Cells = GameLogic.cells;
	}

	private void OnGUI()
	{
		int Y = 500;
		for (int i = 0; i < Cells.Count; i++)
		{
			var currentCell = Cells[i];
			int size = 50;
			int x = currentCell.x * size;
			int y = currentCell.y * size;
			int id = currentCell.value;
			if (id != -1)
			{
				if (GUI.Button(new Rect(x, Y-y, size, size), id.ToString()))
				{
					List<GameLogic.Cell> removingList = new List<GameLogic.Cell>();
					List<List<GameLogic.Cell>> movingList = new List<List<GameLogic.Cell>>();
					GameLogic.TryRemoveCell(currentCell,ref removingList,ref movingList);
					Cells = GameLogic.cells;
					break;
				}	
			}
			else
			{
				GUI.Box(new Rect(x, Y-y, size, size),"");
			}
		}

		if (GUI.Button(new Rect(0, 550, 100, 100), "重新10*10*5"))
		{
			GameLogic = new GameLogic(10,10,5);
			Cells = GameLogic.cells;
		}
		if (GUI.Button(new Rect(100, 550, 100, 100), "重新5*5*3"))
		{
			GameLogic = new GameLogic(5,5,3);
			Cells = GameLogic.cells;
		}
		if (GUI.Button(new Rect(200, 550, 100, 100), "重新4*4*2"))
		{
			GameLogic = new GameLogic(4,4,2);
			Cells = GameLogic.cells;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
