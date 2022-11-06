using System;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace lentarss
{

    public partial class Form1 : Form
    {
        // Корневой элемент
        XElement rootTag;
        // Узел дерева
        TreeNode rootNode;

        public Form1()
        {
            InitializeComponent();
        }

        // Рекурсивное построение дерева элементов и их вывод на экран
        private void FindChildrenTags(TreeNode node, XElement tag)
        {
            node.Tag = tag; //Связь Узла и Тэга

            //Получение дочерних элементов
            IEnumerable<XElement> childTags = tag.Elements();

            if (childTags != null) //Если дочерние элементы есть
            {

                foreach (XElement childTag in childTags)
                {
                    //Создание дочернего узла
                    TreeNode childNode = new TreeNode(childTag.Name.ToString());
                    //Побавление поддочерних элементов
                    FindChildrenTags(childNode, childTag);
                    //Добавление дочернего узла в дерево
                    node.Nodes.Add(childNode);

                    // Если элемент - картинка, то загружаем
                    if ("image".Equals(childTag.Name.ToString().ToLower()))
                    {
                        XElement imageUrl = childTag.Element(XName.Get("url"));
                        pictureBox1.ImageLocation = imageUrl.Value;
                    }

                    // Если item, то выводим новость, ссылку и дату
                    if ("item".Equals(childTag.Name.ToString().ToLower()))
                    {

                        // Одна новость
                        XElement title = childTag.Element(XName.Get("title"));
                        XElement date = childTag.Element(XName.Get("pubDate"));
                        XElement link = childTag.Element(XName.Get("link"));
                        XElement src = childTag.Element(XName.Get("description"));

                        richTextBox1.AppendText(title.Value + "\n");
                        richTextBox1.AppendText("\n" + src.Value + "\n" + link.Value + "\n\n");
                        richTextBox1.AppendText(date.Value + "\n\n----------\n\n");

                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Для того, чтобы ссылка открылась в браузере
            richTextBox1.LinkClicked += richTextBox1_LinkClicked;
            OnResize(e);


        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            // Открыть ссылку в браузере
            System.Diagnostics.Process.Start(e.LinkText);
        }

        protected override void OnResize(EventArgs e)
        {
            // Масштабирование элемента
            richTextBox1.SetBounds(richTextBox1.Bounds.X, richTextBox1.Bounds.Y, this.Bounds.Width - (int)(richTextBox1.Bounds.X * 1.1), this.Height - (int)(richTextBox1.Location.Y * 5));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Очистка элемента
            richTextBox1.Clear();
            try {
                //Загрузка ленты
                //rootTag = XElement.Load("https://lenta.ru/rss/top7");
                rootTag = XElement.Load(textBox1.Text);
            } catch(Exception ex) {
                //MessageBox.Show(,"Error trying to load this rss channel!\n" + ex.Message,"Error!","Error!",MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Error trying to load this rss channel!\n", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            //Создание корневого элемента
            rootNode = new TreeNode(rootTag.Name.ToString());
            //Поиск дочерних элементов корневого элемента
            FindChildrenTags(rootNode, rootTag);
        }
    }
}
