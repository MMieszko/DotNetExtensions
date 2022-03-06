using System;
using System.Collections.Generic;
using System.Linq;
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

    [Fact]
    public void PushRange_Null_DoNothing()
    {
        var stack = new SinkStack<int>(5);

        stack.Push(1);

        stack.PushRange(null);

        stack.Should().HaveCount(1);
    }

    [Fact]
    public void PushRange_NotIncrease_Adds()
    {
        var stack = new SinkStack<int>(5);

        stack.Push(5);
        stack.PushRange(1, 2, 3);

        stack.Should().HaveCount(4);
        stack.Pop().Should().Be(3);
        stack.Pop().Should().Be(2);
        stack.Pop().Should().Be(1);
        stack.Pop().Should().Be(5);
    }

    [Fact]
    public void PushRange_Increase_AddsAndSinksOthers()
    {
        var stack = new SinkStack<int>(2);

        stack.PushRange(1, 2, 3);

        stack.Pop().Should().Be(3);
        stack.Pop().Should().Be(2);
    }

    [Fact]
    public void Pop_Empty_Throws()
    {
        var stack = new SinkStack<int>(2);

        Assert.Throws<InvalidOperationException>(() => stack.Pop());
    }

    [Fact]
    public void Pop_ReturnsTop()
    {
        var stack = new SinkStack<int>(2);

        stack.PushRange(1, 2, 3, 4);

        stack.Pop().Should().Be(4);

        stack.Should().HaveCount(1);
    }

    [Fact]
    public void TryPop_ReturnsTop()
    {
        var stack = new SinkStack<int>(2);

        stack.PushRange(new[] { 1, 2, 3, 4 });

        stack.TryPop(out var value).Should().BeTrue();

        value.Should().Be(4);
    }

    [Fact]
    public void PopOrDefault()
    {
        var stack = new SinkStack<int>(2);

        var result = stack.PopOrDefault(99);

        result.Should().Be(99);
    }

    [Fact]
    public void Peek_Empty_Throws()
    {
        var stack = new SinkStack<int>(2);

        Assert.Throws<InvalidOperationException>(() => stack.Peek());
    }

    [Fact]
    public void Peek_Empty_ReturnsLast()
    {
        var stack = new SinkStack<int>(2);

        stack.Push(5);

        stack.Peek().Should().Be(5);
    }

    [Fact]
    public void Peek()
    {
        var stack = new SinkStack<int>(2);

        stack.Push(5);

        stack.Peek().Should().Be(5);

        stack.Count.Should().Be(1);
    }


    [Fact]
    public void PeekOrDefault()
    {
        var stack = new SinkStack<int>(2);

        var result = stack.PeekOrDefault(99);

        result.Should().Be(99);
    }

    [Fact]
    public void Contains()
    {
        var stack = new SinkStack<int>(2);

        stack.Push(5);

        stack.Contains(5).Should().BeTrue();
    }

    [Fact]
    public void ForEach()
    {
        var stack = new SinkStack<object>(10);

        stack.PushRange(1, 2, 3, 4, 5, "obj");

        var items = new List<object>();

        foreach (var item in stack)
        {
            items.Add(item);
        }

        items.Should().HaveCount(6);
    }

    [Fact]
    public void Linq1()
    {
        var stack = new SinkStack<object>(10);

        stack.PushRange(1, 2, 3, 4, 5, "obj");

        var asd = stack.Where(x => x is string).ToList();

        asd.Should().HaveCount(1);
    }

    [Fact]
    public void Linq2()
    {
        var stack = new SinkStack<object>(10);

        stack.PushRange(1, 2, 3, 4, 5, "obj");

        var asd = stack.Where(x => x is string).ToList();

        asd.Should().HaveCount(1);

        stack.Push('a');

        stack.Any(x => x is char).Should().Be(true);

        stack.Where(x => x is int).Cast<int>().Max().Should().Be(5);

        stack.Should().HaveCount(7);
    }

}