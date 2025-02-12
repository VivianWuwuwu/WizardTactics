using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LogBehavior : CombatantBehavior
{
    public async override Task<IEnumerator> Decide()
    {
        return LogAction();
    }

    private IEnumerator LogAction() {
        Debug.Log("Decided action!");
        yield return new WaitForSeconds(0.2f);
        Debug.Log("Finished action!");
    }
}
