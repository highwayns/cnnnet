import org.lwjgl.LWJGLException;
import org.lwjgl.opengl.*;
import org.lwjgl.util.glu.GLU;
import org.newdawn.slick.opengl.*;
import org.newdawn.slick.util.*;

import java.io.*;
import java.nio.ByteBuffer;
import java.util.*;

import static org.lwjgl.opengl.EXTFramebufferObject.*;
import static org.lwjgl.opengl.GL11.*;

public class NetworkDrawer {





    float angle;
    int colorTextureID;
    int framebufferID;
    int depthRenderBufferID;








    //region Fields

    private Network _network;
    private NeuronBase[][] _neuronPositionMap;
    private List<NeuronBase> _neurons;

    private Texture _neuronActive;
    private Texture _neuronIdle;
    private Texture _neuronInputActive;
    private Texture _neuronInputIdle;

    private ByteBuffer _pixels;

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











        glViewport(0, 0, 512, 512);                                // Reset The Current Viewport
        glMatrixMode(GL_PROJECTION);                               // Select The Projection Matrix
        glLoadIdentity();                                          // Reset The Projection Matrix
        GLU.gluPerspective(45.0f, 512f / 512f, 1.0f, 100.0f);        // Calculate The Aspect Ratio Of The Window
        glMatrixMode(GL_MODELVIEW);                                // Select The Modelview Matrix
        glLoadIdentity ();                                          // Reset The Modelview Matrix

        // Start Of User Initialization
        angle       = 0.0f;                                         // Set Starting Angle To Zero

        glClearColor(0.0f, 0.0f, 0.0f, 0.5f);                      // Black Background
        glClearDepth(1.0f);                                        // Depth Buffer Setup
        glDepthFunc(GL_LEQUAL);                                    // The Type Of Depth Testing (Less Or Equal)
        glEnable(GL_DEPTH_TEST);                                   // Enable Depth Testing
        glShadeModel(GL_SMOOTH);                                   // Select Smooth Shading
        glHint (GL_PERSPECTIVE_CORRECTION_HINT, GL_NICEST);         // Set Perspective Calculations To Most Accurate

