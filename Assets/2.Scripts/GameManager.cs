using System.Collections;

using UnityEngine;

namespace TDS.Game
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance;

		[Header("--- Zombie Spawn ---")]
		[SerializeField] private Transform[] spawnPoints;
		[SerializeField] private GameObject zombiePrefab;
		[SerializeField] private float spawnTime;

		[Space(10)]
		public Transform zombieStackPoint;



		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			// 좀비 오브젝트 풀 생성
			PoolManager.Instance.CreatePool(zombiePrefab, PoolManager.Instance.gameObject.transform, 30);

			// 스폰 시작
			StartCoroutine(SpawnZombieCor());
		}

		private IEnumerator SpawnZombieCor()
		{
			WaitForSeconds wait = new WaitForSeconds(spawnTime);

			while (true)
			{
				int randIndex = Random.Range(0, spawnPoints.Length);
				Transform spawnPoint = spawnPoints[randIndex];

				GameObject zombieObj = PoolManager.Instance.GetObject(zombiePrefab, spawnPoint.position, Quaternion.identity);
				zombieObj.transform.SetParent(spawnPoint);

				// 하위 모든 RiggingLayer 컴포넌트 처리
				RiggingLayer[] riggings = zombieObj.GetComponentsInChildren<RiggingLayer>(true);
				foreach (RiggingLayer rig in riggings)
				{
					rig.GetComponent<SpriteRenderer>().sortingOrder = rig.defaultLayer + (randIndex * -10);
				}
				zombieObj.layer = LayerMask.NameToLayer("SpawnLine_" + randIndex);

				yield return wait;
			}
		}
	}
}
