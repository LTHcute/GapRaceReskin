using UnityEngine;

public class Obstacle : MonoBehaviour
{
	private float obstacleSpeed;

	private float amplitude;

	private float cosSpeed;

	private float angle;

	private float startX;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Bottom"))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void InitOBstacle(float _obstacleSpeed, float _amplitude, float _cosSpeed)
	{
		obstacleSpeed = _obstacleSpeed;
		amplitude = _amplitude;
		cosSpeed = _cosSpeed;
		startX = base.transform.position.x;
	}

	private void Update()
	{
		base.transform.position = new Vector2(startX + Mathf.Cos(angle) * amplitude, base.transform.position.y - obstacleSpeed * Time.deltaTime);
		angle += Time.deltaTime * cosSpeed;
	}
}