        // check if GL_EXT_framebuffer_object can be use on this system
        if (!GLContext.getCapabilities().GL_EXT_framebuffer_object) {
            System.out.println("FBO not supported!!!");
            System.exit(0);
        }
        else {
            System.out.println("FBO is supported!!!");
            // init our fbo
            framebufferID = glGenFramebuffersEXT();                                         // create a new framebuffer
            colorTextureID = glGenTextures();                                               // and a new texture used as a color buffer
            depthRenderBufferID = glGenRenderbuffersEXT();                                  // And finally a new depthbuffer
            glBindFramebufferEXT(GL_FRAMEBUFFER_EXT, framebufferID);                        // switch to the new framebuffer

            // initialize color texture
            glBindTexture(GL_TEXTURE_2D, colorTextureID);                                   // Bind the colorbuffer texture
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);               // make it linear filterd
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, 512, 512, 0, GL_RGBA, GL_INT, (java.nio.ByteBuffer) null);  // Create the texture data
            glFramebufferTexture2DEXT(GL_FRAMEBUFFER_EXT,GL_COLOR_ATTACHMENT0_EXT,GL_TEXTURE_2D, colorTextureID, 0); // attach it to the framebuffer

            // initialize depth renderbuffer
            glBindRenderbufferEXT(GL_RENDERBUFFER_EXT, depthRenderBufferID);                // bind the depth renderbuffer
            glRenderbufferStorageEXT(GL_RENDERBUFFER_EXT, GL14.GL_DEPTH_COMPONENT24, 512, 512); // get the data space for it
            glFramebufferRenderbufferEXT(GL_FRAMEBUFFER_EXT,GL_DEPTH_ATTACHMENT_EXT,GL_RENDERBUFFER_EXT, depthRenderBufferID); // bind it to the renderbuffer
            glBindFramebufferEXT(GL_FRAMEBUFFER_EXT, 0);                                    // Swithch back to normal framebuffer rendering
        }












        /*
        GL11.glMatrixMode(GL11.GL_PROJECTION);
        GL11.glLoadIdentity();
        GL11.glOrtho(0, width, 0, height, 1, -1);
        GL11.glMatrixMode(GL11.GL_MODELVIEW);

        GL11.glEnable(GL11.GL_DRAW_PIXEL_TOKEN);
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

        _pixels = ByteBuffer.allocateDirect(width*height*4);
        */

        _neuronActive = TextureLoader.getTexture("PNG", ResourceLoader.getResourceAsStream("res/neuronActive.png"));
        _neuronIdle = TextureLoader.getTexture("PNG", ResourceLoader.getResourceAsStream("res/neuronIdle.png"));
        _neuronInputActive = TextureLoader.getTexture("PNG", ResourceLoader.getResourceAsStream("res/neuronInputActive.png"));
        _neuronInputIdle = TextureLoader.getTexture("PNG", ResourceLoader.getResourceAsStream("res/neuronInputIdle.png"));
    }









    public void drawBox() {

        // this func just draws a perfectly normal box with some texture coordinates
        glBegin(GL_QUADS);
        // Front Face
        glTexCoord2f(0.0f, 0.0f); glVertex3f(-1.0f, -1.0f,  1.0f);  // Bottom Left Of The Texture and Quad
        glTexCoord2f(1.0f, 0.0f); glVertex3f( 1.0f, -1.0f,  1.0f);  // Bottom Right Of The Texture and Quad
        glTexCoord2f(1.0f, 1.0f); glVertex3f( 1.0f,  1.0f,  1.0f);  // Top Right Of The Texture and Quad
        glTexCoord2f(0.0f, 1.0f); glVertex3f(-1.0f,  1.0f,  1.0f);  // Top Left Of The Texture and Quad
        // Back Face
        glTexCoord2f(1.0f, 0.0f); glVertex3f(-1.0f, -1.0f, -1.0f);  // Bottom Right Of The Texture and Quad
        glTexCoord2f(1.0f, 1.0f); glVertex3f(-1.0f,  1.0f, -1.0f);  // Top Right Of The Texture and Quad
        glTexCoord2f(0.0f, 1.0f); glVertex3f( 1.0f,  1.0f, -1.0f);  // Top Left Of The Texture and Quad
        glTexCoord2f(0.0f, 0.0f); glVertex3f( 1.0f, -1.0f, -1.0f);  // Bottom Left Of The Texture and Quad
        // Top Face
        glTexCoord2f(0.0f, 1.0f); glVertex3f(-1.0f,  1.0f, -1.0f);  // Top Left Of The Texture and Quad
        glTexCoord2f(0.0f, 0.0f); glVertex3f(-1.0f,  1.0f,  1.0f);  // Bottom Left Of The Texture and Quad
        glTexCoord2f(1.0f, 0.0f); glVertex3f( 1.0f,  1.0f,  1.0f);  // Bottom Right Of The Texture and Quad
        glTexCoord2f(1.0f, 1.0f); glVertex3f( 1.0f,  1.0f, -1.0f);  // Top Right Of The Texture and Quad
        // Bottom Face
        glTexCoord2f(1.0f, 1.0f); glVertex3f(-1.0f, -1.0f, -1.0f);  // Top Right Of The Texture and Quad
        glTexCoord2f(0.0f, 1.0f); glVertex3f( 1.0f, -1.0f, -1.0f);  // Top Left Of The Texture and Quad
        glTexCoord2f(0.0f, 0.0f); glVertex3f( 1.0f, -1.0f,  1.0f);  // Bottom Left Of The Texture and Quad
        glTexCoord2f(1.0f, 0.0f); glVertex3f(-1.0f, -1.0f,  1.0f);  // Bottom Right Of The Texture and Quad
        // Right face
        glTexCoord2f(1.0f, 0.0f); glVertex3f( 1.0f, -1.0f, -1.0f);  // Bottom Right Of The Texture and Quad
        glTexCoord2f(1.0f, 1.0f); glVertex3f( 1.0f,  1.0f, -1.0f);  // Top Right Of The Texture and Quad
        glTexCoord2f(0.0f, 1.0f); glVertex3f( 1.0f,  1.0f,  1.0f);  // Top Left Of The Texture and Quad
        glTexCoord2f(0.0f, 0.0f); glVertex3f( 1.0f, -1.0f,  1.0f);  // Bottom Left Of The Texture and Quad
        // Left Face
        glTexCoord2f(0.0f, 0.0f); glVertex3f(-1.0f, -1.0f, -1.0f);  // Bottom Left Of The Texture and Quad
        glTexCoord2f(1.0f, 0.0f); glVertex3f(-1.0f, -1.0f,  1.0f);  // Bottom Right Of The Texture and Quad
        glTexCoord2f(1.0f, 1.0f); glVertex3f(-1.0f,  1.0f,  1.0f);  // Top Right Of The Texture and Quad
        glTexCoord2f(0.0f, 1.0f); glVertex3f(-1.0f,  1.0f, -1.0f);  // Top Left Of The Texture and Quad
        glEnd();
    }










    public void Update() {

        Display.sync(30);

        int width = _network.get_networkParameters().Width;
        int height = _network.get_networkParameters().Height;











        // FBO render pass
        glViewport (0, 0, 512, 512);                                    // set The Current Viewport to the fbo size
        glBindTexture(GL_TEXTURE_2D, 0);                                // unlink textures because if we dont it all is gonna fail
        glBindFramebufferEXT(GL_FRAMEBUFFER_EXT, framebufferID);        // switch to rendering on our FBO
        glClearColor (1.0f, 0.0f, 0.0f, 0.5f);
        glClear (GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);            // Clear Screen And Depth Buffer on the fbo to red
        glLoadIdentity ();                                              // Reset The Modelview Matrix
        glTranslatef (0.0f, 0.0f, -6.0f);                               // Translate 6 Units Into The Screen and then rotate
        glRotatef(angle,0.0f,1.0f,0.0f);
        glRotatef(angle,1.0f,0.0f,0.0f);
        glRotatef(angle,0.0f,0.0f,1.0f);

        glColor3f(1,1,0);                                               // set color to yellow
        drawBox();                                                      // draw the box

        // Normal render pass, draw cube with texture
        glEnable(GL_TEXTURE_2D);                                        // enable texturing
        glBindFramebufferEXT(GL_FRAMEBUFFER_EXT, 0);                    // switch to rendering on the framebuffer

        glClearColor (0.0f, 1.0f, 0.0f, 0.5f);
        glClear (GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);            // Clear Screen And Depth Buffer on the framebuffer to black

        glBindTexture(GL_TEXTURE_2D, colorTextureID);                   // bind our FBO texture

        glViewport (0, 0, 512, 512);                                    // set The Current Viewport

        glLoadIdentity ();                                              // Reset The Modelview Matrix
        glTranslatef (0.0f, 0.0f, -6.0f);                               // Translate 6 Units Into The Screen and then rotate
        glRotatef(angle,0.0f,1.0f,0.0f);
        glRotatef(angle,1.0f,0.0f,0.0f);
        glRotatef(angle,0.0f,0.0f,1.0f);
        glColor3f(1,1,1);                                               // set the color to white
        drawBox();                                                      // draw the box

        glDisable(GL_TEXTURE_2D);
        glFlush ();






















        /*
        IntBuffer buffer = ByteBuffer.allocateDirect(1*4).order(ByteOrder.nativeOrder()).asIntBuffer(); // allocate a 1 int byte buffer
        EXTFramebufferObject.glGenFramebuffersEXT(buffer); // generate
        int myFBOId = buffer.get();

        EXTFramebufferObject.glBindFramebufferEXT( EXTFramebufferObject.GL_FRAMEBUFFER_EXT, myFBOId );
        EXTFramebufferObject.glFramebufferTexture2DEXT( EXTFramebufferObject.GL_FRAMEBUFFER_EXT, EXTFramebufferObject.GL_COLOR_ATTACHMENT0_EXT,
        GL11.GL_TEXTURE_2D, myTextureID, 0);

        */




        /*
        byte[] desirabilityMap = new byte[width*height*4];

        for(int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                desirabilityMap[y * width * 4 + x * 4 + 0] = (byte)120; // R
                desirabilityMap[y * width * 4 + x * 4 + 1] = 0; // G
                desirabilityMap[y * width * 4 + x * 4 + 2] = 0; // B
                desirabilityMap[y * width * 4 + x * 4 + 3] = (byte)255; // A
            }
        }

        _pixels.put(desirabilityMap).flip();


        GL11.glClear(GL11.GL_COLOR_BUFFER_BIT | GL11.GL_DEPTH_BUFFER_BIT);
        GL11.glRasterPos2i(0, 0);

        GL11.glDrawPixels(width, height, GL11.GL_RGBA, GL11.GL_UNSIGNED_BYTE, _pixels);

        GL11.glPixelZoom(2, 2);
        Display.update();
        */

        /*
        int textureWidth, textureHeight, posX, posY;

        for (NeuronBase neuron : _neurons) {

            Texture texture = neuron instanceof  NeuronInput
                    ? _neuronInputIdle
                    : _neuronIdle;

            textureWidth = texture.getTextureWidth();
            textureHeight = texture.getTextureHeight();

            posX = neuron.get_posX() - textureWidth / 2;
            posY = neuron.get_posY() - textureHeight / 2;

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
        }*/
    }

    //endregion

    //region Instance

    public NetworkDrawer(Network network) throws IOException {
        _network = network;
    }

    //endregion
}
