namespace Models
{
    public class Base
    {
        public DbContentCore DbContent;
        public Base() { }
        public Base(DbContentCore DbContent)
        {
            this.DbContent = DbContent;
        }
    }
}
