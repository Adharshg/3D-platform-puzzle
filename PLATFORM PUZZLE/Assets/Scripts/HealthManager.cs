using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {

	public int maxHealth = 10;
	public int currentHealth;

	public PlayerControl thePlayer;

	public float invincibililtyLength;
	private float invincibililtyCounter;

	public Renderer PlayerRenderer;
	private float flashCounter;
	public float flashLength = 0.1f;

	private bool isRespawning;
	private Vector3 respawnPoint;
	public float RespawnLength = 2f;

	public GameObject deathEffect;

	public Image blackScreen;
	private bool isFadeToBlack;
	private bool isFadeFromBlack;
	public float fadeSpeed;
	public float waitForFade;

	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;

		//thePlayer = FindObjectOfType<PlayerControl>();

		respawnPoint = thePlayer.transform.position;

	}
	
	// Update is called once per frame
	void Update () {

		if(invincibililtyCounter > 0)
		{
			invincibililtyCounter -= Time.deltaTime;

			flashCounter -= Time.deltaTime;

			if(flashCounter <= 0)
			{
				PlayerRenderer.enabled = !PlayerRenderer.enabled;
				flashCounter = flashLength;
			}

			if(invincibililtyCounter <= 0)
			{
				PlayerRenderer.enabled = true;
			}
		}

		if(isFadeToBlack)
		{
			blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.MoveTowards(blackScreen.color.a, 1f, fadeSpeed * Time.deltaTime));

			if(blackScreen.color.a == 1f)
			{
				isFadeToBlack = false;
			}
		}

		
		if(isFadeFromBlack)
		{
			blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.MoveTowards(blackScreen.color.a, 0f, fadeSpeed * Time.deltaTime));

			if(blackScreen.color.a == 0f)
			{
				isFadeFromBlack = false;
			}
		}
		
	}

	public void HurtPlayer(int damage, Vector3 direction)
	{
		if(invincibililtyCounter <= 0)
		{
			currentHealth -= damage;

			if(currentHealth <= 0)
			{
				Respawn();
			}
			else
			{
				thePlayer.Knockback(direction);

				invincibililtyCounter = invincibililtyLength;

				PlayerRenderer.enabled = false;

				flashCounter = flashLength;
			}
			
		}

	}

	public void Respawn()
	{
			if(!isRespawning)
			StartCoroutine("RespawnCo");

	}

	public IEnumerator RespawnCo()
	{
		isRespawning = true;
		thePlayer.gameObject.SetActive(false);
		Instantiate(deathEffect, thePlayer.transform.position, thePlayer.transform.rotation);

		yield return new WaitForSeconds(RespawnLength);

		isFadeToBlack = true;

		yield return new WaitForSeconds(waitForFade);

		isFadeFromBlack = true;

		isRespawning = false;

		thePlayer.gameObject.SetActive(true);
		thePlayer.transform.position = respawnPoint;
		currentHealth = maxHealth;

		invincibililtyCounter = invincibililtyLength;
		PlayerRenderer.enabled = false;
		flashCounter = flashLength;
		
	}

	public void HealPlayer(int healAmount)
	{
		if(currentHealth < maxHealth)
		currentHealth = maxHealth;
	}
}
