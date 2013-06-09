import org.lwjgl.opengl.*;

public class DrawingUtility {

    //region Fields
    private static int _drawCircleSlices = 360;
    //endregion

    //region Methods

    public static void DrawCircle(int posX, int posY, float radius, float red, float green, float blue)
    {
        GL11.glColor3f(red, green,blue);
        GL11.glBegin(GL11.GL_LINE_LOOP);

        for (int i=0; i < _drawCircleSlices; i++)
        {
            double angle = 2 * Math.PI * i / _drawCircleSlices;
            GL11.glVertex2d(posX + radius * Math.cos(angle), posY + radius * Math.sin(angle));
        }

        GL11.glEnd();
    }

    //endregion

}
