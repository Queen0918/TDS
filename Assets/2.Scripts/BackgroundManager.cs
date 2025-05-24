using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDS.Game
{
	public class BackgroundManager : MonoBehaviour
	{
		public enum BackgroundType { Top, Bottom}

		[Header("--- Background Top ---")]
		[SerializeField] private Transform[] backgroundsTop;
		[SerializeField] private float scrollSpeedTop;
		private float backgroundTopWidth;

		[Header("--- Background Bottom ---")]
		[SerializeField] private Transform[] backgroundsBottom;
		[SerializeField] private float scrollSpeedBottom;
		private float backgroundBottomWidth;



		private void Awake()
		{
			// Top Background Sprite Width Set
			if (backgroundsTop.Length >= 2)
			{
				SpriteRenderer sr = backgroundsTop[0].GetComponent<SpriteRenderer>();
				float spriteWidth = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
				backgroundTopWidth = spriteWidth * backgroundsTop[0].localScale.x;
			}
			else
			{
				Debug.LogError("[Game.BackgroundManager : backgroundsTop.Length error");
			}

			// Bottom Background Sprite Width Set
			if (backgroundsBottom.Length >= 2)
			{
				SpriteRenderer sr = backgroundsBottom[0].GetComponent<SpriteRenderer>();
				float spriteWidth = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
				backgroundBottomWidth = spriteWidth * backgroundsBottom[0].localScale.x;
			}
			else
			{
				Debug.LogError("[Game.BackgroundManager.Awake] : backgroundsBottom.Length error");
			}
		}

		private void Update()
		{
			// Top
			foreach (Transform bg in backgroundsTop)
			{
				bg.position += Vector3.left * scrollSpeedTop * Time.deltaTime;

				if (bg.position.x <= -backgroundTopWidth)
				{
					float rightMostX = GetRightMostBackground(BackgroundType.Top).position.x;
					bg.position = new Vector3(rightMostX + backgroundTopWidth, bg.position.y, bg.position.z);
				}
			}

			// Bottom
			foreach (Transform bg in backgroundsBottom)
			{
				bg.position += Vector3.left * scrollSpeedBottom * Time.deltaTime;

				if (bg.position.x <= -backgroundBottomWidth)
				{
					float rightMostX = GetRightMostBackground(BackgroundType.Bottom).position.x;
					bg.position = new Vector3(rightMostX + backgroundBottomWidth, bg.position.y, bg.position.z);
				}
			}
		}

		private Transform GetRightMostBackground(BackgroundType backgroundType)
		{
			if (backgroundType == BackgroundType.Top)
			{
				return backgroundsTop[0].position.x > backgroundsTop[1].position.x ? backgroundsTop[0] : backgroundsTop[1];
			}
			else if (backgroundType == BackgroundType.Bottom)
			{
				return backgroundsBottom[0].position.x > backgroundsBottom[1].position.x ? backgroundsBottom[0] : backgroundsBottom[1];
			}
			else
			{
				Debug.LogError("[Game.BackgroundManager.GetRightMostBackground] : backgroundType Error");
				return null;
			}
		}
	}
}
