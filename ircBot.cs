using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using CommandLine.Utility;

namespace ircBot.net
{
   class IrcBot
   {

      public static string SERVER; //= "irc.mozilla.org";
      // Irc server's port (6667 is default port)
      private static int PORT = 6667;

      // Bot's nickname
      private static string NICK;// = "GISBot";

      // User information defined in RFC 2812 (Internet Relay Chat: Client Protocol) is sent to irc server
      private static string USER = "GISBot 8 * :I'm a bot written in C#";

      // Channel to join
      private static string CHANNEL;// = "#GIS-bot";

      private static string USAGE = "Usage: IRCBot.exe --server:server.name " +
      "[--port:port] --nick:nickname --channel:#channelname";

      // StreamWriter is declared here so that PingSender can access it
      public static StreamWriter writer;
      public static TcpClient irc;
      static void Main (string[] args)
      {

         NetworkStream stream;
         string inputLine;
         StreamReader reader;
         string nickname;
         string pong;
         //int iCount=0;
         //USAGE: GISBot.exe --server:irc.mozilla.org [--port:nnnn] --nick:GISBot --channel:#GIS-bot

         //get parameters from command line
         Arguments CommandLine=new Arguments(args);
         if(CommandLine["server"] != null)
            SERVER = CommandLine["server"];
         else
        {
            Console.WriteLine(USAGE);
            System.Environment.Exit(0);
        }

        if(CommandLine["nick"] != null)
            NICK = CommandLine["nick"];
        else
         {
            Console.WriteLine(USAGE);
            System.Environment.Exit(0);
         }

         if(CommandLine["port"] != null)
            PORT = int.Parse(CommandLine["port"]);
         else
         {
            Console.WriteLine("Port not defined; Using 6667");

         }

         if(CommandLine["channel"] != null)
            CHANNEL = CommandLine["channel"];
         else
         {
            Console.WriteLine(USAGE);
            System.Environment.Exit(0);
         }

   try
         {
            /*********VARIABLES***************/
            irc = new TcpClient (SERVER, PORT);
            stream = irc.GetStream ();
            reader = new StreamReader (stream);
            writer = new StreamWriter (stream);
            Random rnd = new Random();
            string query;
            string formattedQuery;
            int iRnd;
            string randImgURL = "NOTHING";
            //SearchResponse response;
            /*********VARIABLES***************/

            Console.WriteLine("Sending NICK -> " + NICK);
            writer.WriteLine ("NICK " + NICK);
            writer.Flush ();
            Thread.Sleep(100);
            Console.WriteLine("Sending USER -> " + USER);
            writer.WriteLine ("USER " + USER);
            writer.Flush ();
            Thread.Sleep(100);
            Console.WriteLine("Sending JOIN -> " + CHANNEL);
            writer.WriteLine ("JOIN " + CHANNEL);
            writer.Flush ();
            Thread.Sleep(100);

            //the main message loop
            while (true)
            {
               while ( (inputLine = reader.ReadLine () ) != null )
               {
                  Console.WriteLine(inputLine);
                  if (inputLine.EndsWith ("JOIN :" + CHANNEL) )
                  {
                     // Parse nickname of person who joined the channel
                     nickname = inputLine.Substring(1, inputLine.IndexOf ("!") - 1);

                     // Welcome the nickname to channel by sending a notice
                     writer.WriteLine ("NOTICE " + nickname + " :Hi " + nickname
                     + ". \"!GIS:<query>\" gets you a random Google Image (possibly NSFW).");
                     writer.Flush ();
                     // Sleep to prevent excess flood
                     Thread.Sleep (2000);
                  }

                  if (inputLine.StartsWith ("PING"))
                  {
                     // Parse PING
                     pong = inputLine.Substring(inputLine.IndexOf (":"),(inputLine.Length-inputLine.IndexOf (":")));

                     // Welcome the nickname to channel by sending a notice
                     Console.WriteLine ("PONG " + pong);
                     writer.WriteLine ("PONG " + pong);
                     writer.Flush ();
                     writer.WriteLine ("JOIN " + CHANNEL);
                     writer.Flush ();

                  }

                  if (inputLine.IndexOf("!bot:")> -1)
                  {
query = inputLine.Substring(inputLine.IndexOf ("!bot:")+5,(inputLine.Length-(inputLine.IndexOf ("!bot:")+5)));
//query = inputLine.Substring(inputLine.IndexOf ("!:")+3,inputLine.Length-(inputLine.IndexOf ("!:")+3);
                     //do something with query here
                     writer.WriteLine("NOTICE " + CHANNEL + " : You said: " + query);
                     writer.Flush();

                  }
               }

               writer.Close ();
               reader.Close ();
               irc.Close ();
            }
         }
         catch (Exception e)
         {
            // Show the exception, sleep for a while and try to establish a new connection to irc server
            Console.WriteLine (e.ToString () );
            Thread.Sleep (5000);
            string[] argv = { };
            irc.Close();
            Main (argv);
         }
      }
   }


}
