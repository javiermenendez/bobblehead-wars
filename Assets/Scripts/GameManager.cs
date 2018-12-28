using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject player;
    public GameObject[] spawnPoints;
    public GameObject alien;
    public GameObject upgradePrefab;
    public Gun gun;
    public float upgradeMaxTimeSpawn = 7.5f;

    public int maxAliensOnScreen;
    public int totalAliens;
    public float minSpawnTime;
    public float maxSpawnTime;
    public int aliensPerSpawn;

    private int aliensOnScreen = 0;
    private float generatedSpawnTime = 0;
    private float currentSpawnTime = 0;
    private bool spawnedUpgrade = false;
    private float actualUpgradeTime = 0;
    private float currentUpgradeTime = 0;

	// Use this for initialization
	void Start () {
        // Determine the actual upgrade time (random)
        actualUpgradeTime = Mathf.Abs(Random.Range(upgradeMaxTimeSpawn - 3.0f, upgradeMaxTimeSpawn));
    }
	
	// Update is called once per frame
	void Update () {

        NoPlayerNoParty();

        // Accumulate the amount of time that’s passed between each frame update
        currentSpawnTime += Time.deltaTime;
        currentUpgradeTime += Time.deltaTime;

        spawnAlien();

        spawnGunUpgrade();
    }

    private void spawnAlien()
    {
        // Check if elapsed time is greater than the randomly generated
        if (currentSpawnTime > generatedSpawnTime)
        {
            // Reset the timer
            currentSpawnTime = 0;

            // This is the spawn-time randomizer
            generatedSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);

            // Whether to spawn
            if (aliensPerSpawn > 0 && aliensOnScreen < totalAliens)
            {
                // Array to keep track of spawning spots
                List<int> previousSpawnLocations = new List<int>();

                #region Prevent from disaster
                // limit the number of aliens you can spawn by the number of spawn points
                if (aliensPerSpawn > spawnPoints.Length)
                    aliensPerSpawn = spawnPoints.Length - 1;

                // a spawning event will never create more aliens than the maximum amount that you've configured
                aliensPerSpawn = (aliensPerSpawn > totalAliens) ? aliensPerSpawn - totalAliens : aliensPerSpawn;
                #endregion Prevent from disaster

                // Iterates once for each spawned alien
                for (int i = 0; i < aliensPerSpawn; i++)
                {
                    // Don't let the bastards grind you down
                    if (aliensOnScreen < maxAliensOnScreen)
                    {
                        aliensOnScreen += 1;

                        #region Find a spawn point
                        // the generated spawn point number
                        int spawnPoint = -1;
                        // loop runs until it finds a spawn point
                        while (spawnPoint == -1)
                        {
                            // possible spawn point
                            int randomNumber = Random.Range(0, spawnPoints.Length - 1);
                            // checks the previousSpawnLocations array to see if that random number is an active spawn point
                            if (!previousSpawnLocations.Contains(randomNumber))
                            {
                                previousSpawnLocations.Add(randomNumber);
                                spawnPoint = randomNumber;
                            }
                        }
                        #endregion Find a spawn point

                        #region Spawn the alien
                        // grab the spawn point
                        GameObject spawnLocation = spawnPoints[spawnPoint];
                        // create an instance of alien prefab
                        GameObject newAlien = ObjectPooler.SharedInstance.GetPooledObject("Alien");

                        if (newAlien != null)
                        {
                            newAlien.SetActive(true);
                            // position the alien in the spawn location
                            newAlien.transform.position = spawnLocation.transform.position;

                            #region Set Alien target
                            Alien alienScript = newAlien.GetComponent<Alien>();
                            alienScript.target = player.transform;

                            // Rotate towards target
                            Vector3 targetRotation = new Vector3(player.transform.position.x, newAlien.transform.position.y, player.transform.position.z);
                            newAlien.transform.LookAt(targetRotation);
                            alienScript.OnDisable.AddListener(AlienKilled);
                            #endregion Set Alien target
                        }

                        #endregion Spawn the alien
                    }
                }
            }
        }
    }

    private void spawnGunUpgrade()
    {
        if (currentUpgradeTime > actualUpgradeTime)
        {
            // After the random time period passes, this checks if the upgrade has already spawned.
            if(!spawnedUpgrade)
            {
                // The upgrade will appear in one of the alien spawn points
                int randomNumber = Random.Range(0, spawnPoints.Length - 1);
                GameObject spawnLocation = spawnPoints[randomNumber];

                // This section handles the business of spawning the upgrade and associating the gun with it.
                GameObject upgrade = Instantiate(upgradePrefab) as GameObject;
                Upgrade upgradeScript = upgrade.GetComponent<Upgrade>();
                upgradeScript.gun = gun;
                upgrade.transform.position = spawnLocation.transform.position;

                // This informs the code that an upgrade has been spawned
                spawnedUpgrade = true;
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.powerUpAppear);
            }
        }
    }

    public void AlienKilled()
    {
        Debug.Log("dead alien");
        aliensOnScreen -= 1;
        totalAliens -= 1;
    }

    private void NoPlayerNoParty()
    {
        if (player == null)
            return;
    }
}
