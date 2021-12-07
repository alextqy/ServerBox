using Service;
using System;

namespace Init
{
    public class Base
    {
        public Tools Tools { set; get; }

        public Base()
        {
            this.Tools = new Tools();
        }
    }
}
