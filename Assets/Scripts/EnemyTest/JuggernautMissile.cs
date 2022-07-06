using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautMissile : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Missile missilePrefab;
    [SerializeField] string missileTag;
    [SerializeField] JuggernautAI juggernautAI;
    List<Missile> missiles = new List<Missile>(); //Stores shot missiles
    [SerializeField]GameObject[] missileOrigins; //Assigned in inspector the stationary origin points on juggernaut
    [SerializeField] float timeBetweenMissiles;
    bool missilesShot = false; //Tracks whether missiles are ready to be shot

    private void Update()
    {
        if (!missilesShot)
        {
            missilesShot = true;
            animator.Play("JuggernautMissile", animator.GetLayerIndex("Missile Layer"));
        }
    }

    IEnumerator ShootMissiles()
    {
        for(int i = 0;i < missileOrigins.Length; i++)
        {
            missileOrigins[i].SetActive(false);
            Missile missile = ObjectPooler.Instance.SpawnFromPool(missileTag, missileOrigins[i].transform.position, Quaternion.identity).GetComponent<Missile>();//Instantiate<Missile>(missilePrefab, missileOrigins[i].transform.position, Quaternion.identity);//shoot missile
            missiles.Add(missile);
            missile.juggernautAI = juggernautAI;
            missile.juggernautMissile = this;
            yield return new WaitForSeconds(0.07f);
        }
        yield return new WaitForSeconds(timeBetweenMissiles);
        for (int i = 0; i < missileOrigins.Length; i++)
        {
            missileOrigins[i].SetActive(true);
            yield return new WaitForSeconds(0.017f);
        }
        missilesShot = false;
    }

    private void OnEnable()
    {
        for (int i = 0; i < missileOrigins.Length; i++)
        {
            missileOrigins[i].SetActive(true);
        }
        missilesShot = false;
    }

    public void StartShooting()
    {
        StartCoroutine(ShootMissiles());
    }

    public void RemoveFromList(Missile missile)
    {
        missiles.Remove(missile);
    }

    private void OnDisable()
    {
        foreach(Missile missile in missiles){
            if (missile)
            {
                missile.gameObject.SetActive(false);
            }
        }
        missiles.Clear();
    }
}
