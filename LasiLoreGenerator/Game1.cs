using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace LasiLoreGenerator
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        private RenderTarget2D CardTarget;
        private Texture2D Instructions;
        private Texture2D Background;


        private StreamReader reader;
        private StreamWriter saver;
        private List<string> CardInfo;
        private List<Card> Cards;

        private SpriteFont UsableFont20;
        private SpriteFont UsableFont24;

        private Vector2 TitlePosition;
        private Vector2 TypePosition;
        private Vector2 EffectPosition;
        private Vector2 CostPosition;
        private Vector2 PowerPosition;
        private Vector2 LifePosition;

        private Color textColor;



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            _graphics.PreferredBackBufferHeight = Instructions.Height;
            _graphics.PreferredBackBufferWidth = Instructions.Width;
            _graphics.ApplyChanges();


            TitlePosition = new Vector2(440, 90);
            TypePosition = new Vector2(440, 1016);
            EffectPosition = new Vector2(440, 1200);
            CostPosition = new Vector2(440, 1430);
            PowerPosition = new Vector2(97, 1420);
            LifePosition = new Vector2(785, 1420);


            textColor = new Color(62, 37, 92);

            


        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Instructions = Content.Load<Texture2D>("Instructions");
            Background = Content.Load<Texture2D>("Background");
            UsableFont20 = Content.Load<SpriteFont>("UsableFont20");
            UsableFont24 = Content.Load<SpriteFont>("UsableFont24");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(Keyboard.GetState().IsKeyDown(Keys.P))
            {
                ClearAllGenerated();
                SaveAllCards();
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(Instructions, new Vector2(0,0), Color.White);


            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Clears all generated cards
        /// </summary>
        private void ClearAllGenerated()
        {
            try
            {
                IEnumerable<string> oldCards = System.IO.Directory.EnumerateFiles("../../../CardOutput/");

                foreach(string file in oldCards)
                {
                    System.IO.File.Delete(file);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed To Delete Files! " + e.Message);
            }
            finally
            {
                if(saver != null)
                {
                    saver.Close();
                }
            }
        }


        private void SaveAllCards()
        {

            //Read in All Cards
            try
            {
                reader = new StreamReader("../../../CardGeneration.txt");
                CardInfo = new List<string>();
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    line = line.Substring(0, 1).ToLower() + line.Substring(1);
                    CardInfo.Add(line);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Printing Cards Failed: " + e.Message);
            }
            finally
            {
                if(reader != null)
                {
                    reader.Close();
                }
            }

            //Validate and Split Cards
            Cards = new List<Card>();
            string[] CurrentCard;

            foreach (String s in CardInfo)
            {
                CurrentCard = s.Split('|');
                if(s.StartsWith("m"))
                {
                    Cards.Add(new Card("Monster", CurrentCard[1], CurrentCard[2], CurrentCard[3], CurrentCard[4], CurrentCard[5]));
                } 
                else if(s.StartsWith("s"))
                {
                    Cards.Add(new Card("Spell", CurrentCard[1], CurrentCard[2], "N/A", "N/A", CurrentCard[3]));
                }
                else
                {
                    Debug.WriteLine("Failed To Detect Card: " + s);
                }
            }

            //Generate and Save cards

            
            CardTarget = new RenderTarget2D(_graphics.GraphicsDevice, Background.Width, Background.Height, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            
            GraphicsDevice.SetRenderTarget(CardTarget);
            SpriteBatch sb = new SpriteBatch(GraphicsDevice);
            

            for (int i = 0; i < Cards.Count; i++)
            {
                //Do Some MATH
                Vector2 titleMidpoint = UsableFont20.MeasureString(Cards[i].Name) / 2;
                string effect = ParseMultilineText(Cards[i].EffectText, 750, UsableFont20);
                Vector2 effectMidpoint = UsableFont20.MeasureString(effect) / 2;
                Vector2 costMidpoint = UsableFont24.MeasureString(Cards[i].Cost) / 2;
                Vector2 typeMidpoint = UsableFont20.MeasureString(Cards[i].Type) / 2;
                Vector2 powerMidpoint = UsableFont20.MeasureString(Cards[i].Power) / 2;
                Vector2 lifeMidpoint = UsableFont20.MeasureString(Cards[i].Life) / 2;

                //DRAW
                sb.Begin();

                sb.Draw(Background, Vector2.Zero, Color.White);

                sb.DrawString(UsableFont20, Cards[i].Name, TitlePosition, textColor, 0, titleMidpoint, 1.0f, SpriteEffects.None, .5f);
                sb.DrawString(UsableFont20, effect, EffectPosition, textColor, 0, effectMidpoint, 1.0f, SpriteEffects.None, .5f);
                sb.DrawString(UsableFont24, Cards[i].Cost, CostPosition, textColor, 0, costMidpoint, 1.0f, SpriteEffects.None, .5f);
                sb.DrawString(UsableFont20, Cards[i].Type, TypePosition, textColor, 0, typeMidpoint, 1.0f, SpriteEffects.None, .5f);
                sb.DrawString(UsableFont20, Cards[i].Power, PowerPosition, textColor, 0, powerMidpoint, 1.0f, SpriteEffects.None, .5f);
                sb.DrawString(UsableFont20, Cards[i].Life, LifePosition, textColor, 0, lifeMidpoint, 1.0f, SpriteEffects.None, .5f);



                sb.End();

                

                //SAVE
                try 
                {
                    saver = new StreamWriter("../../../CardOutput/" + Cards[i].Name + ".png");
                    CardTarget.SaveAsPng(saver.BaseStream, Background.Width, Background.Height);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Failed to Save Card " + Cards[i].Name + ", : " + e.Message);
                }
                finally
                {
                    if (saver != null)
                    {
                        saver.Close();
                    }
                }

                GraphicsDevice.Clear(Color.Black);

            }


            GraphicsDevice.SetRenderTarget(null);
        }//END SaveAllCards()

        /// <summary>
        /// returns a multiline (padded if actually more than 1 line) string 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxWidth"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        private string ParseMultilineText(string text, int maxWidth, SpriteFont font)
        {
            string finalResult = "";

            List<string> words = text.Split(' ').ToList();
            int currentLineLength = 0;
            string currentLine = "";

            int spaceLength = (int)font.MeasureString(" ").X;
            int totalSpacesToAdd = 0;

            while(words.Count > 0)
            {
                //measure what we are adding
                int wordLength = (int)font.MeasureString(words[0]).X;

                //add word to line if the line is empty
                if (currentLineLength == 0)
                {
                    currentLine += words[0];
                    currentLineLength += wordLength;
                    words.RemoveAt(0);
                } 
                //if it's not empty, see if it would make line too long. if not, add it.
                else if (currentLineLength + spaceLength + wordLength <= maxWidth)
                {
                    currentLine += " " + words[0];
                    currentLineLength += wordLength + spaceLength;

                    words.RemoveAt(0);
                }
                //otherwise, we need to make a new line and handle the old line. No word will be added this iteration.
                else
                {
                    //first, center align our current line. Count the number of spaces it would take to make the line go over (non-negative)
                    totalSpacesToAdd = Math.Max((maxWidth - currentLineLength) / spaceLength, 0);
                    //pad with spaces
                    for(int i = 0; i < totalSpacesToAdd/2; i++)
                    {
                        finalResult += " ";
                    }
                    //add the text and a newline
                    finalResult += currentLine + "\n";

                    //reset
                    currentLine = "";
                    currentLineLength = 0;
                }

            }

            //if there are still words left over in the current line, add them as well. Pad only if there are lines previous.
            if(finalResult != "")
            {
                totalSpacesToAdd = Math.Max((maxWidth - currentLineLength) / spaceLength, 0);
                for (int i = 0; i < totalSpacesToAdd / 2; i++)
                {
                    finalResult += " ";
                }
            }
            
            finalResult += currentLine + "\n";



            return finalResult;
        }


    }
}