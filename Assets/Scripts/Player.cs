using UnityEngine;

public class Player : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (GameManager.Instance.uIManager.gameState == GameState.PLAYING && collision.gameObject.CompareTag("Side"))
		{
			GameManager.Instance.GameOver();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (GameManager.Instance.uIManager.gameState == GameState.PLAYING && collision.gameObject.CompareTag("Obstacle"))
		{
			if (collision.gameObject.GetComponent<SpriteRenderer>().color == base.gameObject.GetComponent<SpriteRenderer>().color)
			{
				UnityEngine.Object.Destroy(collision.gameObject);
				GameManager.Instance.OpenSides();
				ScoreManager.Instance.UpdateScore(1);
				AudioManager.Instance.PlayEffects(AudioManager.Instance.sameColor);
			}
			else
			{
				AudioManager.Instance.PlayEffects(AudioManager.Instance.wrongColor);
				GameManager.Instance.GameOver();
			}
		}
	}
}
