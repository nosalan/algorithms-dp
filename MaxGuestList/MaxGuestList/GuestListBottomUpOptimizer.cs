﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MaxGuestList
{
    public class GuestListBottomUpOptimizer
    {
        private readonly int[] _guests;
        private readonly IEnumerable<(int first, int second)> _guestsCollisions;
        private readonly Dictionary<int, ISet<int>> _collisionsDict = new Dictionary<int, ISet<int>>();

        public GuestListBottomUpOptimizer(int[] guests, IEnumerable<(int first, int second)> guestsCollisions)
        {
            _guests = guests;
            _guestsCollisions = guestsCollisions;
        }

        public List<List<int>> GetMaxNonCollidingGuestsConfigurations()
        {
            BuildCollisionsHelperDictionary();
            var startGuestIndex = 0;
            return AnalyzeGuestsList(new List<int>(), startGuestIndex);
        }

        private bool AreGuestsColliding(int firstGuest, int secondsGuest)
        {
            return _collisionsDict.TryGetValue(firstGuest, out var value) && value.Contains(secondsGuest);
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

        private List<List<int>> AnalyzeGuestsList(List<int> guestList, int index)
        {
            if (index >= _guests.Length)
                return new List<List<int>> {guestList};

            var guestToTakeOrNot = _guests[index];

            foreach (var guest in guestList)
            {
                if (AreGuestsColliding(guest, guestToTakeOrNot))
                {
                    //not taking it and advancing to new guest
                    return AnalyzeGuestsList(guestList, index + 1);
                }
            }

            var guestListWithNewGuest = new List<int>(guestList) {guestToTakeOrNot};

            //if it not collides we are taking this new guest it and advancing to next guest ..
            var subresultWithNewGuest = AnalyzeGuestsList(guestListWithNewGuest, index + 1);
            //.. and also not taking it and advancing to next guest
            var subresultWithoutNewGuest = AnalyzeGuestsList(guestList, index + 1);

            return Max(subresultWithNewGuest, subresultWithoutNewGuest);
        }

        private static List<List<int>> Max(List<List<int>> subresult1, List<List<int>> subresult2)
        {
            if (subresult1.First().Count > subresult2.First().Count)
            {
                return subresult1;
            }

            if (subresult2.First().Count > subresult1.First().Count)
            {
                return subresult2;
            }

            return subresult1.Concat(subresult2).ToList();
        }
    }
}