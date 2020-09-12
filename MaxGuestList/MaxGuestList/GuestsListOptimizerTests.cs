using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TddXt.AnyRoot.Collections;
using TddXt.AnyRoot.Numbers;
using static TddXt.AnyRoot.Root;


namespace MaxGuestList
{
    public class MaxGuestsListTests
    {
        [Test]
        public void ShouldReturnEmptyMaxGuestListForZeroGuests()
        {
            //GIVEN
            var guests = new int[0];
            var guestsCollisions = new List<(int, int)>();
            var guestsListOptimizer = new GuestsListOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestsLists = guestsListOptimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestsLists.Should().BeEmpty();
        }

        [Test]
        public void ShouldReturnMaxGuestListWithSingleElementForSingleGuest()
        {
            //GIVEN
            var guests = new[] {Any.Integer()};
            var guestsCollisions = new List<(int, int)>();
            var guestsListOptimizer = new GuestsListOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestsLists = guestsListOptimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestsLists.Should().ContainSingle();
            maxGuestsLists.First().Should().BeEquivalentTo(guests);
        }

        [Test]
        public void ShouldReturnAllGuestsWhenExclusionListIsEmpty()
        {
            //GIVEN
            var guests = new[] {0, 1, 2};
            var guestsCollisions = new List<(int, int)>();
            var guestsListOptimizer = new GuestsListOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestsLists = guestsListOptimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestsLists.Should().ContainSingle();
            maxGuestsLists.First().Should().BeEquivalentTo(new HashSet<int>(guests));
        }

        [Test]
        public void ShouldReturnEachGuestAloneWhenEachGuestsCollidesWithEachOther()
        {
            //GIVEN
            var guests = new[] {0, 1, 2};
            var guestsCollisions = new List<(int, int)>
            {
                (0, 1), (1, 2), (2, 0)
            };
            var guestsListOptimizer = new GuestsListOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestsLists = guestsListOptimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestsLists.Should().HaveCount(3);
            maxGuestsLists.Should().ContainEquivalentOf(new List<int> {0});
            maxGuestsLists.Should().ContainEquivalentOf(new List<int> {1});
            maxGuestsLists.Should().ContainEquivalentOf(new List<int> {2});
        }

        [Test]
        public void ShouldReturnOnePossibleGuestsList()
        {
            //GIVEN
            var guests = Enumerable.Range(0, 5).ToArray();
            var guestsCollisions = new List<(int, int)>
            {
                (0, 1),
                (0, 2),
                (2, 3),
                (1, 4),
            };

            var guestsListOptimizer = new GuestsListOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestsLists = guestsListOptimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestsLists.Should().ContainSingle();
            maxGuestsLists.First().Should().BeEquivalentTo(new List<int> {0, 3, 4});
        }

        [Test]
        public void ShouldReturnTwoPossibleMaxGuestsLists()
        {
            //GIVEN
            var guests = new[] {0, 1, 2};
            var guestsCollisions = new List<(int, int)> {(0, 1)};
            var guestsListOptimizer = new GuestsListOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestsLists = guestsListOptimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestsLists.Should().HaveCount(2);
            maxGuestsLists.Should().ContainEquivalentOf(new[] {1, 2});
            maxGuestsLists.Should().ContainEquivalentOf(new[] {0, 2});
        }


        [Test]
        public void ShouldReturnThreePossibleMaxGuestsLists()
        {
            //GIVEN
            var guests = Enumerable.Range(0, 4).ToArray();
            var guestsCollisions = new List<(int, int)>
            {
                (0, 1),
                (1, 2),
                (2, 3)
            };

            var guestsListOptimizer = new GuestsListOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestsList = guestsListOptimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestsList.Should().HaveCount(3);
            maxGuestsList.Should().ContainEquivalentOf(new[] {0, 2});
            maxGuestsList.Should().ContainEquivalentOf(new[] {1, 3});
            maxGuestsList.Should().ContainEquivalentOf(new[] {0, 3});
        }

        [Test]
        public void Large()
        {
            //GIVEN
            var guests = Enumerable.Range(0, 10).ToArray();
            var guestsCollisions = new List<(int, int)>
            {
                (0, 1),
                (1, 2),
                (2, 3)
            };

            var guestsListOptimizer = new GuestsListOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestsLists = guestsListOptimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestsLists.Should().HaveCount(3);
            maxGuestsLists.Should().ContainEquivalentOf(new[] {0, 2});
            maxGuestsLists.Should().ContainEquivalentOf(new[] {1, 3});
            maxGuestsLists.Should().ContainEquivalentOf(new[] {0, 3});
        }

        [Test]
        public void ShouldReturn8PossibleGuestsLists()
        {
            //GIVEN
            var exclusionList = new List<(int, int)>
            {
                (0, 1),
                (2, 3),
                (4, 5)
            };
            var guestsListOptimizer = new GuestsListOptimizer(Enumerable.Range(0, 6).ToArray(), exclusionList);

            //WHEN
            var maxGuestsLists = guestsListOptimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestsLists.Should().HaveCount(8);
            maxGuestsLists.Should().ContainEquivalentOf(new[] {0, 2, 4});
            maxGuestsLists.Should().ContainEquivalentOf(new[] {1, 2, 4});
            maxGuestsLists.Should().ContainEquivalentOf(new[] {0, 3, 4});
            maxGuestsLists.Should().ContainEquivalentOf(new[] {1, 3, 4});
            maxGuestsLists.Should().ContainEquivalentOf(new[] {0, 2, 5});
            maxGuestsLists.Should().ContainEquivalentOf(new[] {1, 2, 5});
            maxGuestsLists.Should().ContainEquivalentOf(new[] {0, 3, 5});
            maxGuestsLists.Should().ContainEquivalentOf(new[] {1, 3, 5});
        }

        [Test]
        public void ShouldThrowExceptionWhenExclusionsListContainsPairWithTheSameGuest()
        {
            //GIVEN
            var anyGuest = Any.Integer();
            var exclusionList = new List<(int, int)>
            {
                (anyGuest, anyGuest)
            };
            var guestsListOptimizer = new GuestsListOptimizer(Any.Array<int>(), exclusionList);

            //WHEN
            Action act = () => guestsListOptimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            act.Should().Throw<InvalidCollisionPairException>()
                .WithMessage($"Guest {anyGuest} cannot be colliding with himself");
        }
    }
}