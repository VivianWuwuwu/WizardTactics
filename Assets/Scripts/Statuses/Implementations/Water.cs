using System.Collections;

// No-Op status - only used for combining
public class Water : CombatantStatus {
    public static IEnumerator Extinguish(Water w, Burn b) {
        w.Destroy();
        b.Destroy();
        // Play smoke animation here?
        yield return null;
    }
}