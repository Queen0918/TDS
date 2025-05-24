using System.Collections;

using UnityEngine;

namespace TDS.Game
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Zombie : MonoBehaviour
	{
		[Header("--- Movement ---")]
		[SerializeField] private float moveForce;
		[SerializeField] private float maxSpeed;
		[SerializeField] private float verticalUpForce;
		[SerializeField] private float checkRadius;

		[Header("--- Climb ---")]
		[SerializeField] private float climbCheckDistance;
		[SerializeField] private float climbCooldownTime;
		private float climbCooldownTimer = 0f;
		public bool isClimbing;

		[Header("--- GoingBack ---")]
		[SerializeField] private float aboveCheckDistance;
		[SerializeField] private float goingBackTime;
		public bool isGoingBack;

		private Rigidbody2D rigid;
		private Transform target;




		private void Start()
		{
			rigid = GetComponent<Rigidbody2D>();
			target = GameManager.Instance.zombieStackPoint;
		}
		private void Update()
		{
			if (climbCooldownTimer > 0f)
			{
				climbCooldownTimer -= Time.deltaTime;
			}
		}
		private void FixedUpdate()
		{
			if (target == null)
			{
				return;
			}

			CheckZombieSurroundings();

			if (isGoingBack == true)
			{
				//rigid.AddForce(Vector2.right * moveForce);
				transform.position += Vector3.right * maxSpeed * 0.5f * Time.deltaTime;
			}
			else
			{
				rigid.AddForce(Vector2.left * moveForce);
			}

			if (rigid.velocity.magnitude > maxSpeed)
			{
				rigid.velocity = rigid.velocity.normalized * maxSpeed;
			}
		}

		private void CheckZombieSurroundings()
		{
			Vector2 myPos = transform.position;

			int layerMask = 1 << gameObject.layer;
			Collider2D[] hits = Physics2D.OverlapCircleAll(myPos, checkRadius, layerMask);
			foreach (Collider2D col in hits)
			{
				if (col == null || col.gameObject == gameObject)
				{
					continue;
				}
				if (col.CompareTag("Zombie") == false)
				{
					continue;
				}

				Zombie other = col.GetComponent<Zombie>();
				if (other == null)
				{
					continue;
				}

				Vector2 dir = ((Vector2)other.transform.position - myPos).normalized;
				float dist = Vector2.Distance(other.transform.position, myPos);

				// 왼쪽에 있고 가까운 좀비라면 Climb 시도
				if (dir.x < -0.5f && dist <= climbCheckDistance)
				{
					if (isClimbing == false && climbCooldownTimer <= 0f)
					{
						StartCoroutine(ClimbCor(other));
					}
				}

				// 위쪽 좀비 감지 시 뒤로 물러남
				if (dir.y > 0.5f && dist <= aboveCheckDistance)
				{
					if (isGoingBack == false)
					{
						StartCoroutine(GoingBackCor());
					}
				}
			}
		}

		private IEnumerator ClimbCor(Zombie targetZombie)
		{
			isClimbing = true;

			Vector3 targetPosition = transform.position + new Vector3(-1f, 3f, 0f);
			float duration = 0.5f;
			float timer = 0f;

			while (timer < duration)
			{
				Vector2 direction = (targetPosition - transform.position).normalized;
				float factor = Mathf.Lerp(1f, 0f, timer / duration);
				rigid.AddForce(direction * verticalUpForce * factor, ForceMode2D.Force);

				timer += Time.deltaTime;
				yield return null;
			}

			isClimbing = false;
			climbCooldownTimer = climbCooldownTime;
		}

		private IEnumerator GoingBackCor()
		{
			isGoingBack = true;

			yield return new WaitForSeconds(goingBackTime);

			isGoingBack = false;
		}
	}
}
