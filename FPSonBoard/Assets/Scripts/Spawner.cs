//キャラクタを生成する
//Attach to GameController

//ポーズ対応

using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	GameObject[] WayPoints;
	int posNum;
	public GameObject[] CharacterPrefabs;
	int charNum;
	int Parcent;
	public int SpawnNum;

	public float area;

	float timer;
	float interval=10f;


	// Use this for initialization
	void Start () {
		//WayPoints = GameObject.FindGameObjectsWithTag("Respawn");
		//posNum = WayPoints.Length;
		charNum = CharacterPrefabs.Length;
		RandomSpawn (SpawnNum);
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > interval) {
			int enemyNum = GameObject.FindGameObjectsWithTag("Enemy").Length;
			RandomSpawn(SpawnNum-enemyNum);
			timer=0f;
		}
	}


	void RandomSpawn(int _spawnNum){
		//int pos = Random.Range (0, posNum);
		//int cha = Random.Range (0, charNum);
		//Shuffle (WayPoints);

		for (int i=0; i<_spawnNum; i++) {
			//int rand = GetRandom(CharacterPrefabs);
			Vector2 newPosition = Random.insideUnitCircle * area;
			Instantiate (CharacterPrefabs [0], new Vector3(newPosition.x,0,newPosition.y) + transform.position, Quaternion.Euler (new Vector3 (0, Random.Range (0f, 360f), 0)));
		}		
		
	}

	void Shuffle(GameObject[] ary){
		int n = ary.Length;
		for(int i = n - 1; i > 0; i--) {
			int j = (int)Mathf.Floor(Random.value * (i + 1));
			GameObject tmp = ary[i];
			ary[i] = ary[j];
			ary[j] = tmp;
		} 
	}

	/*int GetRandom(GameObject[] list){
		int total=0;
		foreach (GameObject obj in list) total += obj.GetComponent<CharInfo>().spawnParcentage;

		float randomPoint = Random.value * total;
		for (int i = 0; i < list.Length; i++) {
			if (randomPoint < list[i].GetComponent<CharInfo>().spawnParcentage)
				return i;
			else
				randomPoint -= list[i].GetComponent<CharInfo>().spawnParcentage;
		}
		return list.Length - 1;
	}*/

}
