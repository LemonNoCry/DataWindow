using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataWindow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataWindow.CustomPropertys;
using DataWindow.Serialization;

namespace DataWindow.Core.Tests
{
    [TestClass()]
    public class CollectionsTests
    {
        public class Controlss : ControlSerializable
        {
            public string NName { get; set; }
            public override CustomPropertyCollection GetCollections(Control control)
            {
                Console.WriteLine(1);
                return null;
            }
        }

        [TestMethod()]
        public void ControlConvertSerializableTest()
        {
            //验证子类
            Control con = new Control();
            var cs = Collections.ControlConvertSerializable(con);

            Console.WriteLine(cs);
        }
    }
}