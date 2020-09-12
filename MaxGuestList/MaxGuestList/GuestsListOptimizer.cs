using System;
using System.Collections.Generic;
using System.Linq;

namespace MaxGuestList
{
    public enum GuestsRelation : byte
    {
        Collision,
        NoCollision
    }

    public class GuestsListOptimizer
    {
        private readonly int[] _guests;
        private readonly IEnumerable<(int first, int second)> _guestsCollisions;
        private readonly Dictionary<int, ISet<int>> _collisionsDict = new Dictionary<int, ISet<int>>();
        private readonly Dictionary<string, GuestsRelation> _memoTable = new Dictionary<string, GuestsRelation>();

        public GuestsListOptimizer(int[] guests, IEnumerable<(int first, int second)> guestsCollisions)
        {
            _guests = guests;
            _guestsCollisions = guestsCollisions;
        }

        public List<List<int>> GetMaxNonCollidingGuestsConfigurations()
        {
            BuildCollisionsHelperDictionary();
            var maxGuestLists = new List<List<int>> {new List<int>()};
            AnalyzeGuestsList(_guests.ToList(), maxGuestLists);

            return maxGuestLists.First().Any()
                ? maxGuestLists
                : EachGuestAlone();
        }

        private void BuildCollisionsHelperDictionary()
        {
            foreach (var (first, second) in _guestsCollisions)
            {
                if (first == second)
                {
                    throw new InvalidCollisionPairException(first);
                }

                UpdateCollisionsDictionary(first, second);
                UpdateCollisionsDictionary(second, first);
            }
        }

        private void UpdateCollisionsDictionary(int first, int second)
        {
            if (_collisionsDict.ContainsKey(first))
            {
                _collisionsDict[first].Add(second);
            }
            else
            {
                _collisionsDict[first] = new HashSet<int>(new[] {second});
            }
        }

        private GuestsRelation AnalyzeGuestsList(List<int> guestsList, List<List<int>> maxGuestsLists)
        {
            //memoization is used for performance benefits only
            //to avoid recomputing the same problem during recursion

            var memoKey = string.Join("_", guestsList);
            if (_memoTable.ContainsKey(memoKey))
            {
                return _memoTable[memoKey];
            }

            if (guestsList.Count == 2)
            {
                AnalyzeForTwoGuests(guestsList, maxGuestsLists, memoKey);
            }
            else
            {
                AnalyzeForMoreThanTwoGuests(guestsList, maxGuestsLists, memoKey);
            }

            return _memoTable[memoKey];
        }

        private void AnalyzeForTwoGuests(List<int> guestsList, List<List<int>> maxGuestsLists, string memoKey)
        {
            var first = guestsList[0];
            var second = guestsList[1];

            if (AreGuestsColliding(first, second))
            {
                _memoTable[memoKey] = GuestsRelation.Collision;
            }
            else
            {
                _memoTable[memoKey] = GuestsRelation.NoCollision;
                UpdateMaxGuestsList(guestsList, maxGuestsLists);
            }
        }

        private void AnalyzeForMoreThanTwoGuests(List<int> guestsList, List<List<int>> maxGuestsLists, string memoKey)
        {
            var resultOfChildrenComputations = new List<GuestsRelation>();

            //creating subproblems (sublists), each with one, different element removed
            //for list with 'n' elements there will be 'n' sublists
            //e.g.: [0,1,2,3] -> [0,1,2], [0,1,3], [0,2,3], [1,2,3]

            for (var indexOfGuestToRemove = guestsList.Count - 1;
                indexOfGuestToRemove >= 0;
                indexOfGuestToRemove--)
            {
                var subproblemList = new List<int>(guestsList);
                subproblemList.RemoveAt(indexOfGuestToRemove);
                var childResult = AnalyzeGuestsList(subproblemList, maxGuestsLists);
                resultOfChildrenComputations.Add(childResult);
            }

            //if all(!) of the subproblems don't have collisions it means that the current list doesn't have collision either
            //so this is potentially a max guests list or a sublist of one
            if (resultOfChildrenComputations.All(result => result == GuestsRelation.NoCollision))
            {
                _memoTable[memoKey] = GuestsRelation.NoCollision;
                UpdateMaxGuestsList(guestsList, maxGuestsLists);
            }
            else
            {
                _memoTable[memoKey] = GuestsRelation.Collision;
            }
        }

        private bool AreGuestsColliding(int firstGuest, int secondsGuest)
        {
            return _collisionsDict.TryGetValue(firstGuest, out var value) && value.Contains(secondsGuest);
        }

        private void UpdateMaxGuestsList(List<int> currentGuestsList, List<List<int>> maxGuestsLists)
        {
            var currentMaxGuestsListLength = maxGuestsLists.First().Count;
            if (currentMaxGuestsListLength == currentGuestsList.Count)
            {
                maxGuestsLists.Add(currentGuestsList);
            }
            else if (currentMaxGuestsListLength < currentGuestsList.Count)
            {
                maxGuestsLists.Clear();
                maxGuestsLists.Add(currentGuestsList);
            }
        }

        private List<List<int>> EachGuestAlone()
        {
            return _guests.GroupBy(e => e).Select(e => e.ToList()).ToList();
        }
    }

    public class InvalidCollisionPairException : Exception
    {
        public InvalidCollisionPairException(in int guestNumber) :
            base($"Guest {guestNumber} cannot be colliding with himself")
        {
        }
    }
}