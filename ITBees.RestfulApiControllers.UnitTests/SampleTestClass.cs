using NUnit.Framework;

namespace ITBees.RestfulApiControllers.UnitTests
{
    public class SampleTestClass
    {
        [Test]
        public void SampleTest()
        {
            Assert.True(true);
        }
    }

    public interface IVmWithPropertyType
    {
        void SetTargetVmType(string vmTypeName);
    }

    public class MyVmDerivedTestClassVm : Vm, IVmWithPropertyType
    {
        public string MyVmDerivedTestClassVmType { get; set; }
        public void SetTargetVmType(string vmTypeName)
        {
            MyVmDerivedTestClassVmType = vmTypeName;
        }

        public string TestProperty1 { get; set; }
        public string TestProperty2 { get; set; }
        public string TestProperty3 { get; set; }
        public string TestProperty4 { get; set; }
        public string TestProperty5 { get; set; }
        public string TestProperty6 { get; set; }
        public string TestProperty7 { get; set; }
        public string TestProperty8 { get; set; }
        public string TestProperty9 { get; set; }
        public string TestProperty10 { get; set; }
        public string TestProperty11 { get; set; }
        public string TestProperty12 { get; set; }
    }

    public class BVm : Vm
    {

    }

    public class AVm : Vm
    {

    }
    public class Vm
    {
        public string TestProperty1 { get; set; }
        public string TestProperty2 { get; set; }
        public string TestProperty3 { get; set; }
        public string TestProperty4 { get; set; }
        public string TestProperty5 { get; set; }
        public string TestProperty6 { get; set; }
        public string TestProperty7 { get; set; }
        public string TestProperty8 { get; set; }
        public string TestProperty9 { get; set; }
        public string TestProperty10 { get; set; }
        public string TestProperty11 { get; set; }
        public string TestProperty12 { get; set; }
    }
}
