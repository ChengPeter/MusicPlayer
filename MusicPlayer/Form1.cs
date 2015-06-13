using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MusicPlayer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //初始化音乐播放器
        private void Form1_Load(object sender, EventArgs e)
        {
            skinEngine1.SkinFile = @"C:\Users\xinqiang\Desktop\皮肤素材库\DiamondBlue.ssk";
            pictureBox1.Image = Image.FromFile(@"C:\Users\xinqiang\Desktop\Picture\1.jpg");
            //取消自动播放
            musicPlayer.settings.autoStart = false;
            //musicPlayer.URL = @"C:\Users\xinqiang\Desktop\music\父亲.mp3";
        }
        int i = 0;
        //换肤
        private void btnChangeSkin_Click(object sender, EventArgs e)
        {
            string[] pathStyle = Directory.GetFiles(@"C:\Users\xinqiang\Desktop\皮肤素材库");
            i++;
            if (i == pathStyle.Length)
            {
                i = 0;
            }
            skinEngine1.SkinFile =pathStyle[i];

        }
        int j = 0;
        //每隔一秒钟换背景图片
        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] pathPicture = Directory.GetFiles(@"C:\Users\xinqiang\Desktop\Picture");
            j++;
            if (j == pathPicture.Length)
            {
                j = 0;
            }
            pictureBox1.Image = Image.FromFile(pathPicture[j]);
        }
        bool b = true;
        //单击播放
        private void btnPlayOrPause_Click(object sender, EventArgs e)
        {
            
            if (btnPlayOrPause.Text == "播放") 
            {
                if (lbMusic.Items.Count == 0)
                {
                    MessageBox.Show("请先选择音乐文件");
                    return;
                }
                if(b)
                    musicPlayer.URL=listpath[lbMusic.SelectedIndex];
                musicPlayer.Ctlcontrols.play();
                loadLrc();
                btnPlayOrPause.Text = "暂停";

            }
            else if(btnPlayOrPause.Text == "暂停")
            {
                musicPlayer.Ctlcontrols.pause();
                btnPlayOrPause.Text = "播放";
                b = false;
            }
        }
        //上一曲
        private void btnBefore_Click(object sender, EventArgs e)
        {

            int index = lbMusic.SelectedIndex;
            lbMusic.ClearSelected();
            index--;
            if (index <0)
            {
                index = listpath.Count-1;
            }
            lbMusic.SelectedIndex = index;
            musicPlayer.URL = listpath[index];
            musicPlayer.Ctlcontrols.play();
        }
        //停止
        private void btnStop_Click(object sender, EventArgs e)
        {
            musicPlayer.Ctlcontrols.stop();
        }
        List<string> listpath = new List<string>();
        //打开音乐文件
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "请选择要打开的音乐文件呦(づ￣3￣)づ╭❤～";
            ofd.Multiselect = true;
            ofd.Filter = "mp3格式音乐文件|*.mp3|所有文件|*.*";
            ofd.InitialDirectory = @"C:\Users\xinqiang\Desktop\music";
            ofd.ShowDialog();
            string[] path = ofd.FileNames;
            if (path.Length == 0)
                return;
            for (int i = 0; i < path.Length; i++)
            {
                listpath.Add(path[i]);
                lbMusic.Items.Add(Path.GetFileName(path[i])); 
            }
           
        }
        //双击音乐列表播放制定音乐
        private void lbMusic_DoubleClick(object sender, EventArgs e)
        {
            
            if(lbMusic.Items.Count==0)
            {
                MessageBox.Show("请先选择音乐文件");
                return;
            }
            try
            {
                musicPlayer.URL = listpath[lbMusic.SelectedIndex];
                musicPlayer.Ctlcontrols.play();
                btnPlayOrPause.Text = "暂停";
                loadLrc();
            }
            catch { }
            
        }
        //下一曲
        private void btnNext_Click(object sender, EventArgs e)
        {
          
            int index = lbMusic.SelectedIndex; 
            lbMusic.ClearSelected();
            index++;
            if (index == listpath.Count)
            {
                index = 0;
            }
            lbMusic.SelectedIndex = index;
            musicPlayer .URL=listpath[index];
            musicPlayer.Ctlcontrols.play();
        }
        //多选删除
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = lbMusic.SelectedItems.Count;
            for (int i = 0; i < count; i++)
            {
                listpath.RemoveAt(lbMusic.SelectedIndex);
                lbMusic.Items.RemoveAt(lbMusic.SelectedIndex);
            }
        }
        //静音与放音
        private void btnShutupOrIOpen_Click(object sender, EventArgs e)
        {
            if (btnShutupOrIOpen.Text == "静音")
            {
                musicPlayer.settings.mute = true;
                btnShutupOrIOpen.Text = "放音";
            }
            else if (btnShutupOrIOpen.Text == "放音")
            {
                musicPlayer.settings.mute = false;
                btnShutupOrIOpen.Text = "静音";
            }
        }
        //根据时间差时间下一曲
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (musicPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                label2.Text = musicPlayer.Ctlcontrols.currentPosition + "\r\n" + musicPlayer.Ctlcontrols.currentPositionString
                    + "\r\n" + musicPlayer.currentMedia.duration + "\r\n" + musicPlayer.currentMedia.durationString + "\r\n";
                //如果歌曲的总时间减去歌曲当前的播放时间小于等于1时播放下一曲
                //if (musicPlayer.currentMedia.duration - musicPlayer.Ctlcontrols.currentPosition < 1)
                //{
                //    int index = lbMusic.SelectedIndex;
                //    lbMusic.ClearSelected();
                //    index++;
                //    if (index == listpath.Count)
                //    {
                //        index = 0;
                //    }
                //    lbMusic.SelectedIndex = index;
                //    musicPlayer.URL = listpath[index];
                //    musicPlayer.Ctlcontrols.play();
                //}
            }
        }
        //根据播放器状态实现下一曲
        private void musicPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (musicPlayer.playState == WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                int index = lbMusic.SelectedIndex;
                lbMusic.ClearSelected();
                index++;
                if (index == listpath.Count)
                {
                    index = 0;
                }
                lbMusic.SelectedIndex = index;
                musicPlayer.URL = listpath[index];   
            }
            if (musicPlayer.playState == WMPLib.WMPPlayState.wmppsReady)
            {
                try
                {
                    musicPlayer.Ctlcontrols.play();
                    loadLrc();
                }
                catch { }
            }

        }
        //加载歌词
        private void loadLrc() 
        { 
         string songpath=listpath[lbMusic.SelectedIndex];
         songpath += ".lrc";
         lblLrc.Text = "";
         if (File.Exists(songpath))
         {
             lblLrc.Text = "歌词正在加载中";
             string[] lrcs = File.ReadAllLines(songpath);
             //格式化歌词
             formatLrc(lrcs);
         }
         else
         {
             lblLrc.Text = "——歌词未找到——";
         }
        }
        //存储时间
        List<double> listTime = new List<double>();
        //存储歌词
        List<string> listLrc = new List<string>();
        private void formatLrc(string[] lrcs)
        { 
            for (int i = 0; i < lrcs.Length; i++)
			{
                //lrcs[0] [00:02.20]相信自己
                //strTemp[0] 00:02.20
                //strTemp[1] 相信自己
                string[] strTemp = lrcs[i].Split(new char[] {'[',']'},StringSplitOptions.RemoveEmptyEntries);
                listLrc.Add(strTemp[1]);
                //strNewTemp[0] 00
                //strNewTemp[2] 02.20
                string[] strNewTemp = strTemp[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                double time = double.Parse(strNewTemp[0])*60 + double.Parse(strNewTemp[1]);
                listTime.Add(time);
			}
         
        }
        //显示歌词
        private void timer3_Tick(object sender, EventArgs e)
        {
            double currentTime = musicPlayer.Ctlcontrols.currentPosition;
            //lblLrc.Text = currentTime.ToString();
            for (int i = 0; i < listTime.Count - 1; i++)
            {
                if (currentTime >= listTime[i] && currentTime < listTime[i + 1])
                {
                    lblLrc.Text = listLrc[i];
                    lblLrc.ForeColor = Color.Red;
                    //string[] fonts = { "微软雅黑", "宋体", "黑体", "隶书", "华文行楷" };

                    Font fb = new Font("华文行楷", 10);
                    lblLrc.Font = fb;
                }
            }
        }

    }
}
