using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;


namespace smoodi.mc.lib
{
    public class mclib
    {
        Process proc;
        public const string defaultFMLTweakClass = "net.minecraftforge.fml.common.launcher.FMLTweaker";
        public const string defaultFMLMainClass = "net.minecraft.launchwrapper.Launch";
        public const string defaultVanillaMainClass = "net.minecraft.client.main.Main";
        string args = "";
        string jpath = "";
        string jexe = "";
        string javaExecutable = "";
        string serverip = "";
        string serverport = "";
        bool fml = true;
        bool visibleConsole = true;
        string gameDir = null;

        public bool setJavaPath(string javapath)
        {
            if (System.IO.Directory.Exists(javapath))
            {
                jpath = javapath;
                return true;
            }
            else
                return false;
        }
        public void setVisibleConsole(bool visible)
        {
            visibleConsole = visible;
            if (!visible)
                javaExecutable = jpath + @"\javaw.exe";
            else
                javaExecutable = jpath + @"\java.exe";
        }
        public void configMinecraftStartManuallyExt(string arguments)
        {
            args = arguments;
        }
        public bool setJavaExecutableExt(string javaexe)
        {
            if (System.IO.File.Exists(jpath + @"\" + javaexe))
            {
                javaExecutable = jpath + @"\" + javaexe;
                jexe = javaexe;
                return true;
            }
            else return false;
        }
        public void setForgeModLoader(bool isFML)
        {
            fml = isFML;
        }
        public void setServerAutoConnect(string autoconnectserverip, string autoconnectserverport)
        {
            serverip = autoconnectserverip;
            serverport = autoconnectserverport;
        }

        public string configMinecraftStart(string gamedir, string tweakClass, string mainClass, string accessToken, string nativesFolder, string assetsDir, string version, string playername, string uuid, string assetIndex, string mainFile, string userProperties, string minimumRAM, string maximumRAM)
        {
            this.gameDir = gamedir;

            //LET'S ESCAPE SPACES IN OUR GAMEDIR 
            if (!gamedir.EndsWith(@"\")) gamedir += @"\";

            if (fml)
            {
                args = " \"-Djava.library.path=" + nativesFolder + "\" \"-Dorg.lwjgl.librarypath=" + nativesFolder + "\" -Xms" + minimumRAM + " -Xmx" + maximumRAM +" -cp " + '"' + gamedir + mainFile + ";"
                /*
                                + gamedir + @"libraries\lzma\lzma\0.0.1\lzma-0.0.1.jar;" + gamedir + @"libraries\net\minecraft\launchwrapper\1.12\launchwrapper-1.12.jar;"
                                + gamedir + @"libraries\org\ow2\asm\asm-debug-all\5.2\asm-debug-all-5.2.jar;"
                                + gamedir + @"libraries\com\typesafe\akka\akka-actor_2.11\2.3.3\akka-actor_2.11-2.3.3.jar;"
                                + gamedir + @"libraries\com\typesafe\config\1.2.1\config-1.2.1.jar;" + gamedir + @"libraries\org\scala-lang\scala-actors-migration_2.11\1.1.0\scala-actors-migration_2.11-1.1.0.jar;"
                                + gamedir + @"libraries\org\scala-lang\scala-compiler\2.11.1\scala-compiler-2.11.1.jar;" + gamedir
                                + @"libraries\org\scala-lang\plugins\scala-continuations-library_2.11\1.0.2_mc\scala-continuations-plugin_2.11-1.0.2_mc.jar;"
                                + gamedir + @"libraries\org\scala-lang\plugins\scala-continuations-plugin_2.11.1\1.0.2_mc\scala-continuations-plugin_2.11.1-1.0.2_mc.jar;"
                                + gamedir + @"libraries\org\scala-lang\scala-library\2.11.1\scala-library-2.11.1.jar;"
                                + gamedir + @"libraries\org\scala-lang\scala-parser-combinators_2.11\1.0.1\scala-parser-combinators_2.11-1.0.1.jar;"
                                + gamedir + @"libraries\org\scala-lang\scala-reflect\2.11.1\scala-reflect-2.11.1.jar;" + gamedir
                                + @"libraries\org\scala-lang\scala-swing_2.11\1.0.1\scala-swing_2.11-1.0.1.jar;" + gamedir + @"libraries\org\scala-lang\scala-xml_2.11\1.0.2\scala-xml_2.11-1.0.2.jar;"
                                + gamedir + @"libraries\org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar;" + gamedir + @"libraries\commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar;"
                                + gamedir + @"libraries\org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar;" + gamedir + @"libraries\org\lwjgl\lwjgl\lwjgl_util\2.9.4-nightly-20150209\lwjgl_util-2.9.4-nightly-20150209.jar;"
                                + gamedir + @"libraries\net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar;" + gamedir + @"libraries\com\paulscode\soundsystem\20120107\soundsystem-20120107.jar;" + gamedir
                                + @"libraries\com\paulscode\codecjorbis\20101023\codecjorbis-20101023.jar;" + gamedir + @"libraries\com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar;"
                                + gamedir + @"libraries\com\paulscode\librarylwjglopenal\20100824\librarylwjglopenal-20100824.jar;" + gamedir + @"libraries\net\sf\jopt-simple\jopt-simple\5.0.3\jopt-simple-5.0.3.jar;"
                                + gamedir + @"libraries\com\google\guava\guava\21.0\guava-21.0.jar;" + gamedir + @"libraries\com\google\code\gson\gson\2.8.0\gson-2.8.0.jar;" + gamedir
                                + @"libraries\org\apache\logging\log4j\log4j-api\2.8.1\log4j-api-2.8.1.jar;" + gamedir + @"libraries\org\apache\logging\log4j\log4j-core\2.8.1\log4j-core-2.8.1.jar;"
                                + gamedir + @"libraries\org\apache\commons\commons-lang3\3.5\commons-lang3-3.5.jar;" + gamedir + @"libraries\org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar;"
                                + gamedir + @"libraries\commons-io\commons-io\2.5\commons-io-2.5.jar;" + gamedir + @"libraries\commons-codec\commons-codec\1.10\commons-codec-1.10.jar;"
                                 //+ gamedir + @"libraries\com\mojang\netty\1.6\netty-1.6.jar;"
                                 + gamedir + @"libraries\com\mojang\realms\1.10.22\realms-1.10.22.jar;" + gamedir + @"libraries\net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar;" +
                                gamedir + @"libraries\io\netty\netty-all\4.1.9.Final\netty-all-4.1.9.Final.jar;" + gamedir + @"libraries\com\mojang\authlib\1.5.25\authlib-1.5.25.jar;" + gamedir +
                                @"libraries\org\lwjgl\lwjgl\lwjgl\2.9.4-nightly-20150209\lwjgl-2.9.4-nightly-20150209.jar;" + gamedir + @"libraries\org\lwjgl\lwjgl\lwjgl_util\2.9.4-nightly-20150209\lwjgl_util-2.9.4-nightly-20150209.jar;" + gamedir +
                                @"libraries\java3d\vecmath\1.5.2\vecmath-1.5.2.jar;" + gamedir + @"libraries\net\sf\trove4j\trove4j\3.0.3\trove4j-3.0.3.jar;" + gamedir +
                                @"libraries\com\paulscode\codecwav\20101023\codecwav-20101023.jar;" + gamedir + @"libraries\com\paulscode\libraryjavasound\20101123\libraryjavasound-20101123.jar;" +


                                gamedir + @"libraries\com\mojang\patchy\1.1\patchy-1.1.jar;" +
                                gamedir + @"libraries\oshi-project\oshi-core\1.1\oshi-core-1.1.jar;" +
                                gamedir + @"libraries\net\java\dev\jna\jna\4.4.0\jna-4.4.0.jar;" +
                                gamedir + @"libraries\net\java\dev\jna\platform\3.4.0\platform-3.4.0.jar;" +
                                gamedir + @"libraries\org\lwjgl\lwjgl\lwjgl-platform\2.9.4-nightly-20150209\lwjgl-platform-2.9.4-nightly-20150209-natives-windows.jar;" +
                                gamedir + @"libraries\net\java\jinput\jinput-platform\2.0.5\jinput-platform-2.0.5-natives-windows.jar" +
                                gamedir + @"libraries\com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar" +
                                gamedir + @"libraries\it\unimi\dsi\fastutil\7.1.0\fastutil-7.1.0.jar" +
                                gamedir + @"libraries\com\mojang\text2speech\1.10.3\text2speech-1.10.3.jar"+
                                gamedir + @"libraries\com\mojang\text2speech\1.10.3\text2speech-1.10.3-natives-windows.jar" +

                                gamedir + @"libraries\com\paulscode\librarylwjglopenal\20100824\librarylwjglopenal-20100824.jar" + '"' +
                                */

                + gamedir + @"libraries\com\google\code\gson\gson\2.8.0\gson-2.8.0.jar;" +
gamedir + @"libraries\com\google\guava\guava\21.0\guava-21.0.jar;" +
gamedir + @"libraries\com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar;" +
gamedir + @"libraries\com\mojang\authlib\1.5.25\authlib-1.5.25.jar;" +
gamedir + @"libraries\com\mojang\patchy\1.1\patchy-1.1.jar;" +
gamedir + @"libraries\com\mojang\realms\1.10.22\realms-1.10.22.jar;" +
gamedir + @"libraries\com\mojang\text2speech\1.10.3\text2speech-1.10.3-natives-windows.jar;" +
gamedir + @"libraries\com\mojang\text2speech\1.10.3\text2speech-1.10.3.jar;" +
gamedir + @"libraries\com\paulscode\codecjorbis\20101023\codecjorbis-20101023.jar;" +
gamedir + @"libraries\com\paulscode\codecwav\20101023\codecwav-20101023.jar;" +
gamedir + @"libraries\com\paulscode\libraryjavasound\20101123\libraryjavasound-20101123.jar;" +
gamedir + @"libraries\com\paulscode\librarylwjglopenal\20100824\librarylwjglopenal-20100824.jar;" +
gamedir + @"libraries\com\paulscode\soundsystem\20120107\soundsystem-20120107.jar;" +
gamedir + @"libraries\com\typesafe\akka\akka-actor_2.11\2.3.3\akka-actor_2.11-2.3.3.jar;" +
gamedir + @"libraries\com\typesafe\config\1.2.1\config-1.2.1.jar;" +
gamedir + @"libraries\commons-codec\commons-codec\1.10\commons-codec-1.10.jar;" +
gamedir + @"libraries\commons-io\commons-io\2.5\commons-io-2.5.jar;" +
gamedir + @"libraries\commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar;" +
gamedir + @"libraries\io\netty\netty-all\4.1.9.Final\netty-all-4.1.9.Final.jar;" +
gamedir + @"libraries\it\unimi\dsi\fastutil\7.1.0\fastutil-7.1.0.jar;" +
gamedir + @"libraries\java3d\vecmath\1.5.2\vecmath-1.5.2.jar;" +
gamedir + @"libraries\lzma\lzma\0.0.1\lzma-0.0.1.jar;" +
gamedir + @"libraries\net\java\dev\jna\jna\4.4.0\jna-4.4.0.jar;" +
gamedir + @"libraries\net\java\dev\jna\platform\3.4.0\platform-3.4.0.jar;" +
gamedir + @"libraries\net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar;" +
gamedir + @"libraries\net\java\jinput\jinput-platform\2.0.5\jinput-platform-2.0.5-natives-windows.jar;" +
gamedir + @"libraries\net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar;" +
gamedir + @"libraries\net\minecraft\launchwrapper\1.12\launchwrapper-1.12.jar;" +
gamedir + @"libraries\net\minecraftforge\forge\1.12.2-14.23.5.2854\forge-1.12.2-14.23.5.2854.jar;" +
gamedir + @"libraries\net\sf\jopt-simple\jopt-simple\5.0.3\jopt-simple-5.0.3.jar;" +
gamedir + @"libraries\net\sf\trove4j\trove4j\3.0.3\trove4j-3.0.3.jar;" +
gamedir + @"libraries\org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar;" +
gamedir + @"libraries\org\apache\commons\commons-lang3\3.5\commons-lang3-3.5.jar;" +
gamedir + @"libraries\org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar;" +
gamedir + @"libraries\org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar;" +
gamedir + @"libraries\org\apache\logging\log4j\log4j-api\2.8.1\log4j-api-2.8.1.jar;" +
gamedir + @"libraries\org\apache\logging\log4j\log4j-core\2.8.1\log4j-core-2.8.1.jar;" +
gamedir + @"libraries\org\apache\maven\maven-artifact\3.5.3\maven-artifact-3.5.3.jar;" +
gamedir + @"libraries\org\jline\jline\3.5.1\jline-3.5.1.jar;" +
gamedir + @"libraries\org\lwjgl\lwjgl\lwjgl\2.9.4-nightly-20150209\lwjgl-2.9.4-nightly-20150209.jar;" +
gamedir + @"libraries\org\lwjgl\lwjgl\lwjgl-platform\2.9.4-nightly-20150209\lwjgl-platform-2.9.4-nightly-20150209-natives-windows.jar;" +
gamedir + @"libraries\org\lwjgl\lwjgl\lwjgl_util\2.9.4-nightly-20150209\lwjgl_util-2.9.4-nightly-20150209.jar;" +
gamedir + @"libraries\org\ow2\asm\asm-debug-all\5.2\asm-debug-all-5.2.jar;" +
gamedir + @"libraries\org\scala-lang\plugins\scala-continuations-library_2.11\1.0.2_mc\scala-continuations-library_2.11-1.0.2_mc.jar;" +
gamedir + @"libraries\org\scala-lang\plugins\scala-continuations-plugin_2.11.1\1.0.2_mc\scala-continuations-plugin_2.11.1-1.0.2_mc.jar;" +
gamedir + @"libraries\org\scala-lang\scala-actors-migration_2.11\1.1.0\scala-actors-migration_2.11-1.1.0.jar;" +
gamedir + @"libraries\org\scala-lang\scala-compiler\2.11.1\scala-compiler-2.11.1.jar;" +
gamedir + @"libraries\org\scala-lang\scala-library\2.11.1\scala-library-2.11.1.jar;" +
gamedir + @"libraries\org\scala-lang\scala-parser-combinators_2.11\1.0.1\scala-parser-combinators_2.11-1.0.1.jar;" +
gamedir + @"libraries\org\scala-lang\scala-reflect\2.11.1\scala-reflect-2.11.1.jar;" +
gamedir + @"libraries\org\scala-lang\scala-swing_2.11\1.0.1\scala-swing_2.11-1.0.1.jar;" +
gamedir + @"libraries\org\scala-lang\scala-xml_2.11\1.0.2\scala-xml_2.11-1.0.2.jar;" +
gamedir + @"libraries\oshi-project\oshi-core\1.1\oshi-core-1.1.jar;" +'"' + ' '
+ mainClass + " --username " + playername + " --version " + version + " --gameDir " + gamedir + " --assetsDir " + assetsDir + " --assetIndex 1.12" +
                " --uuid " + uuid + " --accessToken " + accessToken + " --userType mojang" + " --tweakClass " + tweakClass + " --versionType Forge";

                if (serverip != "")
                    args += " --server " + serverip;
                if (serverport != "")
                    args += " --port " + serverport;
                return args;
            }
            return args;
        }

        public Process runMinecraft()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.Arguments = args;
            processInfo.FileName = javaExecutable;
            processInfo.WorkingDirectory = gameDir;

            proc = Process.Start(processInfo);
            return proc;
        }
        public Process getMinecraftProcess()
        {
            return proc;
        }
        public string getCurrentJavaPath()
        {
            return jpath;
        }
        public string getCurrentJavaExecutable()
        {
            return jexe;
        }
        public string getCurrentJavaExecutableFullPath()
        {
            return javaExecutable;
        }
        public string getArguments()
        {
            return args;
        }
        public bool getIsFML()
        {
            return fml;
        }
        public bool getIsVisibleConsole()
        {
            return visibleConsole;
        }
        public string getServerAutoConnectServerIP()
        {
            return serverip;
        }
        public string getServerAutoConnectServerPort()
        {
            return serverport;
        }
    }
}
