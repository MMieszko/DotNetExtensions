using System;
using DotNetExtensions.Collections;
using FluentAssertions;
using Xunit;

namespace DotNetExtensions.Tests.Collections;

public class SinkStackTests
{
    [Theory]
    [InlineData(-99)]
    [InlineData(-1)]
    [InlineData(0)]
    public void Create_CreateWithIncorrectSize_ShouldThrow(int size)
    {
        Assert.Throws<ArgumentException>(() => new SinkStack<int>(size));
    }

    [Fact]
    public void Create_hNullOtherCollection_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new SinkStack<int>(null));
    }

    [Fact]
    public void Create_OtherSizeIsBiggerThanMaxSize_ShouldThrow()
    {
        Assert.Throws<InvalidOperationException>(() => new SinkStack<int>(new[] { 1, 2, 3, 4, 5 }, 4));
    }

    [Fact]
    public void Create_MaxSizeShouldBeSet()
    {
        var stack = new SinkStack<object>(5);

        stack.MaxCount.Should().Be(5);
    }

    [Fact]
    public void Create_StackShouldBeEmpty()
    {
        var stack = new SinkStack<object>(5);

        stack.Count.Should().Be(0);
    }

    [Fact]
    public void Create_BasedOnAnotherCollection()
    {
        var other = new[] { 1, 2, 3, 4 };

        var stack = new SinkStack<int>(other);

        stack.Count.Should().Be(4);
    }

    [Fact]
    public void Create_BasedOnAnotherCollection_ShouldKeepOrder()
    {
        var other = new[] { 1, 2, 3, 4 };

        var stack = new SinkStack<int>(other);

        stack.Peek().Should().Be(4);
    }

    [Fact]
    public void Create_BasedOnAnotherCollectionWithCustomSize()
    {
        var other = new[] { 1, 2, 3, 4 };

        var stack = new SinkStack<int>(other, 6);

        stack.Count.Should().Be(4);
        stack.MaxCount.Should().Be(6);
    }

    [Fact]
    public void Create_BasedOnAnotherCollectionWithCustomSize_ShouldKeepOrder()
    {
        var other = new[] { 1, 2, 3, 4 };

        var stack = new SinkStack<int>(other, 6);

        stack.Peek().Should().Be(4);
    }

    [Fact]
    public void Push_CountShouldIncrease()
    {
        var stack = new SinkStack<int>(5);

        stack.Push(5);

        stack.Count.Should().Be(1);
        stack.Should().HaveCount(1);
    }
}