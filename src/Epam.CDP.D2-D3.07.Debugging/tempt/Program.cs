using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tempt
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new Test();
            t.Method1(1);
        }
    }

    class Test
    {
        List<int> TEST_A_0;

        public void Method1(int A_0)
        {
            if (TEST_A_0 == null)
                TEST_A_0 = new List<int>();

            TEST_A_0.Add(A_0);
        }

        public void Method2(int A_0)
        {
            Method1(A_0);
        }
    }
}
