using CSharp2TS.Core.Attributes;
using CSharp2TS.Tests.Stubs.Enums;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace CSharp2TS.Tests.Stubs.Models {
    [TSInterface(IncludeMethods = true)]
    public class TestClassMethodReturnTypes {
        public int Id { get; set; }

        public int GetInt() => 0;
        public int? GetNullableInt() => null;
        public double GetDouble() => 0;
        public string GetString() => string.Empty;

        [TSNullable]
        public string GetNullableString() => string.Empty;

        public bool GetBool() => false;
        public int[] GetIntArray() => [];
        public double[] GetDoubleArray() => [];
        public string[] GetStringArray() => [];
        public bool[] GetBoolArray() => [];
        public TestEnum GetEnum() => TestEnum.Value1;
        public TestEnum? GetNullableEnum() => null;
        public TestClass2 GetClass2() => new();
        public GenericClass1<TestClass2> GetGenericClass() => new();
        public Dictionary<int, string> GetDictionary() => [];
        public IFormFile GetFile() => null!;
        public IFormFileCollection GetFileCollection() => null!;
        public JsonElement GetJson() => new();
        public DateTime GetDateTime() => DateTime.MinValue;
        public void GetVoid() { }
        public Task GetTask() => Task.CompletedTask;
        public Task<int> GetTaskInt() => Task.FromResult(0);
        public IEnumerable<int> GetEnumerableInt() => [];
    }
}
