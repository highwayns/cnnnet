import org.lwjgl.*;
import org.lwjgl.opengl.*;

public class Main {

    //region Fields

    private final int _width = 800;
    private final int _height = 600;

    private Network _network;
    private NetworkDrawer _networkDrawer;

    //endregion

    //region Methods

    public void Start() {

        _network.GenerateNetwork();
        _network.Start();

        try {
            Display.setDisplayMode(new DisplayMode(_width, _height));
            Display.create();
        } catch (LWJGLException e) {
            e.printStackTrace();
            System.exit(0);
        }

        GL11.glMatrixMode(GL11.GL_PROJECTION);
        GL11.glLoadIdentity();
        GL11.glOrtho(0, 800, 0, 600, 1, -1);
        GL11.glMatrixMode(GL11.GL_MODELVIEW);

        // init OpenGL here
        while (!Display.isCloseRequested()) {
            // Clear the screen and depth buffer
            GL11.glClear(GL11.GL_COLOR_BUFFER_BIT | GL11.GL_DEPTH_BUFFER_BIT);
            _networkDrawer.Update();
            Display.update();
        }

        Display.destroy();
    }

    //endregion

    //region Entry Point

    public static void main(String[] args) {
        System.out.println("Start");

        Main main = new Main();
        main.Start();

        System.out.println("End");
    }

    //endregion

    //region Instance

    public Main() {
        _network = new Network(_width, _height);
        _networkDrawer = new NetworkDrawer(_network);
    }

    //endregion
}
