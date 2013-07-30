import org.lwjgl.*;
import org.lwjgl.opengl.*;

import java.io.IOException;

public class Main {

    //region Fields

    private final int _width = 800;
    private final int _height = 600;

    private NetworkParameters _networkParameters;
    private Network _network;
    private NetworkDrawer _networkDrawer;

    //endregion

    //region Methods

    public void Start() throws IOException {

        _network.GenerateNetwork();
        _networkDrawer.Init();

        // init OpenGL here
        while (!Display.isCloseRequested()) {
            // Clear the screen and depth buffer
            GL11.glClear(GL11.GL_COLOR_BUFFER_BIT | GL11.GL_DEPTH_BUFFER_BIT);

            _network.Process();

            _networkDrawer.Update();

            Display.update();
        }

        Display.destroy();
    }

    //endregion

    //region Entry Point

    public static void main(String[] args) throws IOException {
        System.out.println("Start");

        Main main = new Main();
        main.Start();

        System.out.println("End");
    }

    //endregion

    //region Instance

    public Main() {
        _networkParameters = new NetworkParameters();
        _networkParameters.Width = _width;
        _networkParameters.Height = _height;

        _network = new Network(_networkParameters);

        try {
            _networkDrawer = new NetworkDrawer(_network);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    //endregion
}
