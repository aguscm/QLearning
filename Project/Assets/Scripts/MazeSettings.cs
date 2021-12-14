using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSettings : MonoBehaviour
{
    public GameObject[] Spawners;
    public GameObject PrefabTarget;
    public GameObject PrefabTrap;
    public GameObject[] TrapOrTargetPlaceholders;
    private List<GameObject> Traps = new List<GameObject>();
    private List<GameObject> Targets = new List<GameObject>();
    // Start is called before the first frame update
    

    void Start()
    {
        ClearAll();
        SortPlaceholdersRandomly();


        //Instantiates the first element, which will be the target
        Targets.Add(Instantiate(PrefabTarget, TrapOrTargetPlaceholders[0].transform.position, Quaternion.identity, TrapOrTargetPlaceholders[0].transform));

        //Instantiates the rest of the elements as traps
        for (var i = 1; i < TrapOrTargetPlaceholders.Length; i++)
        {
            Traps.Add(Instantiate(PrefabTrap, TrapOrTargetPlaceholders[i].transform.position, Quaternion.identity, TrapOrTargetPlaceholders[i].transform));
        }

    }


    //Clears all the traps and targets from the scene (destroying them) and eliminating them from the lists
    private void ClearAll()
    {
        if (Traps.Count > 0)
        {
            foreach (GameObject trap in Traps)
            {
                Destroy(trap);
            }
        }
        if (Targets.Count > 0)
        {
            foreach (GameObject target in Targets)
            {
                Destroy(target);
            }
        }

        Traps.Clear();
        Targets.Clear();
    }

    //Sorts the array of the placeholders randomly
    private void SortPlaceholdersRandomly()
    {
        for (int i = 0; i < TrapOrTargetPlaceholders.Length - 1; i++)
        {
            int rnd = Random.Range(i, TrapOrTargetPlaceholders.Length);
            var tempGO = TrapOrTargetPlaceholders[rnd];
            TrapOrTargetPlaceholders[rnd] = TrapOrTargetPlaceholders[i];
            TrapOrTargetPlaceholders[i] = tempGO;
        }
    }
}
