using System.Collections.Generic;
using UnityEngine;

public class TogglePlatforms : MonoBehaviour
{
    // Defining the objects for platforms here
    public GameObject platformRegular, platformWarning, platformHazard;
    public GameObject[] platforms; // Needed to store in the list of regular platforms to pick
    public GameObject thisParent;

    // Feel free to make changes to starting delay time and interval time for playtesting purposes.
    public float delayTime = 5.0f;
    public float intervalTime = 3.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //InvokeRepeating("SwapPlatforms", delayTime, intervalTime);
    }

    // For this method below, periodically, any number of regular blue platforms will
    // toggle between light red (warning) and blue (regular) a few times before
    // being replaced by red (hazard) platforms for a few seconds before reverting back to blue ones.
    // Here is the sequential pattern for reference: regular -> warning -> hazard -> regular -> ...
    private void SwapPlatforms()
    {
        // Define a List of GameObjects.
        List<GameObject> platformList = new List<GameObject>();

        // Define the number of random platforms to toggle.
        int numPlatforms = Random.Range(0, thisParent.transform.childCount)+1;

        while (platformList.Count < numPlatforms)
        {
            // Add a new platform into the list only if it's not inside.
            int thisIdx = Random.Range(0, thisParent.transform.childCount);
            if (!platformList.Contains(platforms[thisIdx])) {
                platformList.Add(platforms[thisIdx]);
            }
        }

        Debug.Log("List of platforms picked");
        for (int i = 0; i < platformList.Count; i++)
        {
            Debug.Log(platformList[i]);
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < platformList.Count; j++)
            {
                Vector3 thisPosition = platformList[j].transform.position;
                Quaternion thisRotation = platformList[j].transform.rotation;
                Transform thisParent = platformList[j].transform.parent;
                platformList[j].SetActive(false);
                GameObject newPlatform = Instantiate(platformWarning, thisPosition, thisRotation, thisParent);
                WaitRoutine(5);
                platformList[j].SetActive(true);
                Destroy(newPlatform);
            }
            WaitRoutine(5);
            
        }
        for (int i = 0; i < platformList.Count; i++)
        {
            Vector3 thisPosition = platformList[i].transform.position;
            Quaternion thisRotation = platformList[i].transform.rotation;
            Transform thisParent = platformList[i].transform.parent;
            GameObject newPlatform = Instantiate(platformHazard, thisPosition, thisRotation, thisParent);
            platformList[i].SetActive(false);
            WaitRoutine(6);
            platformList[i].SetActive(true);
            Destroy(newPlatform);
        }
        WaitRoutine(10);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator<WaitForSeconds> WaitRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
