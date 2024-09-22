using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{

    public GameObject asteroidPrefab;
    private MeteoriteSpawner meteoriteSpawner;
    private ObjectPooler meteoritePool;
    
    public float speed = 13f;
    public float maxLifeTime = 3f;
    
    
    private Vector3 targetVector = Vector3.right;
    private Vector3 meteoriteScale = new Vector3(0.3f, 0.3f, 1);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(targetVector * (speed * Time.deltaTime));
    }


    private void OnCollisionEnter(Collision collision)
    {
        // Colision para meteoritos
        if (collision.gameObject.CompareTag("Enemy")||collision.gameObject.CompareTag("SubEnemy"))
        {
            IncreaseScore();
            gameObject.SetActive(false); // paramos la bala
            if (collision.gameObject.CompareTag("Enemy"))
            {
                split(collision.gameObject.transform.position); // ya que es meteorito entero, fragmentamos
                
            }
            collision.gameObject.SetActive(false); // destruimos el meteorito
            
        } 
    }

    
    private void IncreaseScore()
    {
        Player.Score++;
        // update text
        GameObject scoreText = GameObject.FindGameObjectWithTag("UI");
        scoreText.GetComponent<Text>().text = "Score: " + Player.Score;
    }
    


    private void split(Vector3 pos)
    {
        // seleccionamos rotacion en el spawn
        Quaternion spawnRot1, spawnRot2;
        float rotDegree = Random.Range(-120f,120f); 
        spawnRot1 = Quaternion.Euler(0, 0, rotDegree*Random.Range(1.3f,0.7f));  // añadimos un poco de rng a la rotacion
        spawnRot2 = Quaternion.Euler(0, 0, -rotDegree*Random.Range(1.3f,0.7f));  // direccion contraria (pero con un poco de rng)
        gestionarSubMeteor(spawnRot1, pos); // subrutina para gestionar creacion y configuracion
        gestionarSubMeteor(spawnRot2, pos);
    }

    private void gestionarSubMeteor(Quaternion spawnRot, Vector3 spawnPos)
    {
        GameObject meteor = meteoritePool.GetPooledObject(); // cogemos de pool
        meteor.SetActive(true); // activamos
        meteor.tag = "SubEnemy"; // damos diferente tag para evitar inifinitas 
        meteor.transform.rotation = spawnRot; // configuramos rotacion, posicion, y tamaño
        meteor.transform.position = spawnPos;
        meteor.transform.localScale = meteoriteScale;
        meteor.transform.localScale *= 0.4f*Random.Range(1.3f,0.7f); // reducimos tamaño
        meteor.GetComponent<Rigidbody>().velocity = -meteor.transform.up * (4f*Random.Range(1.3f,0.7f)); // moverlo en la direccion a la que apunta, en este caso, hacia abajo
        StartCoroutine(meteoriteSpawner.ReturnToPoolAfterTime(meteor));
    }

    
    public void setMeteoriteSpawner(MeteoriteSpawner metSpawn)
    {
        meteoriteSpawner = metSpawn;
    }
    
    public void setMeteoritePool(ObjectPooler objPool)
    {
        meteoritePool = objPool;
    }
    
    
}
