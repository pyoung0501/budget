namespace BTQLib.Util
{
    /// <summary>
    /// A singleton tuple.
    /// </summary>
    /// <typeparam name="T1">Type of the first item.</typeparam>
    public class Tuple<T1>
    {
        /// <summary>
        /// The first item.
        /// </summary>
        public T1 _item1;

        /// <summary>
        /// The first item.
        /// </summary>
        public T1 Item1 { get { return _item1; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="item1">The first item.</param>
        public Tuple(T1 item1)
        {
            _item1 = item1;
        }
    }

    /// <summary>
    /// A [] tuple.
    /// </summary>
    /// <typeparam name="T1">Type of the first item.</typeparam>
    /// <typeparam name="T2">Type fo the second item.</typeparam>
    public class Tuple<T1, T2>
    {
        /// <summary>
        /// The first item.
        /// </summary>
        public T1 _item1;

        /// <summary>
        /// The second item.
        /// </summary>
        public T2 _item2;

        /// <summary>
        /// The first item.
        /// </summary>
        public T1 Item1 { get { return _item1; } }

        /// <summary>
        /// The second item.
        /// </summary>
        public T2 Item2 { get { return _item2; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="item1">The first item.</param>
        /// <param name="item2">The second item.</param>
        public Tuple(T1 item1, T2 item2)
        {
            _item1 = item1;
            _item2 = item2;
        }
    }
}
