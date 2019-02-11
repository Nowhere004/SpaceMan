using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public static GameMaster gm;
    
    void Awake()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
    }

    public Transform playerPrefab;
    public Transform spawnPoint;
    public float spawnDelay = 2f;
    public Transform spawnPrefab;
    public AudioClip[] Sounds;
    public CameraShake cameraShake;

  void Start()
    {
        if (cameraShake==null)
        {
            Debug.LogError("No camera Shake referenced in Gamemaster");
        }
    }
   

    public IEnumerator RespawnPlayer()
    {
        //Debug.Log("TODO: Add waiting for spawn sound");
        GetComponent<AudioSource>().PlayOneShot(Sounds[0]);
        yield return new WaitForSeconds(spawnDelay);

        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        GameObject clone= (Instantiate(spawnPrefab,spawnPoint.position,spawnPoint.rotation)).gameObject;
        Destroy(clone, 3f);
        //Debug.Log("TODO: Add Spawn Particles");
    }

    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm.RespawnPlayer());
    }
    public static void KillEnemy(Enemy enemy)
    {
        
        gm._KillEnemy(enemy);
    }
    public void _KillEnemy(Enemy _enemy)
    {
        GameObject clone=(Instantiate(_enemy.deathParticles,_enemy.transform.position,Quaternion.identity)).gameObject;
        Destroy(clone);
        cameraShake.Shake(_enemy.shakeAmt,_enemy.shakeLength);
        Destroy(_enemy.gameObject);

    }
}