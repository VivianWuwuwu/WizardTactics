using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO -> Think about this harder l8r :((
public class CombinationTable<Ingredient, Output> where Output : class {
    public List<Recipe<Ingredient, Output>> Recipes;
    public Output Combine(ISet<Ingredient> ingredients) {
        foreach (var recipe in Recipes) {
            Output got = recipe.TryCombine(new HashSet<Ingredient>(ingredients));
            if (got != null) {
                return got;
            }
        }
        return null;
    }
}

public class Recipe<Ingredient, Output> where Output : class {
    public Func<ISet<Ingredient>, Output> TryCombine{get; private set;} // Given a set of ingredients this returns null if the recipe couldn't be fulfilled, or an action to combine two ingredients
    private Recipe(Func<ISet<Ingredient>, Output> tryCombine) {
        TryCombine = tryCombine;
    }

    public static Recipe<Ingredient, Output> CreateRecipe<A, B>(Func<(A, B), Output> combine) {
        return GenerateRecipe(Extract<A, B>, combine);
    }

    public static Recipe<Ingredient, Output> CreateRecipe<A, B, C>(Func<(A, B, C), Output> combine) {
        return GenerateRecipe(Extract<A, B, C>, combine);
    }

    // READ AT YOUR OWN RISK - GENERICS HELL BELOW:
    private static Recipe<Ingredient, Output> GenerateRecipe<Ingredients>(Func<ISet<Ingredient>, Ingredients?> extract, Func<Ingredients, Output> combine) where Ingredients : struct {
        Func<ISet<Ingredient>, Output> tryCombine = (ISet<Ingredient> ingredients) => {
            Ingredients? extracted = extract(ingredients);
            if (!extracted.HasValue) {
                return null;
            }
            return combine(extracted.Value);
        };
        return new Recipe<Ingredient, Output>(tryCombine);
    }

    public static bool Extract<A>(ISet<Ingredient> ingredients, out A got) {
        got = default;
        foreach (var ingredient in new HashSet<Ingredient>(ingredients)) {
            if (ingredient is A extracted) {
                got = extracted;
                ingredients.Remove(ingredient); // remove that ingredient now that we successfully extracted it
                return true;
            }
        }
        return false;
    }

    // Two item extraction - IE (Burn, Freeze)
    public static (A, B)? Extract<A, B>(ISet<Ingredient> ingredients) {
        (A, B) result;
        if (!Extract(ingredients, out result.Item1)) {
            return null;
        }
        if (!Extract(ingredients, out result.Item2)) {
            return null;
        }
        return result;
    }

    // Three item extraction - IE (Burn, Water, Freeze)
    public static (A, B, C)? Extract<A, B, C>(ISet<Ingredient> ingredients) {
        (A, B, C) result;
        if (!Extract(ingredients, out result.Item1)) {
            return null;
        }
        if (!Extract(ingredients, out result.Item2)) {
            return null;
        }
        if (!Extract(ingredients, out result.Item3)) {
            return null;
        }
        return result;
    }
}