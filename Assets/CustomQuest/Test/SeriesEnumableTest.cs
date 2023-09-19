using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Custom;

public class SeriesEnumableTest
{
    [Test]
    public void SeriesEnumableTestSimplePasses()
    {
        var testContainer = new TestDepth1
        (
            new TestDepth2(new TestObj(0), new TestObj(1), new TestObj(2), new TestObj(3), new TestObj(4)),
            new TestDepth2(new TestObj(5), new TestObj(6), new TestObj(7), new TestObj(8), new TestObj(9)),
            new TestDepth2(new TestObj(10), new TestObj(11), new TestObj(12), new TestObj(13), new TestObj(14))
        );

        Assert.AreEqual(15, testContainer.Flatten<TestObj>().Count);
        Assert.AreEqual(15, testContainer.Flatten(item => item.Item.IsType<TestObj>()).Count);
        Assert.AreEqual( 3, testContainer.Flatten<TestDepth2>(1).Count);
        Assert.AreEqual( 3, testContainer.Flatten(1, item => item.Item.IsType<TestDepth2>()).Count);

        var seriesEnumerator = testContainer.GetEnumerator();
        Assert.AreEqual(15, seriesEnumerator.Flatten<TestObj>().Count);
        Assert.AreEqual(15, seriesEnumerator.Flatten(item => item.Item.IsType<TestObj>()).Count);
        Assert.AreEqual( 3, seriesEnumerator.Flatten<TestDepth2>(1).Count);
        Assert.AreEqual( 3, seriesEnumerator.Flatten(1, item => item.Item.IsType<TestDepth2>()).Count);

        seriesEnumerator
            .Flatten<ISeriesEnumerator>(1, item => item.Item as SeriesEnumerator<TestObj>)
            .ForEach(enumerator => Assert.AreEqual(5, enumerator.Flatten<TestObj>().Count));
    }
}

public class TestObj 
{
    public int Number { get; }

    public TestObj(int number)
        => this.Number = number;
}

public class TestDepth1 : ISeriesEnumerable
{
    public List<TestDepth2> Items { get; }

    public int Depth => 3;

    public TestDepth1(params TestDepth2[] objs)
    {
        this.Items = objs.ToList();
    }

    public TestDepth1(IEnumerable<TestDepth2> objs)
    {
        this.Items = objs.ToList();
    }

    public ISeriesEnumerator GetEnumerator() 
        => new SeriesEnumerator<TestDepth2>(this.Items);

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    IEnumerator<CoordinateItem> IEnumerable<CoordinateItem>.GetEnumerator() => this.GetEnumerator();
}

public class TestDepth2 : ISeriesEnumerable<TestObj> 
{
    public List<TestObj> Items { get; }

    public int Depth => 2;

    public TestDepth2(params TestObj[] objs) 
    {
        this.Items = objs.ToList();
    }

    public TestDepth2(IEnumerable<TestObj> objs)
    {
        this.Items = objs.ToList();
    }

    public ISeriesEnumerator<TestObj> GetEnumerator() => new SeriesEnumerator<TestObj>(this.Items);

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    ISeriesEnumerator<TestObj> ISeriesEnumerable<TestObj>.GetEnumerator() => this.GetEnumerator();
    ISeriesEnumerator ISeriesEnumerable.GetEnumerator() => this.GetEnumerator();
    IEnumerator<CoordinateItem> IEnumerable<CoordinateItem>.GetEnumerator() => this.GetEnumerator();
    IEnumerator<CoordinateItem<TestObj>> IEnumerable<CoordinateItem<TestObj>>.GetEnumerator() => this.GetEnumerator();
}