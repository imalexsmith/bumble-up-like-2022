using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

// ========================
// Revision 2022.04.29
// ========================

namespace NightFramework
{
    public enum RandomValueSelectionMode
    {
        SingleElement = 0,
        SeveralWithRepeats = 1,
        SeveralWithNoRepeats = 2,
        EveryNotNull = 3
    }

    [Serializable]
    public struct RandomisedSetEntry<T>
    {
        public T Value;
        public bool Always;
        [ConditionalField(nameof(Always), true), Min(0f)]
        public float Weight;

        public RandomisedSetEntry(T value) : this()
        {
            Value = value;
        }

        public RandomisedSetEntry(T value, bool always, float weight) : this(value)
        {
            Always = always;
            Weight = weight;
        }
    }

    [Serializable]
    public class RandomisedSet<T>
    {
        // ========================================================================================
        public RandomValueSelectionMode SelectionMode;
        [ConditionalField(nameof(SelectionMode), false, RandomValueSelectionMode.SeveralWithRepeats, RandomValueSelectionMode.SeveralWithNoRepeats), Min(1)]
        public int SelectionRounds = 2;
        public List<RandomisedSetEntry<T>> Values = new List<RandomisedSetEntry<T>>();


        // ========================================================================================
        public List<T> SelectRandomValues()
        {
            return SelectRandomValues(SelectionMode, SelectionRounds);
        }

        public List<T> SelectRandomValues(RandomValueSelectionMode mode, int rounds)
        {
            var result = new List<T>();

            if (mode == RandomValueSelectionMode.EveryNotNull)
            {
                result.AddRange(Values.Select(x => x.Value).Where(x => x != null));
            }
            else
            {
                var selectFrom = new List<RandomisedSetEntry<T>>();
                foreach (var value in Values)
                {
                    if (value.Always)
                        result.Add(value.Value);
                    else
                        selectFrom.Add(value);
                }

                if (selectFrom.Count == 0)
                    return result;

                rounds = mode switch
                {
                    RandomValueSelectionMode.SingleElement => 1,
                    RandomValueSelectionMode.SeveralWithRepeats => rounds,
                    RandomValueSelectionMode.SeveralWithNoRepeats => Mathf.Min(rounds, selectFrom.Count),
                    _ => throw new UnityException(),
                };

                for (int i = 0; i < rounds; i++)
                {
                    var single = SelectSingle(selectFrom);

                    if (single.Value != null)
                        result.Add(single.Value);

                    if (mode == RandomValueSelectionMode.SeveralWithNoRepeats)
                        selectFrom.Remove(single);
                }
            }

            return result;
        }

        private RandomisedSetEntry<T> SelectSingle(IEnumerable<RandomisedSetEntry<T>> from)
        {
            var sumWeight = from.Sum(x => x.Weight);
            var r = UnityEngine.Random.Range(0, sumWeight);
            foreach (var item in from)
            {
                r -= item.Weight;
                if (r <= 0)
                    return item;
            }

            throw new UnityException();
        }
    }
}