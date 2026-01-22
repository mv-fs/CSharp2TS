using CSharp2TS.Core.Attributes;

namespace CSharp2TS.Tests.Stubs.Models {
    [TSInterface]
    public class ParentClass {
        public int ParentClassProperty { get; set; }
    }

    [TSInterface]
    public class ChildClass : ParentClass {
        public int ChildClassProperty { get; set; }
    }

    [TSInterface]
    public abstract class ParentClassGeneric {
        public abstract int ParentClassAbstractProperty { get; set; }
        public virtual int ParentClassVirtualProperty { get; set; }
        public virtual int ParentClassOtherVirtualProperty { get; set; }
        public int ParentClassNormalProperty { get; set; }
    }


    [TSInterface]
    public class ChildClassOverride : ParentClassGeneric {
        public override int  ParentClassAbstractProperty { get; set; }
        public override int ParentClassVirtualProperty { get; set; }
        public int ChildClassProperty { get; set; }
    }
}
