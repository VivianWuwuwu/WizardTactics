using System.Collections;

public class Timer : CombatantStatus
{
    public int duration; // how many turns this lasts for

    public void OnEnable() {
        Parent.OnRefresh.Subscribe(TicDown, this, -1);
    }

    public void OnDisable() {
        Parent.OnRefresh.Unsubscribe(TicDown);
    }

    public IEnumerator TicDown() {
        duration--;
        if (duration == 0) {
            
        }
        yield return null;
    }
}