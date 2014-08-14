namespace AspNet.Identity.Oracle
{
    public static class OracleBoolDecimalExtentions
    {
        public static decimal ToDecimal(this bool target)
        {
            return (target) ? 1 : 0;
        }

        public static bool ToBool(this decimal target)
        {
            return (target == 1);
        }
    }
}
