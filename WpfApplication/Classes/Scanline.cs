using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfApplication
{
    public class Scanline
    {
        public void Sweep(List<Point> points)
        {
            var windows = (MainWindow)Application.Current.MainWindow;
            //Encontra o bounding box para y e os pontos críticos (mínimos em y).
            int y_min = int.MaxValue;
            int y_max = int.MinValue;
            List<CriticalP> criticals = new List<CriticalP>();
            for (int i = 0; i < points.Count(); i++)
            {
                if (points[i].Y < y_min)
                {
                    y_min = points[i].Y;
                }
                else if (points[i].Y > y_max)
                {
                    y_max = points[i].Y;
                }

                //Encontrar os pontos críticos.
                Point p_aux = points[(i + 1) % points.Count()];
                if (points[i].Y < p_aux.Y)
                {
                    criticals.Add(new CriticalP(
                        i,
                        1,
                        points[i].X,
                        (p_aux.X - points[i].X * 1.0f) / (p_aux.Y - points[i].Y * 1.0f)
                    ));
                }
                p_aux = points[(i - 1 + points.Count()) % points.Count()];
                if (points[i].Y < p_aux.Y)
                {
                    criticals.Add(new CriticalP(
                        i,
                        -1,
                        points[i].X,
                        (p_aux.X - points[i].X * 1.0f) / (p_aux.Y - points[i].Y * 1.0f)
                    ));
                }
            }

            //Início do algoritmo de varredura - Loop de varredura
            List<CriticalP> active_criticalPs = new List<CriticalP>();
            //Java: CriticalPComparator comparator = new CriticalPComparator();

            for (int y = y_min; y <= y_max; y++)
            {
                //Atualiza o valor de cada intersecção nos pontos ativos.
                foreach (CriticalP e in active_criticalPs)
                {
                    e.x_intersection += e.inv_slope;
                }

                //Adiciona as arestas com pontos criíticos para o y corrente.
                foreach (CriticalP e in criticals)
                {
                    if (points[e.index].Y == y)
                    {
                        active_criticalPs.Add(e);
                    }
                }

                //Remove os pontos com y_max = y_var.
                for (int i = active_criticalPs.Count() - 1; i >= 0; i--)
                {
                    CriticalP e = active_criticalPs[i];
                    Point p_max = points[(e.index + e.dir + points.Count()) % points.Count()];
                    if (p_max.Y == y)
                    {
                        active_criticalPs.RemoveAt(i);
                    }
                }

                //Ordena os pontos ativos conforme o valor de x_intersection para o y corrente.
                active_criticalPs.Sort(new CriticalPIntersectionFirst());

                //Pinta entre cada par de pontos ativos.
                for (int i = 0; i < active_criticalPs.Count(); i += 2)
                {
                    int x_start = Convert.ToInt32(Math.Round(active_criticalPs[i].x_intersection));
                    int x_end = Convert.ToInt32(Math.Round(active_criticalPs[i + 1].x_intersection));

                    for (int x = x_start; x < x_end; x++)
                    {
                        if (windows._board[windows.GetIndex(x, y)].Color == "Red")
                            continue;
                        windows.PutPixel(x, y, "Blue");
                    }
                }
            }
        }
    }
}
