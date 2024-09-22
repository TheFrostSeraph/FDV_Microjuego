using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MeteoriteSpawner : MonoBehaviour
{

    public GameObject asteroidPrefab;
    public ObjectPooler metoritePool;
    
    private float spawnRatePM = 30f;
    private float spawnRateIncrement = 1f;
    private float spawnLimity = 6.5f;
    private float spawnLimitx = 10.5f;
    private float maxLifeTime = 6f;
    private Vector3 meteoriteScale = new Vector3(0.3f, 0.3f, 1);
    
    private float nextSpawn = 0f;
    

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + 60 / spawnRatePM; // El proximo spawn sera el tiempo actual + el tiempo medio dado por spawnrate
            spawnRatePM += spawnRateIncrement;
            
            // Para el spawn preferí optar por todas las direcciones, no solo arriba, y usé también
            // una rotacion que aprox mira al centro de la pantalla, y por ello una velocity en esa direccion en vez de gravedad
            
            (Vector3, Quaternion) spawnAndRot = selectSpawnAndRot(); // Seleccionamos la posicion y rotaticion del met y lo instanciamos
            GameObject meteor = metoritePool.GetPooledObject();
            meteor.tag = "Enemy"; // reconfiguramos tag por si estaba en pool tras ser split
            meteor.SetActive(true);
            meteor.transform.position = spawnAndRot.Item1;
            meteor.transform.rotation = spawnAndRot.Item2;
            meteor.transform.localScale = meteoriteScale; // reseteamos tamaño
            meteor.transform.localScale *= 0.9f * Random.Range(1.3f,0.7f); // randomizamos tamaño un poco
            meteor.GetComponent<Rigidbody>().velocity = -meteor.transform.up * (4f*Random.Range(1.2f,0.8f)); // moverlo en la direccion a la que apunta, en este caso, hacia abajo
            StartCoroutine(ReturnToPoolAfterTime(meteor));
        }
    }

    public IEnumerator ReturnToPoolAfterTime(GameObject gameObject)
    {
        
        float elapsedTime = 0f;
        while (elapsedTime < maxLifeTime)
        {
            // If the object is deactivated manually, break out of the coroutine
            if (!gameObject.activeInHierarchy)
            {
                yield break;  // Stop the coroutine early
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // After the time is up, return the object to the pool if it's still active
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }

    }
    
    
    // metodo auxiliar privado que devuelve el vector3 con la posicion de spawn seleccionada aleatoriamiente y
    // el Quaternion que contiene la rotacion asignada
    private (Vector3,Quaternion) selectSpawnAndRot()
    {
        Vector3 spawnPos;
        Quaternion spawnRot;
        string axis = Random.value > 0.5 ? "x" : "y"; // decidimos eje (x para arriba y abajo e "y" para izq y derecha
        int plusorminus = Random.value > 0.5 ? 1 : -1; // decidimos lado (izq vs dcha y arriba vs abajo)
        
        if (axis == "x")
        {
            spawnPos = new Vector3(Random.Range(-spawnLimitx, spawnLimitx), plusorminus * spawnLimity, -1f);
            float rotDegree = plusorminus > 0 ? 0 : 180; // si es arriba el hacemos mirar a abajo (rot inicial) o si es abajo le hacemos mirar arriba (rotar 180)
            spawnRot = Quaternion.Euler(0, 0 , rotDegree*Random.Range(1.2f,0.8f)); // añadimos un poco de rng a la rotacion
        } 
        else // if (axis == "y")
        {
            spawnPos = new Vector3(plusorminus * spawnLimitx, Random.Range(-spawnLimity, spawnLimity), -1f);
            float rotDegree = plusorminus > 0 ? -90 : 90; // si es izq el hacemos mirar a la derecha y viciversa (+-90)
            spawnRot = Quaternion.Euler(0, 0, rotDegree*Random.Range(1.2f,0.8f));  // añadimos un poco de rng a la rotacion
        }
        return (spawnPos,spawnRot);
    }
}
