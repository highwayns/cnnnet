/**
 * Created with IntelliJ IDEA.
 * User: smiron86
 * Date: 6/12/13
 * Time: 8:34 PM
 */
public class Extensions {
    public static double GetDistance(int x1, int y1, int x2, int y2)
    {
        return x1 == x2 && y1 == y2
                ? 0
                : Math.sqrt(Math.pow(x1 - x2, 2) + Math.pow(y1 - y2, 2));
    }
}
