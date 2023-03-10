using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nelibur.ObjectMapper;
using NUnit.Framework;

namespace ITBees.RestfulApiControllers.UnitTests
{
    public class InheritedTypeMapperTests
    {
        [Test]
        public void InheritedTypeMapper_MapAndModify_shouldMapOneListToAnotherWithSettingDeriveClassTypeProperty()
        {
            var vms = new List<Vm>()
            {
                new AVm(), new BVm(), new BVm()
            };
            var stopwatch = new Stopwatch();
            TinyMapper.Bind<Vm, MyVmDerivedTestClassVm>();
            TinyMapper.Bind<AVm, MyVmDerivedTestClassVm>();
            TinyMapper.Bind<BVm, MyVmDerivedTestClassVm>();
            TinyMapper.Bind<List<Vm>, List<MyVmDerivedTestClassVm>>();

           
            stopwatch.Start();
            List<MyVmDerivedTestClassVm> myVmDerivedTestClassVms = null;
            InheritedTypeMapper.MapAndModify(vms, ref myVmDerivedTestClassVms);
            stopwatch.Stop();
            foreach (var myVmDerivedTestClassVm in myVmDerivedTestClassVms)
            {
                Console.WriteLine(myVmDerivedTestClassVm.MyVmDerivedTestClassVmType);
            }

            Assert.True(myVmDerivedTestClassVms.First().MyVmDerivedTestClassVmType == "AVm");
            Assert.True(myVmDerivedTestClassVms.Last().MyVmDerivedTestClassVmType == "BVm");
            Console.WriteLine($"Method take : {stopwatch.ElapsedTicks} ticks");
        }

        [Test]
        public void InheritedTypeMapper_MapAndModifyUsingReflection_shouldMapOneListToAnotherWithSettingDeriveClassTypeProperty()
        {
            var vms = new List<Vm>()
            {
                new AVm(), new BVm(), new BVm()
            };
            var stopwatch = new Stopwatch();
            TinyMapper.Bind<Vm, MyVmDerivedTestClassVm>();
            TinyMapper.Bind<AVm, MyVmDerivedTestClassVm>();
            TinyMapper.Bind<BVm, MyVmDerivedTestClassVm>();
            TinyMapper.Bind<List<Vm>, List<MyVmDerivedTestClassVm>>();

            stopwatch.Start();
            List<MyVmDerivedTestClassVm> myVmDerivedTestClassVms = null;
            InheritedTypeMapper.MapAndModifyUsingReflection(vms, ref myVmDerivedTestClassVms);
            stopwatch.Stop();
            foreach (var myVmDerivedTestClassVm in myVmDerivedTestClassVms)
            {
                Console.WriteLine(myVmDerivedTestClassVm.MyVmDerivedTestClassVmType);
            }

            Assert.True(myVmDerivedTestClassVms.First().MyVmDerivedTestClassVmType == "AVm");
            Assert.True(myVmDerivedTestClassVms.Last().MyVmDerivedTestClassVmType == "BVm");
            Console.WriteLine($"Method take : {stopwatch.ElapsedTicks} ticks");
        }

    }
}