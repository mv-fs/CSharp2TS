namespace CSharp2TS.Core.Attributes {
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class TSEnumAttribute : TSAttributeBase {
        /// <summary>
        /// Generate descriptions for enum values using the Description attribute if present.
        /// </summary>
        public bool GenerateDescriptions { get; set; }
        /// <summary>
        /// Generate an array of value and description objects for the enum.
        /// </summary>
        public bool GenerateItemsArray { get; set; }
    }
}
