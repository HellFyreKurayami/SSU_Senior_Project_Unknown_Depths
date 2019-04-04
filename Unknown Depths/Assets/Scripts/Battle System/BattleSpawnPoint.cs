using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSpawnPoint : MonoBehaviour {

	public Entity spawn(Entity toSpawn)
    {
        Entity e = Instantiate<Entity>(toSpawn, this.transform);
        return e;
    }

    public Player spawn(Player toSpawn)
    {
        Player e = Instantiate<Player>(toSpawn, this.transform);
        return e;
    }
}
