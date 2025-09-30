#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
using CSharp2TS.Core.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace CSharp2TS.Tests.Stubs.Models {
    [TSInterface]
    public class TestClass {
        // Primitives
        public int IntProperty { get; set; }
        [TSNullable]
        public int? NullableIntProperty { get; set; }
        public long LongProperty { get; set; }
        public long? NullableLongProperty { get; set; }
        public float FloatProperty { get; set; }
        public float? NullableFloatProperty { get; set; }
        public double DoubleProperty { get; set; }
        public double? NullableDoubleProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public decimal? NullableDecimalProperty { get; set; }
        public bool BoolProperty { get; set; }
        public bool? NullableBoolProperty { get; set; }
        public char CharProperty { get; set; }
        public char? NullableCharProperty { get; set; }
        public byte ByteProperty { get; set; }
        public byte? NullableByteProperty { get; set; }
        public sbyte SByteProperty { get; set; }
        public sbyte? NullableSByteProperty { get; set; }
        public short ShortProperty { get; set; }
        public short? NullableShortProperty { get; set; }
        public ushort UShortProperty { get; set; }
        public ushort? NullableUShortProperty { get; set; }
        public uint UIntProperty { get; set; }
        public uint? NullableUIntProperty { get; set; }
        public ulong ULongProperty { get; set; }
        public ulong? NullableULongProperty { get; set; }
        public string StringProperty { get; set; }
        [TSNullable]
        public string? NullableStringProperty { get; set; }
        public Guid GuidProperty { get; set; }
        public Guid? NullableGuidProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public DateTime? NullableDateTimeProperty { get; set; }
        public DateTimeOffset DateTimeOffsetProperty { get; set; }
        public DateTimeOffset? NullableDateTimeOffsetProperty { get; set; }
        public DateOnly DateOnlyProperty { get; set; }
        public DateOnly? NullableDateOnlyProperty { get; set; }

        public TestClass ThisClass { get; set; }
        public TestClass2 Class2 { get; set; }
        public TestClassInFolder ClassInFolder { get; set; }
        public GenericClass1<int[]> GenericArray { get; set; }
        public GenericClass1<int?> NullableGeneric { get; set; }
        public GenericClass1<TestClass> GenericClass1 { get; set; }
        public GenericClass2<TestClass, TestEnum> GenericClass2 { get; set; }
        public TestEnum Enum { get; set; }
        public TestEnum? NullableEnum { get; set; }
        public TestEnumInFolder EnumInFolder { get; set; }
        public FormFile FormFile { get; set; }
        public IFormFile IFormFile { get; set; }

        // Enumerable types
        public IEnumerable<int> IntEnumerable { get; set; }
        [TSNullable]
        public IEnumerable<int> TSNullableIntEnumerable { get; set; }
        public ICollection<int> IntCollection { get; set; }
        public IList<int> IntIList { get; set; }
        public List<int> IntList { get; set; }
        public int[] IntArray { get; set; }
        public Dictionary<int, int> IntDictionary { get; set; }
        public IDictionary<int, int> IntIDictionary { get; set; }
        public IDictionary<int, int[]> IntArrayIDictionary { get; set; }
        public IDictionary<int, IList<string>> StringListIDictionary { get; set; }
        public HashSet<int> IntHashSet { get; set; }
        public ISet<int> IntISet { get; set; }
        public Queue<int> IntQueue { get; set; }
        public Stack<int> IntStack { get; set; }
        public LinkedList<int> IntLinkedList { get; set; }
        public SortedSet<int> IntSortedSet { get; set; }
        public SortedList<int, int> IntSortedList { get; set; }
        public SortedDictionary<int, int> IntSortedDictionary { get; set; }
        public IReadOnlyCollection<int> IntReadOnlyCollection { get; set; }
        public IReadOnlyList<int> IntReadOnlyList { get; set; }
        public IReadOnlyDictionary<int, int> IntReadOnlyDictionary { get; set; }
        public Collection<int> IntCollection2 { get; set; }
        public ConcurrentBag<int> IntConcurrentBag { get; set; }
        public FormFileCollection FormFileCollection { get; set; }
        public IFormFileCollection IFormFileCollection { get; set; }


        // NullableEnumerable types
        public IEnumerable<int?> NullableIntEnumerable { get; set; }
        [TSNullable]
        public IEnumerable<int?> TSNullableNullableIntEnumerable { get; set; }
        public ICollection<int?> NullableIntCollection { get; set; }
        public IList<int?> NullableIntIList { get; set; }
        public List<int?> NullableIntList { get; set; }
        public int?[] NullableIntArray { get; set; }
        public Dictionary<int, int?> NullableIntDictionary { get; set; }
        public IDictionary<int, int?> NullableIntIDictionary { get; set; }
        public HashSet<int?> NullableIntHashSet { get; set; }
        public ISet<int?> NullableIntISet { get; set; }
        public Queue<int?> NullableIntQueue { get; set; }
        public Stack<int?> NullableIntStack { get; set; }
        public LinkedList<int?> NullableIntLinkedList { get; set; }
        public SortedSet<int?> NullableIntSortedSet { get; set; }
        public SortedList<int, int?> NullableIntSortedList { get; set; }
        public SortedDictionary<int, int?> NullableIntSortedDictionary { get; set; }
        public IReadOnlyCollection<int?> NullableIntReadOnlyCollection { get; set; }
        public IReadOnlyList<int?> NullableIntReadOnlyList { get; set; }
        public IReadOnlyDictionary<int?, int?> NullableIntReadOnlyDictionary { get; set; }
        public Collection<int?> NullableIntCollection2 { get; set; }
        public ConcurrentBag<int?> NullableIntConcurrentBag { get; set; }
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.