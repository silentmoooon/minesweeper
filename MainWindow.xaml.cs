
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameProcess gameProcess = new GameProcess();
        DispatcherTimer timer = new DispatcherTimer();
        private List<Node> allNode = new List<Node>();
        private Node[,] nodeLayout = new Node[18, 32];
        private int maxNodeCount = 480;
        private int maxMineCount = 99;

        private int gameStatus = 0;
        private int clickCount = 0;
        private bool leftPress = false;
        private bool rightPress = false;

        public MainWindow()
        {
            InitializeComponent();
            InitButton();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
            gameProcess.Time = 0;
            gameProcess.RemainderCount = maxMineCount;
            Binding binding = new Binding();//创建Binding实例
            binding.Source = gameProcess;//指定数据源
            binding.Path = new PropertyPath("Time");//指定访问路径

            gameTime.SetBinding(ContentProperty, binding);

            binding = new Binding();//创建Binding实例
            binding.Source = gameProcess;//指定数据源
            binding.Path = new PropertyPath("RemainderCount");//指定访问路径


            remainderCount.SetBinding(ContentProperty, binding);

        }
        private void ResetMine()
        {
            clickCount = 0;
            timer.Stop();
            gameStatus = 0;
            gameProcess.Time = 0;
            gameProcess.RemainderCount = 99;
            allNode.ForEach(node => { node.IsMine = false; node.MineCount = 0; node.IsMaybeMine = false; node.DisplayStatus = false; });
        }
        private void InitMine(Node node)
        {
            Shuffle(allNode);
            int tempMaxMineCount = maxMineCount;
            for (int i = 0; i < tempMaxMineCount; i++)
            {
                if (allNode[i].Row == node.Row && allNode[i].Col == node.Col)
                {
                    tempMaxMineCount++;
                    continue;
                }
                allNode[i].IsMine = true;
            }



            for (int i = 0; i < allNode.Count; i++)
            {
                Node n = allNode[i];

                if (n.IsMine)
                {
                    continue;
                }

                var subMineList = n.nodes.Where(node => node != null && node.IsMine);

                n.MineCount = subMineList.Count();
            }
            timer.Start();

        }
        private void Shuffle(List<Node> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                Random random = new Random();
                int next = random.Next(i);
                Node n = list[next];
                list[next] = list[i];
                list[i] = n;
            }

        }

        private void InitButton()
        {


            for (int i = 1; i < nodeLayout.GetLength(0) - 1; i++)
            {


                RowDefinition row = new RowDefinition
                {
                    Height = GridLength.Auto,

                };


                grid.RowDefinitions.Add(row);
                for (int j = 1; j < nodeLayout.GetLength(1) - 1; j++)
                {
                    ColumnDefinition col = new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    };

                    grid.ColumnDefinitions.Add(col);
                    Node node = new Node();
                    node.Col = i - 1;
                    node.Row = j - 1;
                    nodeLayout[i, j] = node;
                    allNode.Add(node);
                    Button button = new Button();
                    button.Margin = new Thickness(1, 1, 1, 1);
                    button.Height = 40;
                    button.Width = 40;
                    grid.Children.Add(button);
                    button.SetValue(Grid.RowProperty, i - 1);
                    button.SetValue(Grid.ColumnProperty, j - 1);

                    Binding binding = new Binding();//创建Binding实例
                    binding.Source = node;//指定数据源
                    binding.Path = new PropertyPath("Text");//指定访问路径

                    button.SetBinding(ContentProperty, binding);
                    binding = new Binding();//创建Binding实例
                    binding.Source = node;//指定数据源
                    binding.Path = new PropertyPath("Color");//指定访问路径
                    button.SetBinding(BackgroundProperty, binding);
                    button.Tag = node;
                    button.AddHandler(MouseUpEvent, new MouseButtonEventHandler(Button_MouseUp), true);
                    button.AddHandler(MouseDownEvent, new MouseButtonEventHandler(Button_MouseDown), true);


                }

            }


            for (int i = 1; i < nodeLayout.GetLength(0) - 1; i++)
            {

                for (int j = 1; j < nodeLayout.GetLength(1) - 1; j++)
                {
                    Node n = nodeLayout[i, j];

                    n.nodes[0] = nodeLayout[i - 1, j - 1];
                    n.nodes[1] = nodeLayout[i - 1, j];
                    n.nodes[2] = nodeLayout[i - 1, j + 1];
                    n.nodes[3] = nodeLayout[i, j - 1];
                    n.nodes[4] = nodeLayout[i, j + 1];
                    n.nodes[5] = nodeLayout[i + 1, j - 1];
                    n.nodes[6] = nodeLayout[i + 1, j];
                    n.nodes[7] = nodeLayout[i + 1, j + 1];


                }
            }

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            gameProcess.Time++;
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Button b = (Button)sender;
            Node node = (Node)b.Tag;

            if (gameStatus == 0)
            {
                gameStatus = 1;
                InitMine(node);

            }


            if (leftPress && rightPress)
            {

                leftPress = false;
                rightPress = false;
                if (node.DisplayStatus == false)
                {
                    return;
                }
                int maybeCount = 0;
                for (int i = 0; i < node.nodes.Length; i++)
                {
                    if (node.nodes[i] != null && node.nodes[i].IsMaybeMine == true)
                    {
                        
                        maybeCount++;
                    }
                }
               
                if (maybeCount == node.MineCount)
                {
                    bool result = true;
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (node.nodes[i] != null && node.nodes[i].DisplayStatus == false && node.nodes[i].IsMaybeMine == false)
                        {
                            
                            if (!HandleLeftClick(node.nodes[i]))
                            {
                                result = false;
                            }

                        }
                    }
                    CheckStatus(result);
                }
            }
            else if (e.ChangedButton == MouseButton.Left)
            {

                leftPress = false;

                bool result = HandleLeftClick(node);
                CheckStatus(result);

            }
            else if (e.ChangedButton == MouseButton.Right)
            {

                rightPress = false;
                if (node.DisplayStatus)
                {
                    return;
                }
                if (node.IsMaybeMine == false)
                {

                    node.IsMaybeMine = true;
                    gameProcess.RemainderCount--;

                }
                else
                {

                    node.IsMaybeMine = false;
                    gameProcess.RemainderCount++;
                }

            }



        }

        private void CheckStatus(bool result)
        {
            if (!result)
            {

                MessageBox.Show("用时: " + gameProcess.Time.ToString() + "\n点击重来", "fail");
                ResetMine();
                return;
            }
            if (clickCount == maxNodeCount - maxMineCount)
            {
                MessageBox.Show("用时: " + gameProcess.Time.ToString(), "成功");
                ResetMine();
                return;
            }

        }
        private bool HandleLeftClick(Node node)
        {
            if (node.DisplayStatus || node.IsMaybeMine)
            {
                return true;
            }

            if (node.IsMine)
            {

                for (int i = 0; i < allNode.Count; i++)
                {

                    allNode[i].DisplayStatus = true;

                }

                return false;
            }

            clickCount++;

            node.DisplayStatus = true;
            if (node.MineCount == 0)
            {
                for (int i = 0; i < node.nodes.Length; i++)
                {
                    if (node.nodes[i] != null && node.nodes[i].DisplayStatus == false)
                    {
                        HandleLeftClick(node.nodes[i]);

                    }
                }
            }

            return true;
        }


        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {


            if (e.LeftButton == MouseButtonState.Pressed)
            {
                leftPress = true;

            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                rightPress = true;

            }
        }



    }
}
