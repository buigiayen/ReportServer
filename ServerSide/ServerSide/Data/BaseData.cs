using System;
using System.Collections.Generic;

namespace ServerSide.Data
{
    public class BaseData
    {
        public static List<Test> TestJson()
        {
            List<Test> tests = new List<Test>();
            for (int i = 0; i < 3; i++)
            {
                tests.Add(new Test { MA_XA = new Random().Next().ToString() });
            }

            return tests;
        }
        public class Test
        {
            public string MA_XA { get; set; }
            public string TEN_XA { get; set; }
            public string MA_HUYEN { get; set; }
            public string TEN_HUYEN { get; set; }
            public string MA_TINH { get; set; }
            public string TEN_TINH { get; set; }
            public string MA_QUOCGIA { get; set; }
            public string TEN_QUOCGIA { get; set; }
            public string NameRegion { get; set; }
            public string IDRegion { get; set; }
        }

    }

}
