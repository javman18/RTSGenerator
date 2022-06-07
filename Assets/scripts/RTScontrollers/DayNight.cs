using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using TMPro;

public class DayNight : MonoBehaviour
{
    public GameObject sun;
    public GameObject moon;
    public GameObject gloabalLight2D;
    public float timeOfDay;
    public float timeOfNight;
    public float timer;
    public bool isNight = false;
    public bool isDay = true;
    public int cycles = 0;
    public GameObject rain;
    public GameObject fog;
    bool isFlickering = false;
    private AudioSource audioSource;
    public AudioClip thunderSFX;
    public AudioClip fogSFX;
    private float distance;
    public float seconds;
    public int mins;
    public int hours;
    public int days = 1;
    public float sunPosX;
    float random = 0;
    public float percent;
    Vector2 endPoint;
    Vector2 dif;
    Vector2 start;
    public float lengthOfCycle;
    public bool isMorning, isNoon, isSunset, isDusk, isMidnight, isDawn;
    public AnimationCurve sunIntensity;
    public AnimationCurve moonIntensity;
    public TextMeshProUGUI timeText;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        distance = 4000;
        start = sun.transform.position;
        endPoint = new Vector2(4020, 1000);
        dif = endPoint - start;
        
    }

    // Update is called once per frame
    void Update()
    {
        //CalcTime();
        if(timer <= lengthOfCycle)
        {
            timer += Time.deltaTime;
            
            percent = timer / lengthOfCycle;
            
            //sun.transform.Translate(Vector2.right + dif * percent);
        }
        if(timer > lengthOfCycle)
        {
            timer = 0;
            cycles++;
            //sun.transform.position = new Vector2(0, 1000);
            //Debug.Log(cycles % 2);
            
        }
        if(cycles %2 == 0) // day
        {
            //timeOfNight = 0;
            audioSource.volume = 0f;
            fog.SetActive(true);
            rain.SetActive(false);
            sun.SetActive(true);
            sun.transform.position = start + dif * percent;
            moon.SetActive(false);
            isDay = true;
            isNight = false;
            gloabalLight2D.GetComponent<Light2D>().intensity = sunIntensity.Evaluate(percent) + .5f;
            sun.GetComponent<Light2D>().intensity = sunIntensity.Evaluate(percent);
            if(sun.transform.position.x >= 4000)
            {
                sun.transform.position = new Vector2(-2000, 1000);
            }
            
            

        }
        if (cycles % 2 == 1) //night
        {
            //timeOfDay = 0;
            audioSource.volume = 1f;
            fog.SetActive(false);
            rain.SetActive(true);
            moon.SetActive(true);
            moon.transform.position = start + dif * percent;
            sun.SetActive(false);
            isNight = true;
            isDay = false;
            gloabalLight2D.GetComponent<Light2D>().intensity = moonIntensity.Evaluate(percent) +.5f;
            moon.GetComponent<Light2D>().intensity = moonIntensity.Evaluate(percent);
            if (moon.transform.position.x >= 4000)
            {
                moon.transform.position = new Vector2(-2000, 1000);
            }
            timeOfNight += Time.deltaTime;
            if (!isFlickering)
            {
                StartCoroutine(Thunder());
            }



        }

        CalcTime(percent);
        //GetSunPos();
    }

    IEnumerator Thunder()
    {
        isFlickering = true;
        gloabalLight2D.GetComponent<Light2D>().intensity = 0.3f;
        random = Random.Range(1f, 7f);
   
        yield return new WaitForSeconds(random);

        gloabalLight2D.GetComponent<Light2D>().intensity = 2f;
        
        audioSource.volume = .5f;
        audioSource.PlayOneShot(thunderSFX);
        yield return new WaitForSeconds(0.6f);
        
        isFlickering = false;


    }
    public void CalcTime(float value) 
    {
        if (isDay)
        {
            if(value <= 0.40)
            {
                isMorning = true;
                isNoon = false;
                isSunset = false;
                timeText.text = "Morning";
            }
            else if(value > 0.40 && value <= 0.60)
            {
                isNoon = true;
                isMorning = false;
                isSunset = false;
                timeText.text = "Noon";
            }
            else if (value > 0.60 && value <= 0.99)
            {
                isSunset = true;
                isMorning = false;
                isNoon = false;
                timeText.text = "Sunset";
            }
            else
            {
                isSunset = false;
                isMorning = false;
                isNoon = false;
            }
        }
        else if (isNight)
        {
            if (value <= 0.40)
            {
                isDusk = true;
                isMidnight = false;
                isDawn = false;
                timeText.text = "Dusk";
            }
            else if (value > 0.40 && value <= 0.60)
            {
                isDusk = false;
                isMidnight = true;
                isDawn = false;
                timeText.text = "Midnight";
            }
            else if (value > 0.60 && value <= 0.99)
            {
                isDusk = false;
                isMidnight = false;
                isDawn = true;
                timeText.text = "Dawn";
            }
            else
            {
                isDusk = false;
                isMidnight = false;
                isDawn = false;
            }
        }
    }

}
