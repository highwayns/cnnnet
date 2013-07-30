namespace cnnnet.Lib.Utils
{
    public class Point
    {
        #region Fields

        public readonly int X;
        public readonly int Y;

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format("{0}:{1}", X, Y);
        }

        #endregion

        #region Instance

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion
    }
}