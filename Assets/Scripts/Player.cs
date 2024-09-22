using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{

    // Variables para configurar multipliers de velocidad
    // son public para poder editar dentro del editor unity
    public float thrustForce = 100f;
    public float rotationSpeed = 180f;

    public GameObject gun, bulletPrefab, deathScreen;
    public ObjectPooler bulletPool;
    public ObjectPooler meteoritePool;
    public MeteoriteSpawner meteoriteSpawner;
    
    private Rigidbody rigid;
    
    public static int Score = 0;

    private float xBorder = 9.5f, yBorder = 6f;
    private bool isDead = false;
    

    // Start is called before the first frame update
    void Start()
    {
        deathScreen.SetActive(false);
        rigid = GetComponent<Rigidbody>(); // inicializamos el rigidBody
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isDead && Input.GetKey(KeyCode.Return))
        {
            deathScreen.SetActive(false);
            Score = 0;
            Time.timeScale = 1;
            isDead = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        // movimiento frontal
        // notesé que añadí un poco de drag para que resultase más cómodo de mover
        float thrust = Input.GetAxis("Vertical") * Time.deltaTime; // Las teclas de movimiento vertical configuradas, para usarlas como input
        Vector3 thrustDirection = transform.right; // la orientacion inicial de la nave, derecha es la nariz
        rigid.AddForce(thrustDirection * thrust * thrustForce); // añadimos la fuerza al componente rigidBody

        // rotacion
        float rotation = Input.GetAxis("Horizontal") * Time.deltaTime *-1;
        transform.Rotate(Vector3.forward, rotation * rotationSpeed); // usamos la componente transform para rotar con transform.Rotate()

        // "scroll" de movimiento infinito
        scroll();
        
        // disparo
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shoot();
        }
    }

    private void shoot()
    {
        GameObject bullet = bulletPool.GetPooledObject();
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bullet.transform.position = gun.transform.position;
        bullet.transform.rotation = gun.transform.rotation;
        bulletScript.setMeteoriteSpawner(meteoriteSpawner);
        bulletScript.setMeteoritePool(meteoritePool);
        bullet.SetActive(true);
        StartCoroutine(ReturnToPoolAfterTime(bullet));
        
        //GameObject bullet = Instantiate(bulletPrefab, gun.transform.position, Quaternion.identity/* direccion solo la matriz identidad ya que ya la tiene configurada en el prefab*/);
        //Bullet bulletScript = bullet.GetComponent<Bullet>();
        //bulletScript.targetVector = transform.right; // igualamos la direcion de la bullet a la de la nave (derecha, ya que esa es su nariz)
    }
    
    
    private IEnumerator ReturnToPoolAfterTime(GameObject gameObject)
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1.5f)
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
    
    
    private void OnCollisionEnter(Collision collision)
    {
        // Para colisiones con meteoritos, para reiniciar partida
        if (collision.gameObject.CompareTag("Enemy")|| collision.gameObject.CompareTag("SubEnemy"))
        {
            deathScreen.SetActive(true);
            isDead = true;
            Time.timeScale = 0;
        }
    }


    // metodo auxiliar privado que se encarga de comprobar y realizar los cambios de posicion necesarios para el efecto de espacio infinito
    private void scroll()
    {
        Vector3 pos = transform.position;
        if (pos.x > xBorder)
        {
            pos.x = -(xBorder-0.2f);
        } else if (pos.x < -xBorder)
        {
            pos.x = (xBorder-0.2f);
        } else if (pos.y > yBorder)
        {
            pos.y = -(yBorder-0.2f);
        }
        else if (pos.y < -yBorder)
        {
            pos.y = (yBorder-0.2f);
        }
        transform.position = pos;
    }
    
    
}
