using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Ok we want an ability to specify combos. For a combo, provide a func of how we should resolve this
*/
public class CombinationTable<T>
{
    public Nullable<V> Extract<V>(ISet<object> given) where V : struct {
        return null;
    }

    public void Combine<V>(V ingredients) where V : struct {
        // ...
    }

    public Func<ISet<object>, Action> TryRecipe<V>(Func<ISet<object>, Nullable<V>> extract, System.Action<V> combine) where V : struct {
        return (ISet<object> all) => {
            var extracted = extract(all);
            if (!extracted.HasValue) {
                return null;
            }
            return () => {
                combine(extracted.Value);
            };
        };
    }
}
// ^ Did she pop off? Is this slop?? Who's to say hehe
