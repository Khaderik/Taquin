using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Taquin
{
    public class Taquin
    {
        const int tailleTaquin = 3; //Défini la taille
        const bool taillePair = tailleTaquin % 2 == 0;
        const int nBCases = tailleTaquin * tailleTaquin;
        const int derniereCase = nBCases - 1;
        const int buttonSize = 40;
        const int buttonMargin = 3; //default = 3
        const int formEdge = 9;
        static readonly Random rnd = new Random();
        static readonly Font buttonFont = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        readonly Button[] buttons = new Button[nBCases];
        readonly int[] grille = new int[nBCases];
        readonly int[] positionOf = new int[nBCases];
        int nbCoups = 0;
        DateTime start;

        public static void Main(string[] args)
        {
            Taquin p = new Taquin();
            Form f = p.BuildForm();
            Application.Run(f);
        }

        public Taquin()
        {
            for (int i = 0; i < nBCases; i++)
            {
                grille[i] = i;
                positionOf[i] = i;
            }
        }

        Form BuildForm()
        {
            Button startButton = new Button
            {
                Font = new Font("Arial", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
                Size = new Size(86, 23),
                Location = new Point(formEdge,
                    (buttonSize + buttonMargin * 2) * tailleTaquin + buttonMargin + formEdge),
                Text = "Mélanger",
                UseVisualStyleBackColor = true
            };
            startButton.Click += (sender, e) => Melanger();

            Button ButtonResoudre = new Button
            {
                Font = new Font("Arial", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
                Size = new Size(86, 23),
                Location = new Point(
                    (startButton.Location.X + startButton.Width) + buttonMargin + formEdge,
                    (buttonSize + buttonMargin * 2) * tailleTaquin + buttonMargin + formEdge),
                Text = "Resoudre",
                UseVisualStyleBackColor = true
            };
            ButtonResoudre.Click += (sender, e) => Resoudre();


            int size = buttonSize * tailleTaquin + buttonMargin * tailleTaquin * 2 + formEdge * 2;
            Form form = new Form
            {
                Text = "Taquin",
                ClientSize = new Size(width: size, height: size + buttonMargin * 2 + startButton.Height)
            };
            form.SuspendLayout();
            for (int index = 0; index < nBCases; index++)
            {
                Button button = new Button
                {
                    Font = buttonFont,
                    Size = new Size(buttonSize, buttonSize),
                    Text = (index + 1).ToString(),
                    UseVisualStyleBackColor = true
                };
                SetLocation(button, index);
                form.Controls.Add(button);
                buttons[index] = button;
                int i = index;
                button.Click += (sender, e) => ButtonClick(i);
            }
            form.Controls.Add(startButton);
            form.Controls.Add(ButtonResoudre);
            form.ResumeLayout();
            return form;
        }

        private void Resoudre()
        {
            if (buttons[derniereCase].Visible) return;

            //


        }

        void ButtonClick(int i)
        {
            if (buttons[derniereCase].Visible) return;

            int target = positionOf[i];
            if (positionOf[i] / tailleTaquin == positionOf[derniereCase] / tailleTaquin)
            {
                while (positionOf[derniereCase] < target)
                {
                    Swap(derniereCase, grille[positionOf[derniereCase] + 1]);
                    nbCoups++;
                }
                while (positionOf[derniereCase] > target)
                {
                    Swap(derniereCase, grille[positionOf[derniereCase] - 1]);
                    nbCoups++;
                }
            }
            else if (positionOf[i] % tailleTaquin == positionOf[derniereCase] % tailleTaquin)
            {
                while (positionOf[derniereCase] < target)
                {
                    Swap(derniereCase, grille[positionOf[derniereCase] + tailleTaquin]);
                    nbCoups++;
                }
                while (positionOf[derniereCase] > target)
                {
                    Swap(derniereCase, grille[positionOf[derniereCase] - tailleTaquin]);
                    nbCoups++;
                }
            }
            if (Solved())
            {
                TimeSpan elapsed = DateTime.Now - start;
                elapsed = TimeSpan.FromSeconds(Math.Round(elapsed.TotalSeconds, 0));
                buttons[derniereCase].Visible = true;
                MessageBox.Show(string.Format("Résolu en {0} coups ({1}) ", nbCoups, elapsed));
            }
        }

        bool Solved()
        {
            return Enumerable.Range(0, nBCases - 1).All(i => positionOf[i] == i);
        }

        static void SetLocation(Button button, int index)
        {
            int row = index / tailleTaquin, column = index % tailleTaquin;
            button.Location = new Point(
                 (buttonSize + buttonMargin * 2) * column + buttonMargin + formEdge,
                (buttonSize + buttonMargin * 2) * row + buttonMargin + formEdge);
        }

        void Melanger()
        {
            for (int i = 0; i < nBCases; i++)
            {
                int r = rnd.Next(i, nBCases);
                int g = grille[r];
                grille[r] = grille[i];
                grille[i] = g;
            }
            for (int i = 0; i < nBCases; i++)
            {
                positionOf[grille[i]] = i;
                SetLocation(buttons[grille[i]], i);
            }
            if (!Solvable()) Swap(0, 1); //Swap any 2 blocks

            buttons[derniereCase].Visible = false;
            nbCoups = 0;
            start = DateTime.Now;
        }

        bool Solvable()
        {
            bool parity = true;
            for (int i = 0; i < nBCases - 2; i++)
            {
                for (int j = i + 1; j < nBCases - 1; j++)
                {
                    if (positionOf[j] < positionOf[i]) parity = !parity;
                }
            }
            if (taillePair && positionOf[derniereCase] / tailleTaquin % 2 == 0) parity = !parity;
            return parity;
        }

        void Swap(int a, int b)
        {
            Point location = buttons[a].Location;
            buttons[a].Location = buttons[b].Location;
            buttons[b].Location = location;

            int p = positionOf[a];
            positionOf[a] = positionOf[b];
            positionOf[b] = p;

            grille[positionOf[a]] = a;
            grille[positionOf[b]] = b;
        }
    }
}
