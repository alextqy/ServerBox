using Service;
using System;

namespace UDP
{
    public class Base
    {
        public Tools Tools { set; get; }
        public Base()
        {
            Tools = new Tools();
        }
    }
}
