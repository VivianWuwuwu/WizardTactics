using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
An "Actor" is any GameObject that needs to be orchestrated in our turn order

TODO - Add some notion of priority to this?
*/
public interface Actor
{
    public abstract IEnumerator Act();
}
