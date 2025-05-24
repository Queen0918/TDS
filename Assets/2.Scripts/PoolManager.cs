using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDS
{
	public class PoolManager : MonoBehaviour
	{
		public static PoolManager Instance { get; private set; }

		//각 Prefab의 InstanceID를 Key로 사용
		private Dictionary<int, List<GameObject>> poolObjDic = new Dictionary<int, List<GameObject>>();
		private Dictionary<int, Transform> poolTrsfDic = new Dictionary<int, Transform>();



		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}


		public void CreatePool(GameObject prefab, Transform parent, int poolSize = 10)
		{
			int dicKey = prefab.GetInstanceID();

			if (poolObjDic.ContainsKey(dicKey) == false)
			{
				if (poolTrsfDic.ContainsKey(dicKey) == false)
				{
					poolTrsfDic.Add(dicKey, parent);
				}

				poolObjDic.Add(dicKey, new List<GameObject>());
				for (int i = 0; i < poolSize; i++)
				{
					GameObject obj = Instantiate(prefab, parent);
					obj.SetActive(false);

					poolObjDic[dicKey].Add(obj);
				}
			}
		}

		public GameObject AddPool(GameObject prefab)
		{
			int dicKey = prefab.GetInstanceID();

			if (poolObjDic.ContainsKey(dicKey) == true && poolTrsfDic.ContainsKey(dicKey) == true)
			{
				GameObject obj = Instantiate(prefab, poolTrsfDic[dicKey]);

				obj.SetActive(false);
				poolObjDic[dicKey].Add(obj);

				return obj;
			}
			return null;
		}

		public GameObject GetObject(GameObject prefab, Vector3 pos, Quaternion rot)
		{
			int dicKey = prefab.GetInstanceID();

			if (poolObjDic.ContainsKey(dicKey) == true)
			{
				for (int i = 0; i < poolObjDic[dicKey].Count; i++)
				{
					if (poolObjDic[dicKey][i].activeSelf == false)
					{
						GameObject obj = poolObjDic[dicKey][i];
						obj.SetActive(true);
						obj.transform.position = pos;
						obj.transform.rotation = rot;

						return obj;
					}
				}

				GameObject newObj = AddPool(prefab);
				newObj.SetActive(true);
				newObj.transform.position = pos;
				newObj.transform.rotation = rot;

				return newObj;
			}
			else
			{
				Debug.LogError("[PoolManager.GetObject] : prefab instance key not found");
			}

			return null;
		}

		public void ReturnToPool(GameObject obj)
		{
			int dicKey = obj.GetInstanceID();

			foreach (var pair in poolObjDic)
			{
				if (pair.Value.Contains(obj))
				{
					obj.SetActive(false);
					obj.transform.SetParent(poolTrsfDic[pair.Key]);
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localRotation = Quaternion.identity;
					return;
				}
			}
		}
	}
}