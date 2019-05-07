namespace BAMCIS.AWSLambda.Common
{
    public static class Hashing
    {
        public static int Hash(params object[] args)
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;

                foreach (object item in args)
                {
                    if (item != null)
                    {
                        hash = (hash * 23) + item.GetHashCode();
                    }
                }

                return hash;
            }
        }
    }
}
