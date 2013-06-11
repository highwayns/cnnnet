import org.lwjgl.LWJGLException;
import org.lwjgl.opengl.Display;
import org.lwjgl.opengl.DisplayMode;
import org.lwjgl.opengl.GL11;
import org.newdawn.slick.Color;
import org.newdawn.slick.opengl.*;
import org.newdawn.slick.util.*;

import java.io.*;
import java.util.*;

public class NetworkDrawer {

    //region Fields

    private Network _network;
    private NeuronBase[][] _neuronPositionMap;
    private List<NeuronBase> _neurons;

    private Texture _neuronActive;
    private Texture _neuronIdle;
    private Texture _neuronInputActive;
    private Texture _neuronInputIdle;

    //endregion

    //region Methods

    public void Init() throws IOException {

        _neuronPositionMap = _network.get_neuronPositionMap();
        _neurons = _network.get_neurons();

        int width = _network.get_networkParameters().Width;
        int height = _network.get_networkParameters().Height;

        try {
            Display.setDisplayMode(new DisplayMode(width, height));
            Display.create();
            Display.setVSyncEnabled(true);
        } catch (LWJGLException e) {
            e.printStackTrace();
            System.exit(0);
        }

        GL11.glMatrixMode(GL11.GL_PROJECTION);
        GL11.glLoadIdentity();
        GL11.glOrtho(0, 800, 0, 600, 1, -1);
        GL11.glMatrixMode(GL11.GL_MODELVIEW);


        GL11.glEnable(GL11.GL_TEXTURE_2D);
        GL11.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        // enable alpha blending
        GL11.glEnable(GL11.GL_BLEND);
        GL11.glBlendFunc(GL11.GL_SRC_ALPHA, GL11.GL_ONE_MINUS_SRC_ALPHA);

        GL11.glViewport(0, 0, width, height);
        GL11.glMatrixMode(GL11.GL_MODELVIEW);

        GL11.glMatrixMode(GL11.GL_PROJECTION);
        GL11.glLoadIdentity();
        GL11.glOrtho(0, width, height, 0, 1, -1);
        GL11.glMatrixMode(GL11.GL_MODELVIEW);


        _neuronActive = TextureLoader.getTexture("PNG", ResourceLoader.getResourceAsStream("res/neuronActive.png"));
        _neuronIdle = TextureLoader.getTexture("PNG", ResourceLoader.getResourceAsStream("res/neuronIdle.png"));
        _neuronInputActive = TextureLoader.getTexture("PNG", ResourceLoader.getResourceAsStream("res/neuronInputActive.png"));
        _neuronInputIdle = TextureLoader.getTexture("PNG", ResourceLoader.getResourceAsStream("res/neuronInputIdle.png"));
    }

    public void Update() {

        int textureWidth, textureHeight, posX, posY;

        for (Iterator<NeuronBase> iterator = this._neurons.iterator(); iterator.hasNext(); ) {
            NeuronBase currentNeuron = iterator.next();

            Texture texture = currentNeuron instanceof  NeuronInput
                    ? _neuronInputIdle
                    : _neuronIdle;

            textureWidth = texture.getTextureWidth();
            textureHeight = texture.getTextureHeight();

            posX = currentNeuron.get_posX() - textureWidth / 2;
            posY = currentNeuron.get_posY() - textureHeight / 2;

            Color.white.bind();
            texture.bind();
            GL11.glBegin(GL11.GL_QUADS);
            GL11.glTexCoord2f(0, 0);
            GL11.glVertex2f(posX, posY);
            GL11.glTexCoord2f(1, 0);
            GL11.glVertex2f(posX + textureWidth, posY);
            GL11.glTexCoord2f(1, 1);
            GL11.glVertex2f(posX + textureWidth, posY + textureHeight);
            GL11.glTexCoord2f(0, 1);
            GL11.glVertex2f(posX, posY + textureHeight);
            GL11.glEnd();
        }
    }

    //endregion

    //region Instance

    public NetworkDrawer(Network network) throws IOException {
        _network = network;
    }

    //endregion
}
