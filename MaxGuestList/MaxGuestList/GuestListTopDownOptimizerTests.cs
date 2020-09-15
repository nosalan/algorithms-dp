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
    public class GuestListTopDownOptimizerTests
    {
        [Test]
        public void ShouldReturnEmptyMaxGuestListForZeroGuests()
        {
            //GIVEN
            var guests = new int[0];
            var guestsCollisions = new List<(int, int)>();
            var optimizer = new GuestListTopDownOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestList = optimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestList.Should().BeEmpty();
        }

        [Test]
        public void ShouldReturnMaxGuestListWithSingleElementForSingleGuest()
        {
            //GIVEN
            var guests = new[] {Any.Integer()};
            var guestsCollisions = new List<(int, int)>();
            var optimizer = new GuestListTopDownOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestList = optimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestList.Should().ContainSingle();
            maxGuestList.First().Should().BeEquivalentTo(guests);
        }

        [Test]
        public void ShouldReturnAllGuestsWhenguestsCollisionsIsEmpty()
        {
            //GIVEN
            var guests = new[] {0, 1, 2};
            var guestsCollisions = new List<(int, int)>();
            var optimizer = new GuestListTopDownOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestList = optimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestList.Should().ContainSingle();
            maxGuestList.First().Should().BeEquivalentTo(new HashSet<int>(guests));
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
            var optimizer = new GuestListTopDownOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestList = optimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestList.Should().HaveCount(3);
            maxGuestList.Should().ContainEquivalentOf(new List<int> {0});
            maxGuestList.Should().ContainEquivalentOf(new List<int> {1});
            maxGuestList.Should().ContainEquivalentOf(new List<int> {2});
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

            var optimizer = new GuestListTopDownOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestList = optimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestList.Should().ContainSingle();
            maxGuestList.First().Should().BeEquivalentTo(new List<int> {0, 3, 4});
        }

        [Test]
        public void ShouldReturnTwoPossibleMaxGuestLists()
        {
            //GIVEN
            var guests = new[] {0, 1, 2};
            var guestsCollisions = new List<(int, int)> {(0, 1)};
            var optimizer = new GuestListTopDownOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestList = optimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestList.Should().HaveCount(2);
            maxGuestList.Should().ContainEquivalentOf(new[] {1, 2});
            maxGuestList.Should().ContainEquivalentOf(new[] {0, 2});
        }


        [Test]
        public void ShouldReturnThreePossibleMaxGuestLists()
        {
            //GIVEN
            var guests = Enumerable.Range(0, 4).ToArray();
            var guestsCollisions = new List<(int, int)>
            {
                (0, 1),
                (1, 2),
                (2, 3)
            };

            var optimizer = new GuestListTopDownOptimizer(guests, guestsCollisions);

            //WHEN
            var maxGuestsList = optimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestsList.Should().HaveCount(3);
            maxGuestsList.Should().ContainEquivalentOf(new[] {0, 2});
            maxGuestsList.Should().ContainEquivalentOf(new[] {1, 3});
            maxGuestsList.Should().ContainEquivalentOf(new[] {0, 3});
        }

      
        [Test]
        public void ShouldReturn8PossibleGuestsLists()
        {
            //GIVEN
            var guestsCollisions = new List<(int, int)>
            {
                (0, 1),
                (2, 3),
                (4, 5)
            };
            var optimizer = new GuestListTopDownOptimizer(Enumerable.Range(0, 6).ToArray(), guestsCollisions);

            //WHEN
            var maxGuestList = optimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            maxGuestList.Should().HaveCount(8);
            maxGuestList.Should().ContainEquivalentOf(new[] {0, 2, 4});
            maxGuestList.Should().ContainEquivalentOf(new[] {1, 2, 4});
            maxGuestList.Should().ContainEquivalentOf(new[] {0, 3, 4});
            maxGuestList.Should().ContainEquivalentOf(new[] {1, 3, 4});
            maxGuestList.Should().ContainEquivalentOf(new[] {0, 2, 5});
            maxGuestList.Should().ContainEquivalentOf(new[] {1, 2, 5});
            maxGuestList.Should().ContainEquivalentOf(new[] {0, 3, 5});
            maxGuestList.Should().ContainEquivalentOf(new[] {1, 3, 5});
        }

        [Test]
        public void ShouldThrowExceptionWhenCollisionsListContainsPairWithTheSameGuest()
        {
            //GIVEN
            var anyGuest = Any.Integer();
            var guestsCollisions = new List<(int, int)>
            {
                (anyGuest, anyGuest)
            };
            var optimizer = new GuestListTopDownOptimizer(Any.Array<int>(), guestsCollisions);

            //WHEN
            Action act = () => optimizer.GetMaxNonCollidingGuestsConfigurations();

            //THEN
            act.Should().Throw<InvalidCollisionPairException>()
                .WithMessage($"Guest {anyGuest} cannot be colliding with himself");
        }
    }
}