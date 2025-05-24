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
			// ���� ������Ʈ Ǯ ����
			PoolManager.Instance.CreatePool(zombiePrefab, PoolManager.Instance.gameObject.transform, 30);

			// ���� ����
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

				// ���� ��� RiggingLayer ������Ʈ ó��
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
