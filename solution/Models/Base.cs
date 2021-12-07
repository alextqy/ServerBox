using Service;

namespace Models
{
    public class Base
    {
        public DbContentCore DbContent;
        public Tools Tools { set; get; }

        public Base() { }

        public Base(DbContentCore DbContent)
        {
            this.DbContent = DbContent;
            this.Tools = new Tools();
        }
    }
}
