using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using FaceitAPI.Match.Details;
using FaceitAPI;
using FaceitAPI.Player.Details;
using FaceitAPI.Match.Stats;
using FaceitAPI.Player.History;
using FaceitAPI.Player.Statistics;
using FaceitAPI.Player;
using FaceitAPI.Player.Statistics.Extension;
using System.Text;

namespace WindowsFormsApp1
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public enum LOADING_IMAGES { fromAllSources, fromLocal, fromWww }
        public enum loadedLogins { None, login1, login2 }
        public enum LOADING_STATUS { None, Preparing, Process, Good, Bad }
        #region Login1
        public PlayerCSGO profile;
        public CSGOPlayerStatisticsProfileSegment last20Profile1;
        public bool isLogin1Loaded = false;
        #endregion

        #region Login2
        public PlayerCSGO profile2;
        public CSGOPlayerStatisticsProfileSegment last20Profile2;
        public bool isLogin2Loaded = false;
        #endregion

        #region Main

        #endregion

        #region Match
        public bool isMatchLoaded = false;
        public bool isMatchLoading = false;
        public List<CSGOMatchStatsTeamPlayer> playerS = new List<CSGOMatchStatsTeamPlayer>();
        #region Details
        public CSGOMatchDetails matchDetails;
        #endregion

        #region Stats
        public CSGOMatchStats matchStatistics;
        #endregion

        #endregion

        #region Variables

        public delegate void OnLoadingLoginChange(loadedLogins login);
        public event OnLoadingLoginChange OnLoadedLogin;

        public delegate void OnStatusLoginChange(LOADING_STATUS status);
        public event OnStatusLoginChange OnStatusLogin;

        public LOADING_IMAGES loadingImages = LOADING_IMAGES.fromAllSources;
        BackgroundWorker thread1 = new BackgroundWorker();
        BackgroundWorker thread2 = new BackgroundWorker();
        BackgroundWorker thread3 = new BackgroundWorker();
        BackgroundWorker thread4 = new BackgroundWorker();
        public Engine faceit = new Engine("c275df18-34f8-467f-a03b-e44bb7fcf581");
        public List<PlayerCSGO> players = new List<PlayerCSGO>();
        public int tabMatchint;
        public int tabProfile1int;
        public int tabProfile2int;
        public List<SysELO> sysElo = new List<SysELO>();
        public string filePatchElo = Application.StartupPath + "/elo.json";
        public loadedLogins lastLoadedLogin = loadedLogins.None;
        public loadedLogins currentlyloadingLogin = loadedLogins.None;
        public LOADING_STATUS statusLoadedLogin = LOADING_STATUS.None;
        public bool showDEmaps = true;
        public bool showAIMmaps = true;
        public bool loadAvatars = true;


        #region UI
        public List<MetroFramework.Controls.MetroLink> lNickname = new List<MetroFramework.Controls.MetroLink>();
        public List<MetroFramework.Controls.MetroLabel> lAVGHSS = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> lMatches = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> lWinRate = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> lELO = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> lLevel = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> lKD = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> lRanking = new List<MetroFramework.Controls.MetroLabel>();
        public List<PictureBox> lAvatar = new List<PictureBox>();
        public List<PictureBox> lCountry = new List<PictureBox>();
        public List<PictureBox> lLevelPict = new List<PictureBox>();
        public List<PictureBox> lAnticheat = new List<PictureBox>();

        public List<MetroFramework.Controls.MetroLink> sNickname = new List<MetroFramework.Controls.MetroLink>();
        public List<MetroFramework.Controls.MetroLabel> sKills = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> sAssists = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> sDeaths = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> sPentaKills = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> sQuadroKills = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> sTripleKills = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> sAvgKD = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> sAvgKR = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> sMVP = new List<MetroFramework.Controls.MetroLabel>();
        public List<MetroFramework.Controls.MetroLabel> sHS = new List<MetroFramework.Controls.MetroLabel>();
        #endregion

        public loadedLogins loadingLogin
        {
            get { return currentlyloadingLogin; }
            set
            {
                currentlyloadingLogin = value;
                OnLoadedLogin(value);
            }
        }
        public LOADING_STATUS statusLogin
        {
            get { return statusLoadedLogin; }
            set
            {
                statusLoadedLogin = value;
                OnStatusLogin(value);
            }
        }
        #endregion

        #region Get

        public static Image GetAvatar(string avatarURL)
        {
            if (ConnectivityChecker.CheckInternet() == ConnectivityChecker.ConnectionStatus.Connected)
            {
                try
                {
                    if (avatarURL == null || avatarURL == "")
                    {
                        WebRequest req = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/avatars/avatar_default_user_300x300.jpg");
                        WebResponse res = req.GetResponse();
                        return Image.FromStream(res.GetResponseStream());
                    }
                    else
                    {
                        WebRequest req = WebRequest.Create(avatarURL);
                        WebResponse res = req.GetResponse();
                        return Image.FromStream(res.GetResponseStream());
                    }
                }
                catch (WebException ex) when (ex.Response != null)
                {
                    if (ex.Response == null) throw; // Убрать в C# 6

                    Form2 form = new Form2();
                    form.errorMsg.Text = "Avatar not loaded. Try later...";
                    form.ShowDialog();
                    return null;
                }
            }
            else
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Plz check internet connection.";
                return Image.FromFile(Application.StartupPath + @"\level\10.png");
            }
        }
        public static Image GetImageServer(string serverURL)
        {
            if (ConnectivityChecker.CheckInternet() == ConnectivityChecker.ConnectionStatus.Connected)
            {
                try
                {
                    WebRequest req = WebRequest.Create(serverURL);
                    WebResponse res = req.GetResponse();
                    return Image.FromStream(res.GetResponseStream());
                }
                catch (WebException ex) when (ex.Response != null)
                {
                    if (ex.Response == null) throw; // Убрать в C# 6

                    Form2 form = new Form2();
                    form.errorMsg.Text = "Server image not loaded. Try later...";
                    form.ShowDialog();
                    return null;
                }
            }
            else
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Plz check internet connection.";
                return null;
            }
        }
        public static bool GetCountry(string countryCode, out Image img)
        {
            if (ConnectivityChecker.CheckInternet() == ConnectivityChecker.ConnectionStatus.Connected)
            {
                try
                {
                    if (countryCode != null)
                    {
                        WebRequest req = WebRequest.Create("https://cdn-frontend.faceit.com/web/56-1534534293/src/app/assets/images-compress/flags/" + countryCode.ToUpper() + ".png");
                        WebResponse res = req.GetResponse();
                        img = Image.FromStream(res.GetResponseStream());
                        return true;
                    }
                    else
                    {
                        img = null;
                        return false;
                    }
                }
                catch (WebException ex) when (ex.Response != null)
                {
                    if (ex.Response == null) throw; // Убрать в C# 6

                    Form2 form = new Form2();
                    form.errorMsg.Text = "Country image not loaded.";
                    form.ShowDialog();
                    img = null;
                    return false;
                }
            }
            else
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Plz check internet connection.";
                img = null;
                return false;
            }
        }
        
        public static Image GetImageLevel(int level)
        {
            Image img = Image.FromFile(Application.StartupPath + @"\level\" + level + ".png");
            return img;
        }
        public static Image GetImageAnticheat(bool anticheat)
        {
            if (anticheat)
            {
                Image img = Image.FromFile(Application.StartupPath + @"\anticheat\anticheat_enabled.png");
                return img;
            }
            else
            {
                Image img = Image.FromFile(Application.StartupPath + @"\anticheat\anticheat_disabled.png");
                return img;
            }
            
        }
        #endregion

        private void OnEvent()
        {
            OnLoadedLogin += OnLoadingLogin;
            OnStatusLogin += OnStatusLogining;
        }
        public Form1()
        {
            OnEvent();
            InitializeComponent();
            InitializeBackgroundWorker();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Main();
            //GetInfoMatch1.Enabled = false;
            lNickname.Add(mNickname1);
            lNickname.Add(mNickname2);
            lNickname.Add(mNickname3);
            lNickname.Add(mNickname4);
            lNickname.Add(mNickname5);
            lNickname.Add(mNickname6);
            lNickname.Add(mNickname7);
            lNickname.Add(mNickname8);
            lNickname.Add(mNickname9);
            lNickname.Add(mNickname10);

            lAVGHSS.Add(mAvgHS1);
            lAVGHSS.Add(mAvgHS2);
            lAVGHSS.Add(mAvgHS3);
            lAVGHSS.Add(mAvgHS4);
            lAVGHSS.Add(mAvgHS5);
            lAVGHSS.Add(mAvgHS6);
            lAVGHSS.Add(mAvgHS7);
            lAVGHSS.Add(mAvgHS8);
            lAVGHSS.Add(mAvgHS9);
            lAVGHSS.Add(mAvgHS10);

            lMatches.Add(mMatches1);
            lMatches.Add(mMatches2);
            lMatches.Add(mMatches3);
            lMatches.Add(mMatches4);
            lMatches.Add(mMatches5);
            lMatches.Add(mMatches6);
            lMatches.Add(mMatches7);
            lMatches.Add(mMatches8);
            lMatches.Add(mMatches9);
            lMatches.Add(mMatches10);

            lWinRate.Add(mWinRate1);
            lWinRate.Add(mWinRate2);
            lWinRate.Add(mWinRate3);
            lWinRate.Add(mWinRate4);
            lWinRate.Add(mWinRate5);
            lWinRate.Add(mWinRate6);
            lWinRate.Add(mWinRate7);
            lWinRate.Add(mWinRate8);
            lWinRate.Add(mWinRate9);
            lWinRate.Add(mWinRate10);

            lAnticheat.Add(mAnticheat1);
            lAnticheat.Add(mAnticheat2);
            lAnticheat.Add(mAnticheat3);
            lAnticheat.Add(mAnticheat4);
            lAnticheat.Add(mAnticheat5);
            lAnticheat.Add(mAnticheat6);
            lAnticheat.Add(mAnticheat7);
            lAnticheat.Add(mAnticheat8);
            lAnticheat.Add(mAnticheat9);
            lAnticheat.Add(mAnticheat10);

            lELO.Add(mElo1);
            lELO.Add(mElo2);
            lELO.Add(mElo3);
            lELO.Add(mElo4);
            lELO.Add(mElo5);
            lELO.Add(mElo6);
            lELO.Add(mElo7);
            lELO.Add(mElo8);
            lELO.Add(mElo9);
            lELO.Add(mElo10);

            lKD.Add(mKD1);
            lKD.Add(mKD2);
            lKD.Add(mKD3);
            lKD.Add(mKD4);
            lKD.Add(mKD5);
            lKD.Add(mKD6);
            lKD.Add(mKD7);
            lKD.Add(mKD8);
            lKD.Add(mKD9);
            lKD.Add(mKD10);

            lRanking.Add(mRanking1);
            lRanking.Add(mRanking2);
            lRanking.Add(mRanking3);
            lRanking.Add(mRanking4);
            lRanking.Add(mRanking5);
            lRanking.Add(mRanking6);
            lRanking.Add(mRanking7);
            lRanking.Add(mRanking8);
            lRanking.Add(mRanking9);
            lRanking.Add(mRanking10);

            lAvatar.Add(mAvatar1);
            lAvatar.Add(mAvatar2);
            lAvatar.Add(mAvatar3);
            lAvatar.Add(mAvatar4);
            lAvatar.Add(mAvatar5);
            lAvatar.Add(mAvatar6);
            lAvatar.Add(mAvatar7);
            lAvatar.Add(mAvatar8);
            lAvatar.Add(mAvatar9);
            lAvatar.Add(mAvatar10);

            lCountry.Add(mCountry1);
            lCountry.Add(mCountry2);
            lCountry.Add(mCountry3);
            lCountry.Add(mCountry4);
            lCountry.Add(mCountry5);
            lCountry.Add(mCountry6);
            lCountry.Add(mCountry7);
            lCountry.Add(mCountry8);
            lCountry.Add(mCountry9);
            lCountry.Add(mCountry10);

            lLevelPict.Add(mLevelPicture1);
            lLevelPict.Add(mLevelPicture2);
            lLevelPict.Add(mLevelPicture3);
            lLevelPict.Add(mLevelPicture4);
            lLevelPict.Add(mLevelPicture5);
            lLevelPict.Add(mLevelPicture6);
            lLevelPict.Add(mLevelPicture7);
            lLevelPict.Add(mLevelPicture8);
            lLevelPict.Add(mLevelPicture9);
            lLevelPict.Add(mLevelPicture10);

            lLevel.Add(mLevel1);
            lLevel.Add(mLevel2);
            lLevel.Add(mLevel3);
            lLevel.Add(mLevel4);
            lLevel.Add(mLevel5);
            lLevel.Add(mLevel6);
            lLevel.Add(mLevel7);
            lLevel.Add(mLevel8);
            lLevel.Add(mLevel9);
            lLevel.Add(mLevel10);

            sNickname.Add(statsNickname1);
            sNickname.Add(statsNickname2);
            sNickname.Add(statsNickname3);
            sNickname.Add(statsNickname4);
            sNickname.Add(statsNickname5);
            sNickname.Add(statsNickname6);
            sNickname.Add(statsNickname7);
            sNickname.Add(statsNickname8);
            sNickname.Add(statsNickname9);
            sNickname.Add(statsNickname10);

            sKills.Add(statsKills1);
            sKills.Add(statsKills2);
            sKills.Add(statsKills3);
            sKills.Add(statsKills4);
            sKills.Add(statsKills5);
            sKills.Add(statsKills6);
            sKills.Add(statsKills7);
            sKills.Add(statsKills8);
            sKills.Add(statsKills9);
            sKills.Add(statsKills10);

            sDeaths.Add(statsDeaths1);
            sDeaths.Add(statsDeaths2);
            sDeaths.Add(statsDeaths3);
            sDeaths.Add(statsDeaths4);
            sDeaths.Add(statsDeaths5);
            sDeaths.Add(statsDeaths6);
            sDeaths.Add(statsDeaths7);
            sDeaths.Add(statsDeaths8);
            sDeaths.Add(statsDeaths9);
            sDeaths.Add(statsDeaths10);

            sAssists.Add(statsAssists1);
            sAssists.Add(statsAssists2);
            sAssists.Add(statsAssists3);
            sAssists.Add(statsAssists4);
            sAssists.Add(statsAssists5);
            sAssists.Add(statsAssists6);
            sAssists.Add(statsAssists7);
            sAssists.Add(statsAssists8);
            sAssists.Add(statsAssists9);
            sAssists.Add(statsAssists10);

            sPentaKills.Add(statsPentaKills1);
            sPentaKills.Add(statsPentaKills2);
            sPentaKills.Add(statsPentaKills3);
            sPentaKills.Add(statsPentaKills4);
            sPentaKills.Add(statsPentaKills5);
            sPentaKills.Add(statsPentaKills6);
            sPentaKills.Add(statsPentaKills7);
            sPentaKills.Add(statsPentaKills8);
            sPentaKills.Add(statsPentaKills9);
            sPentaKills.Add(statsPentaKills10);

            sQuadroKills.Add(statsQuadroKills1);
            sQuadroKills.Add(statsQuadroKills2);
            sQuadroKills.Add(statsQuadroKills3);
            sQuadroKills.Add(statsQuadroKills4);
            sQuadroKills.Add(statsQuadroKills5);
            sQuadroKills.Add(statsQuadroKills6);
            sQuadroKills.Add(statsQuadroKills7);
            sQuadroKills.Add(statsQuadroKills8);
            sQuadroKills.Add(statsQuadroKills9);
            sQuadroKills.Add(statsQuadroKills10);

            sTripleKills.Add(statsTripleKills1);
            sTripleKills.Add(statsTripleKills2);
            sTripleKills.Add(statsTripleKills3);
            sTripleKills.Add(statsTripleKills4);
            sTripleKills.Add(statsTripleKills5);
            sTripleKills.Add(statsTripleKills6);
            sTripleKills.Add(statsTripleKills7);
            sTripleKills.Add(statsTripleKills8);
            sTripleKills.Add(statsTripleKills9);
            sTripleKills.Add(statsTripleKills10);

            sAvgKD.Add(statsKD1);
            sAvgKD.Add(statsKD2);
            sAvgKD.Add(statsKD3);
            sAvgKD.Add(statsKD4);
            sAvgKD.Add(statsKD5);
            sAvgKD.Add(statsKD6);
            sAvgKD.Add(statsKD7);
            sAvgKD.Add(statsKD8);
            sAvgKD.Add(statsKD9);
            sAvgKD.Add(statsKD10);

            sAvgKR.Add(statsKR1);
            sAvgKR.Add(statsKR2);
            sAvgKR.Add(statsKR3);
            sAvgKR.Add(statsKR4);
            sAvgKR.Add(statsKR5);
            sAvgKR.Add(statsKR6);
            sAvgKR.Add(statsKR7);
            sAvgKR.Add(statsKR8);
            sAvgKR.Add(statsKR9);
            sAvgKR.Add(statsKR10);

            sMVP.Add(statsMVP1);
            sMVP.Add(statsMVP2);
            sMVP.Add(statsMVP3);
            sMVP.Add(statsMVP4);
            sMVP.Add(statsMVP5);
            sMVP.Add(statsMVP6);
            sMVP.Add(statsMVP7);
            sMVP.Add(statsMVP8);
            sMVP.Add(statsMVP9);
            sMVP.Add(statsMVP10);

            sHS.Add(statsHS1);
            sHS.Add(statsHS2);
            sHS.Add(statsHS3);
            sHS.Add(statsHS4);
            sHS.Add(statsHS5);
            sHS.Add(statsHS6);
            sHS.Add(statsHS7);
            sHS.Add(statsHS8);
            sHS.Add(statsHS9);
            sHS.Add(statsHS10);

        }

        public void OnLoadingLogin(loadedLogins login)
        {
            if (loadingLogin == loadedLogins.None) return;
            
            DisableStatsCompareProfile();
            statusLogin = LOADING_STATUS.Preparing;
            CheckOnLoadedLogins();
        }
        public void OnStatusLogining(LOADING_STATUS status)
        {
            if(loadingLogin == loadedLogins.login1)
            {
                if(statusLogin == LOADING_STATUS.Preparing)
                {
                    ClearProfileTab();
                    isLogin1Loaded = false;
                    getProfile1.Enabled = false; //отключение кнопки
                    login1.Enabled = false; //отключение поля с никнеймом
                    metroPanel1.Visible = true; //скрытие UI
                    statusLogin = LOADING_STATUS.Process;
                }
                else if(statusLogin == LOADING_STATUS.Process)
                {
                    thread3.RunWorkerAsync();
                }
                else if(statusLogin == LOADING_STATUS.Good)
                {
                    ShowInfoProfileTab(profile);
                    Last20StyleProfile1_DefaultStyle();
                    loginProfileProfile1_defaultStyle();
                    loginMapBox1Profile1_defaultStyle();
                    loginMapBox2Profile1_defaultStyle();
                    if (profile.detail.friends_Ids.Count != 0)
                    {
                        friendsBoxProfile1.Enabled = true;
                        getFriendInfoProfile1.Enabled = true;
                        friendsBoxProfile1.SelectedIndex = 0;
                    }
                    else
                    {
                        friendsBoxProfile1.Enabled = false;
                        getFriendInfoProfile1.Enabled = false;
                    }
                    Load20MatchesProfile1History();
                    metroTabControl1.SelectTab(tabProfile1int);
                    metroPanel1.Visible = false;
                    metroProgressBar3.Value = 0;
                    lastLoadedLogin = loadedLogins.login1;
                    login1.Enabled = true;
                    getProfile1.Enabled = true;
                }
                else if(statusLogin == LOADING_STATUS.Bad)
                {

                }
            }
            else if(loadingLogin == loadedLogins.login2)
            {
                if (statusLogin == LOADING_STATUS.Preparing)
                {
                    ClearProfile2Tab();
                    isLogin2Loaded = false;
                    getProfile2.Enabled = false; //отключение кнопки
                    login2.Enabled = false; //отключение поля с никнеймом
                    metroPanel2.Visible = true; //скрытие UI
                    statusLogin = LOADING_STATUS.Process;
                }
                else if (statusLogin == LOADING_STATUS.Process)
                {
                    thread3.RunWorkerAsync();
                }
                else if (statusLogin == LOADING_STATUS.Good)
                {
                    ShowInfoProfile2Tab(profile2);
                    Last20StyleProfile2_DefaultStyle();
                    loginProfileProfile2_defaultStyle();
                    loginMapBox1Profile2_defaultStyle();
                    loginMapBox2Profile2_defaultStyle();
                    if (profile2.detail.friends_Ids.Count != 0)
                    {
                        friendsBoxProfile2.Enabled = true;
                        getFriendInfoProfile2.Enabled = true;
                        friendsBoxProfile2.SelectedIndex = 0;
                    }
                    else
                    {
                        friendsBoxProfile2.Enabled = false;
                        getFriendInfoProfile2.Enabled = false;
                    }
                    Load20MatchesProfile2History();
                    metroTabControl1.SelectTab(tabProfile2int);
                    metroPanel2.Visible = false;
                    metroProgressBar3.Value = 0;
                    lastLoadedLogin = loadedLogins.login2;
                    login2.Enabled = true;
                    getProfile2.Enabled = true;
                }
                else if (statusLogin == LOADING_STATUS.Bad)
                {

                }
            }
        }
        public void LoadMapInComboBox(ComboBox box, List<CSGOPlayerStatisticsProfileSegment> maps)
        {
            for (int i = 0; i < maps.Count; i++)
            {

                if (maps[i].label.StartsWith("aim_"))
                {
                    box.Items.Add(maps[i].label + " (" + maps[i].mode + ")");
                }
                if (maps[i].label == "de_cache")
                {

                    box.Items.Add("Cache (" + maps[i].mode + ")");
                }
                if (maps[i].label == "de_cbble")
                {
                    box.Items.Add("Cobblestone (" + maps[i].mode + ")");
                }
                if (maps[i].label == "de_dust2")
                {
                    box.Items.Add("Dust2 (" + maps[i].mode + ")");
                }
                if (maps[i].label == "de_mirage")
                {
                    box.Items.Add("Mirage (" + maps[i].mode + ")");
                }
                if (maps[i].label == "de_inferno")
                {
                    box.Items.Add("Inferno (" + maps[i].mode + ")");
                }
                if (maps[i].label == "de_nuke")
                {
                    box.Items.Add("Nuke (" + maps[i].mode + ")");
                }
                if (maps[i].label == "de_train")
                {
                    box.Items.Add("Train (" + maps[i].mode + ")");
                }
                if (maps[i].label == "de_overpass")
                {
                    box.Items.Add("Overpass (" + maps[i].mode + ")");
                }
            }
        }

        //Cancel Process
        private void button2_Click(object sender, EventArgs e)
        {
            //Check if background worker is doing anything and send a cancellation if it is
            //if (backgroundWorker.IsBusy)
            //{
                //backgroundWorker.CancelAsync();
            //}

        }

        
        public void LoadMembers()
        {
            List<CSGOMatchDetailsTeamPlayer> plays = new List<CSGOMatchDetailsTeamPlayer>();
            foreach(CSGOMatchDetailsTeamPlayer play in matchDetails.faction1.roster)
            {
                plays.Add(play);
            }
            foreach (CSGOMatchDetailsTeamPlayer play in matchDetails.faction2.roster)
            {
                plays.Add(play);
            }

            for (int i = 0; i < plays.Count; i++)
            {
                for (int k = 0; k < players.Count; k++)
                {
                    if(players[k].detail.player_Id == plays[i].player_Id)
                    {
                        lNickname[i].Text = players[k].detail.nickname;
                        lAVGHSS[i].Text = players[k].statistics.average_Headshots_Percentage.ToString() + "%";
                        lMatches[i].Text = players[k].statistics.matches.ToString();
                        lWinRate[i].Text = players[k].statistics.win_Rate.ToString() + "%";
                        lELO[i].Text = players[k].detail.faceit_Elo.ToString();
                        lKD[i].Text = players[k].statistics.average_KD_Ratio.ToString();
                        lRanking[i].Text = players[k].ranking.position.ToString();
                        lLevel[i].Text = players[k].detail.skill_Level.ToString();
                        if(loadingImages == LOADING_IMAGES.fromAllSources)
                        {
                            if (GetCountry(players[k].detail.country, out Image img))
                            {
                                lCountry[i].Image = img;
                            }
                            lAvatar[i].Image = GetAvatar(players[k].detail.avatar.ToString());
                            lLevelPict[i].Image = GetImageLevel(players[k].detail.skill_Level);
                            lAnticheat[i].Image = GetImageAnticheat(plays[i].anticheat_Required);
                        }
                        else if(loadingImages == LOADING_IMAGES.fromLocal)
                        {
                            lLevelPict[i].Image = GetImageLevel(players[k].detail.skill_Level);
                            lAnticheat[i].Image = GetImageAnticheat(plays[i].anticheat_Required);
                        }
                        else
                        {
                            if (GetCountry(players[k].detail.country, out Image img))
                            {
                                lCountry[i].Image = img;
                            }
                            lAvatar[i].Image = GetAvatar(players[k].detail.avatar.ToString());
                        }
                        if(players[k].detail.player_Id == matchDetails.faction1.leader)
                        {
                            lNickname[i].Style = MetroFramework.MetroColorStyle.Blue;
                        }
                        else if(players[k].detail.player_Id == matchDetails.faction2.leader)
                        {
                            lNickname[i].Style = MetroFramework.MetroColorStyle.Blue;
                        }
                        else
                        {
                            lNickname[i].Style = MetroFramework.MetroColorStyle.Black;
                        }
                    }
                }
                
                /* Недоступно в API версии 2
                lJoinType[i].Text = plays[i].joit_Type; недоступно в API версии 2
                for (int r = 0; r < memb.select_members_id.Count; r++)
                {
                    JObject dec = GetProfile(memb.select_members_id[r], false);
                    lLobby[i].Items.Add(dec["nickname"].ToString());
                }
                lLobby[i].SelectedIndex = 0;
                */
            }
           
        } //Вывод тиммейтов
        
        private bool GetAPI(string matchID, out JObject value)
        {
            value = null;
            StreamReader sr = null;
            HttpWebRequest req = WebRequest.CreateHttp("https://open.faceit.com/data/v4/matches/" + matchID);
            req.Headers.Add("Authorization", "Bearer c275df18-34f8-467f-a03b-e44bb7fcf581");
            req.ContentType = "application / json";
            try
            {
                HttpWebResponse wdwdw = (HttpWebResponse)req.GetResponse();
                using (sr = new StreamReader(wdwdw.GetResponseStream(), Encoding.UTF8))
                {
                    value = JObject.Parse(sr.ReadToEnd());
                    return true;
                }
            }
            catch (WebException e)
            {
                HttpWebResponse response = (HttpWebResponse)e.Response;
                Form2 form = new Form2();
                form.errorMsg.Text = response.StatusCode.ToString();
                form.Show();
                value = null;
                return false;
            }

        }
        private void Main()
        {
            metroPanel1.Visible = true;
            metroPanel3.Visible = false;
            metroPanel2.Visible = true;
            metroTabControl1.SelectedIndex = 0;
            for(int i = 0; i < metroTabControl1.TabPages.Count; i++)
            {
                if(metroTabControl1.TabPages[i].Text == "Match")
                {
                    tabMatchint = i;
                }
                else if (metroTabControl1.TabPages[i].Text == "Profile #1")
                {
                    tabProfile1int = i;
                }
                else if (metroTabControl1.TabPages[i].Text == "Profile #2")
                {
                    tabProfile2int = i;
                }
            }
            
        }
        
        #region Login1

        // Очистить табло логин1
        public void ClearProfileTab()
        {
            friendsBoxProfile1.Items.Clear();
            map1BoxProfile1.Items.Clear();
            map2BoxProfile1.Items.Clear();
        }

        // Показать информацию о map1
        private void map1BoxProfile1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMapBox();
            loginMapBox1Profile1_defaultStyle();
        }

        // Показать информацию о map2
        private void map2BoxProfile1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMapBox2();
            loginMapBox2Profile1_defaultStyle();
        }

        // Показать информацию профиля
        public void ShowInfoProfileTab(PlayerCSGO acc)
        {
            if (isLogin1Loaded)
            {
                for (int i = 0; i < acc.detail.friends_Ids.Count; i++)
                {
                    if(acc.detail.friends_Ids[i] != null)
                    {
                        PlayerDetails pl_det = new PlayerDetails();
                        pl_det.faceit = faceit;
                        if (pl_det.GetMatchDetailsCSGO_PlayerID(acc.detail.friends_Ids[i], out CSGOPlayerDetails value))
                        {
                            friendsBoxProfile1.Items.Add(value.nickname);
                        }
                    }
                }
                avatarProfile1.Image = GetAvatar(acc.detail.avatar.ToString());
                levelProfile1.Text = acc.detail.skill_Level.ToString();
                eloProfile1.Text = acc.detail.faceit_Elo.ToString();
                nicknameProfile1.Text = acc.detail.nickname;
                countryProfile1.Text = acc.detail.country;
                membershipProfile1.Text = acc.detail.membership_Type;
                steamid64Profile1.Text = acc.detail.steam_Id_64.ToString();
                avgHSProfile1.Text = acc.statistics.average_Headshots_Percentage.ToString() + "%";
                avgKDProfile1.Text = acc.statistics.average_KD_Ratio.ToString();
                curWinStrikeProfile1.Text = acc.statistics.current_Win_Streak.ToString();
                longestWinStrikeProfile1.Text = acc.statistics.longest_Win_Streak.ToString();
                matchesProfile1.Text = acc.statistics.matches.ToString();
                winMatchesProfile1.Text = acc.statistics.wins.ToString();
                winRateProfile1.Text = acc.statistics.win_Rate.ToString() + "%";
                LoadMapInComboBox(map1BoxProfile1, acc.statistics.segments);
                LoadMapInComboBox(map2BoxProfile1, acc.statistics.segments);
                map1BoxProfile1.SelectedIndex = 0;
                map2BoxProfile1.SelectedIndex = 0;
            }
        }

        // Обновить информацию о map1
        public void UpdateMapBox()
        {
            int index = map1BoxProfile1.SelectedIndex;
            modeProfile1Map1.Text = profile.statistics.segments[index].mode;
            matchesProfile1Map1.Text = profile.statistics.segments[index].matches.ToString();
            winsProfile1Map1.Text = profile.statistics.segments[index].wins.ToString();
            winRateProfile1Map1.Text = profile.statistics.segments[index].win_Rate_Percantage.ToString() + "%";
            roundsProfile1Map1.Text = profile.statistics.segments[index].rounds.ToString();
            killsProfile1Map1.Text = profile.statistics.segments[index].kills.ToString();
            deathProfile1Map1.Text = profile.statistics.segments[index].deaths.ToString();
            kdRatioProfile1Map1.Text = profile.statistics.segments[index].kd_Ratio.ToString();
            krRatioProfile1Map1.Text = profile.statistics.segments[index].kr_Ratio.ToString();
            mvpsProfile1Map1.Text = profile.statistics.segments[index].mvps.ToString();
            totalHSProfile1Map1.Text = profile.statistics.segments[index].total_Headshot_Percentage.ToString();
            assistsProfile1Map1.Text = profile.statistics.segments[index].assists.ToString();
            avgKDRatioProfile1Map1.Text = profile.statistics.segments[index].average_KD_Ratio.ToString();
            avgKRRatioProfile1Map1.Text = profile.statistics.segments[index].average_KR_Ratio.ToString();
            pentaKillsProfile1Map1.Text = profile.statistics.segments[index].penta_Kills.ToString();
            avgPentaKillsProfile1Map1.Text = profile.statistics.segments[index].average_Penta_Kills.ToString();
            quadroKillsProfile1Map1.Text = profile.statistics.segments[index].quadro_Kills.ToString();
            avgQuadroKillsProfile1Map1.Text = profile.statistics.segments[index].average_Quadro_Kills.ToString();
            tripleKillsProfile1Map1.Text = profile.statistics.segments[index].triple_Kills.ToString();
            avgTripleKillsProfile1Map1.Text = profile.statistics.segments[index].average_Triple_Kills.ToString();
            hsPerMatchProfile1Map1.Text = profile.statistics.segments[index].headshots_Per_match.ToString();
            avgKillsProfile1Map1.Text = profile.statistics.segments[index].average_Kills.ToString();
            avgDeathProfile1Map1.Text = profile.statistics.segments[index].average_Deaths.ToString();
            avgMVPsProfile1Map1.Text = profile.statistics.segments[index].average_MVPs.ToString();
            avgAssistsProfile1Map1.Text = profile.statistics.segments[index].average_Assists.ToString();
            avgHSsProfile1Map1.Text = profile.statistics.segments[index].average_Headshot_Percentage.ToString();
        }

        // Обновить информацию о map2
        public void UpdateMapBox2()
        {
            int index = map2BoxProfile1.SelectedIndex;
            modeProfile1Map2.Text = profile.statistics.segments[index].mode;
            matchesProfile1Map2.Text = profile.statistics.segments[index].matches.ToString();
            winsProfile1Map2.Text = profile.statistics.segments[index].wins.ToString();
            winRateProfile1Map2.Text = profile.statistics.segments[index].win_Rate_Percantage.ToString() + "%";
            roundsProfile1Map2.Text = profile.statistics.segments[index].rounds.ToString();
            killsProfile1Map2.Text = profile.statistics.segments[index].kills.ToString();
            deathProfile1Map2.Text = profile.statistics.segments[index].deaths.ToString();
            kdRatioProfile1Map2.Text = profile.statistics.segments[index].kd_Ratio.ToString();
            krRatioProfile1Map2.Text = profile.statistics.segments[index].kr_Ratio.ToString();
            mvpsProfile1Map2.Text = profile.statistics.segments[index].mvps.ToString();
            totalHSProfile1Map2.Text = profile.statistics.segments[index].total_Headshot_Percentage.ToString();
            assistsProfile1Map2.Text = profile.statistics.segments[index].assists.ToString();
            avgKDRatioProfile1Map2.Text = profile.statistics.segments[index].average_KD_Ratio.ToString();
            avgKRRatioProfile1Map2.Text = profile.statistics.segments[index].average_KR_Ratio.ToString();
            pentaKillsProfile1Map2.Text = profile.statistics.segments[index].penta_Kills.ToString();
            avgPentaKillsProfile1Map2.Text = profile.statistics.segments[index].average_Penta_Kills.ToString();
            quadroKillsProfile1Map2.Text = profile.statistics.segments[index].quadro_Kills.ToString();
            avgQuadroKillsProfile1Map2.Text = profile.statistics.segments[index].average_Quadro_Kills.ToString();
            tripleKillsProfile1Map2.Text = profile.statistics.segments[index].triple_Kills.ToString();
            avgTripleKillsProfile1Map2.Text = profile.statistics.segments[index].average_Triple_Kills.ToString();
            hsPerMatchProfile1Map2.Text = profile.statistics.segments[index].headshots_Per_match.ToString();
            avgKillsProfile1Map2.Text = profile.statistics.segments[index].average_Kills.ToString();
            avgDeathProfile1Map2.Text = profile.statistics.segments[index].average_Deaths.ToString();
            avgMVPsProfile1Map2.Text = profile.statistics.segments[index].average_MVPs.ToString();
            avgAssistsProfile1Map2.Text = profile.statistics.segments[index].average_Assists.ToString();
            avgHSsProfile1Map2.Text = profile.statistics.segments[index].average_Headshot_Percentage.ToString();
        }

        //
        private void compareMaps1_Click(object sender, EventArgs e)
        {
            DisableStatsCompareProfile();
            loginProfileProfile1_defaultStyle();
            loginMapBox1Profile1_defaultStyle();
            loginMapBox2Profile1_defaultStyle();
            CompareMap1andMap2Profile1_Style();
            SelectBestMapProfile1();
        }

        //
        private void goToFaceITProfile1_Click(object sender, EventArgs e)
        {
            Process.Start(profile.detail.faceit_Url.ToString());
        }

        //
        private void goToSteamProfile1_Click(object sender, EventArgs e)
        {
            Process.Start("https://steamcommunity.com/profiles/" + profile.detail.steam_Id_64);
        }

        public void Load20MatchesProfile1History()
        {
            List<CSGOMatchStatsTeamPlayerStats> statLast20 = new List<CSGOMatchStatsTeamPlayerStats>();
            MatchStats match_Stats = new MatchStats();
            match_Stats.faceit = faceit;
            CSGOPlayerStatisticsProfileSegment last20Profile1 = new CSGOPlayerStatisticsProfileSegment();

            for (int i = 0; i < profile.history.matches.Count; i++)
            {
                if (match_Stats.GetMatchStatsCSGO(profile.history.matches[i].match_Id, out CSGOMatchStats value))
                {
                    last20Profile1.rounds += value.rounds;
                    statLast20.Add(value.GetStatsPlayer(profile.detail.player_Id));
                }
                else
                {
                    Form2 form = new Form2();
                    form.errorMsg.Text = "Error: " + match_Stats.GetLastError;
                    form.Show();
                    return;
                }
            }

            for (int i = 0; i < statLast20.Count; i++)
            {
                last20Profile1.assists += statLast20[i].assists;
                last20Profile1.deaths += statLast20[i].deaths;
                last20Profile1.headshots += statLast20[i].headshot;
                last20Profile1.total_Headshot_Percentage += statLast20[i].headshot_Percentage;
                last20Profile1.kd_Ratio += statLast20[i].kd_Ratio;
                last20Profile1.kills += statLast20[i].kills;
                last20Profile1.kr_Ratio += statLast20[i].kr_Ratio;
                last20Profile1.mvps += statLast20[i].mvps;
                last20Profile1.penta_Kills += statLast20[i].penta_Kills;
                last20Profile1.quadro_Kills += statLast20[i].quadro_Kills;
                last20Profile1.triple_Kills += statLast20[i].triple_Kills;
                last20Profile1.wins += statLast20[i].result;

            }
            last20Profile1.matches = statLast20.Count + 1;
            last20Profile1.average_Assists = last20Profile1.assists / last20Profile1.matches;
            last20Profile1.average_Deaths = last20Profile1.deaths / last20Profile1.matches;
            last20Profile1.average_Headshot_Percentage = last20Profile1.headshots * 100 / last20Profile1.matches;
            last20Profile1.average_KD_Ratio = last20Profile1.kd_Ratio / last20Profile1.matches;
            last20Profile1.average_KR_Ratio = last20Profile1.kr_Ratio / last20Profile1.matches;
            last20Profile1.average_Kills = last20Profile1.kills / last20Profile1.matches;
            last20Profile1.average_MVPs = last20Profile1.mvps / last20Profile1.matches;
            last20Profile1.average_Penta_Kills = last20Profile1.penta_Kills / last20Profile1.matches;
            last20Profile1.average_Quadro_Kills = last20Profile1.quadro_Kills / last20Profile1.matches;
            last20Profile1.average_Triple_Kills = last20Profile1.triple_Kills / last20Profile1.matches;
            last20Profile1.headshots_Per_match = last20Profile1.headshots / last20Profile1.matches;
            last20Profile1.win_Rate_Percantage += last20Profile1.wins * 100 / last20Profile1.matches;
            
            Combine20MatchesProfile1(last20Profile1);
        }

        //
        public void Combine20MatchesProfile1(CSGOPlayerStatisticsProfileSegment last20)
        {
            /*profileLast20mode.Text = "";
            for (int i = 0; i < stats.Count; i++)
            {
                if (!profileLast20mode.Text.Contains(stats[i].mode))
                {
                    profileLast20mode.Text += '"' + stats[i].mode + '"' + " ";
                }
            }*/

            double average_Rounds = last20.rounds / last20.matches;
            double average_Headshot = last20.headshots / last20.matches;
            profile1Last20Death.Text = last20.deaths.ToString();
            profile1Last20Assists.Text = last20.assists.ToString();
            profile1Last20Kills.Text = last20.kills.ToString();
            profile1Last20KDr.Text = last20.kd_Ratio.ToString();
            profile1Last20KRr.Text = last20.kr_Ratio.ToString();
            profile1Last20Matches.Text = last20.matches.ToString();
            profile1Last20MVPs.Text = last20.mvps.ToString();
            profile1Last20Penta.Text = last20.penta_Kills.ToString();
            profile1Last20Quadro.Text = last20.quadro_Kills.ToString();
            profile1Last20Triple.Text = last20.triple_Kills.ToString();
            profile1Last20WinRate.Text = last20.win_Rate_Percantage.ToString() + "%";
            profile1Last20Wins.Text = last20.wins.ToString();
            profileLast20HS.Text = last20.headshots.ToString();
            profile1Last20Rounds.Text = last20.rounds.ToString();
            profileLast20HSperMatch.Text = last20.headshots_Per_match.ToString();
            profile1Last20avgRounds.Text = Math.Round(average_Rounds, 2).ToString();
            profile1Last20avgKills.Text = Math.Round(last20.average_Kills, 2).ToString();
            profile1Last20avgDeath.Text = Math.Round(last20.average_Deaths, 2).ToString();
            profile1Last20avgAssists.Text = Math.Round(last20.average_Assists, 2).ToString();
            profile1Last20avgHSs.Text = Math.Round(average_Headshot, 2).ToString();
            profile1Last20avgHSp.Text = last20.average_Headshot_Percentage.ToString() + "%";
            profile1Last20avgKDR.Text = Math.Round(last20.average_KD_Ratio, 2).ToString();
            profile1Last20avgKRR.Text = Math.Round(last20.average_KR_Ratio, 2).ToString();
            profile1Last20avgMVPs.Text = Math.Round(last20.average_MVPs, 2).ToString();
            profile1Last20avgPenta.Text = Math.Round(last20.average_Penta_Kills, 2).ToString();
            profile1Last20avgQuadro.Text = Math.Round(last20.average_Quadro_Kills, 2).ToString();
            profile1Last20avgTriple.Text = Math.Round(last20.average_Triple_Kills, 2).ToString();
        }

        //
        private void map1BoxProfile1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            UpdateMapBox();
            loginMapBox1Profile1_defaultStyle();
        }

        //
        private void map2BoxProfile1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            UpdateMapBox2();
            loginMapBox2Profile1_defaultStyle();
        }

        //
        private void getFriendInfoProfile1_Click_1(object sender, EventArgs e)
        {
            login2.Text = friendsBoxProfile1.Items[friendsBoxProfile1.SelectedIndex].ToString();
            getProfile2.PerformClick();
        }



        #endregion
        
        #region Login2

        // 
        public void ShowInfoProfile2Tab(PlayerCSGO acc)
        {
            if (isLogin2Loaded)
            {
                for (int i = 0; i < acc.detail.friends_Ids.Count; i++)
                {
                    if (acc.detail.friends_Ids[i] != null)
                    {
                        PlayerDetails pl_det = new PlayerDetails();
                        pl_det.faceit = faceit;
                        if(pl_det.GetMatchDetailsCSGO_PlayerID(acc.detail.friends_Ids[i], out CSGOPlayerDetails value))
                        {
                            friendsBoxProfile2.Items.Add(value.nickname);
                        }
                    }
                }
                avatarProfile2.Image = GetAvatar(acc.detail.avatar.ToString());
                levelProfile2.Text = acc.detail.skill_Level.ToString();
                eloProfile2.Text = acc.detail.faceit_Elo.ToString();
                nicknameProfile2.Text = acc.detail.nickname;
                countryProfile2.Text = acc.detail.country;
                membershipProfile2.Text = acc.detail.membership_Type;
                steamid64Profile2.Text = acc.detail.steam_Id_64.ToString();
                avgHSProfile2.Text = acc.statistics.average_Headshots_Percentage.ToString() + "%";
                avgKDProfile2.Text = acc.statistics.average_KD_Ratio.ToString();
                curWinStrikeProfile2.Text = acc.statistics.current_Win_Streak.ToString();
                longestWinStrikeProfile2.Text = acc.statistics.longest_Win_Streak.ToString();
                matchesProfile2.Text = acc.statistics.matches.ToString();
                winMatchesProfile2.Text = acc.statistics.wins.ToString();
                winRateProfile2.Text = acc.statistics.win_Rate.ToString() + "%";
                LoadMapInComboBox(map1BoxProfile2, acc.statistics.segments);
                LoadMapInComboBox(map2BoxProfile2, acc.statistics.segments);
                map1BoxProfile2.SelectedIndex = 0;
                map2BoxProfile2.SelectedIndex = 0;
            }
        }

        // Обновить информацию о map1
        public void UpdateMapBoxProfile2()
        {
            int index = map1BoxProfile2.SelectedIndex;
            modeProfile2Map1.Text = profile2.statistics.segments[index].mode;
            matchesProfile2Map1.Text = profile2.statistics.segments[index].matches.ToString();
            winsProfile2Map1.Text = profile2.statistics.segments[index].wins.ToString();
            winRateProfile2Map1.Text = profile2.statistics.segments[index].win_Rate_Percantage.ToString() + "%";
            roundsProfile2Map1.Text = profile2.statistics.segments[index].rounds.ToString();
            killsProfile2Map1.Text = profile2.statistics.segments[index].kills.ToString();
            deathProfile2Map1.Text = profile2.statistics.segments[index].deaths.ToString();
            kdRatioProfile2Map1.Text = profile2.statistics.segments[index].kd_Ratio.ToString();
            krRatioProfile2Map1.Text = profile2.statistics.segments[index].kr_Ratio.ToString();
            mvpsProfile2Map1.Text = profile2.statistics.segments[index].mvps.ToString();
            totalHSProfile2Map1.Text = profile2.statistics.segments[index].total_Headshot_Percentage.ToString();
            assistsProfile2Map1.Text = profile2.statistics.segments[index].assists.ToString();
            avgKDRatioProfile2Map1.Text = profile2.statistics.segments[index].average_KD_Ratio.ToString();
            avgKRRatioProfile2Map1.Text = profile2.statistics.segments[index].average_KR_Ratio.ToString();
            pentaKillsProfile2Map1.Text = profile2.statistics.segments[index].penta_Kills.ToString();
            avgPentaKillsProfile2Map1.Text = profile2.statistics.segments[index].average_Penta_Kills.ToString();
            quadroKillsProfile2Map1.Text = profile2.statistics.segments[index].quadro_Kills.ToString();
            avgQuadroKillsProfile2Map1.Text = profile2.statistics.segments[index].average_Quadro_Kills.ToString();
            tripleKillsProfile2Map1.Text = profile2.statistics.segments[index].triple_Kills.ToString();
            avgTripleKillsProfile2Map1.Text = profile2.statistics.segments[index].average_Triple_Kills.ToString();
            hsPerMatchProfile2Map1.Text = profile2.statistics.segments[index].headshots_Per_match.ToString();
            avgKillsProfile2Map1.Text = profile2.statistics.segments[index].average_Kills.ToString();
            avgDeathProfile2Map1.Text = profile2.statistics.segments[index].average_Deaths.ToString();
            avgMVPsProfile2Map1.Text = profile2.statistics.segments[index].average_MVPs.ToString();
            avgAssistsProfile2Map1.Text = profile2.statistics.segments[index].average_Assists.ToString();
            avgHSsProfile2Map1.Text = profile2.statistics.segments[index].average_Headshot_Percentage.ToString();
        }

        // Обновить информацию о map2
        public void UpdateMap2BoxProfile2()
        {
            int index = map2BoxProfile2.SelectedIndex;
            modeProfile2Map2.Text = profile2.statistics.segments[index].mode;
            matchesProfile2Map2.Text = profile2.statistics.segments[index].matches.ToString();
            winsProfile2Map2.Text = profile2.statistics.segments[index].wins.ToString();
            winRateProfile2Map2.Text = profile2.statistics.segments[index].win_Rate_Percantage.ToString() + "%";
            roundsProfile2Map2.Text = profile2.statistics.segments[index].rounds.ToString();
            killsProfile2Map2.Text = profile2.statistics.segments[index].kills.ToString();
            deathProfile2Map2.Text = profile2.statistics.segments[index].deaths.ToString();
            kdRatioProfile2Map2.Text = profile2.statistics.segments[index].kd_Ratio.ToString();
            krRatioProfile2Map2.Text = profile2.statistics.segments[index].kr_Ratio.ToString();
            mvpsProfile2Map2.Text = profile2.statistics.segments[index].mvps.ToString();
            totalHSProfile2Map2.Text = profile2.statistics.segments[index].total_Headshot_Percentage.ToString();
            assistsProfile2Map2.Text = profile2.statistics.segments[index].assists.ToString();
            avgKDRatioProfile2Map2.Text = profile2.statistics.segments[index].average_KD_Ratio.ToString();
            avgKRRatioProfile2Map2.Text = profile2.statistics.segments[index].average_KR_Ratio.ToString();
            pentaKillsProfile2Map2.Text = profile2.statistics.segments[index].penta_Kills.ToString();
            avgPentaKillsProfile2Map2.Text = profile2.statistics.segments[index].average_Penta_Kills.ToString();
            quadroKillsProfile2Map2.Text = profile2.statistics.segments[index].quadro_Kills.ToString();
            avgQuadroKillsProfile2Map2.Text = profile2.statistics.segments[index].average_Quadro_Kills.ToString();
            tripleKillsProfile2Map2.Text = profile2.statistics.segments[index].triple_Kills.ToString();
            avgTripleKillsProfile2Map2.Text = profile2.statistics.segments[index].average_Triple_Kills.ToString();
            hsPerMatchProfile2Map2.Text = profile2.statistics.segments[index].headshots_Per_match.ToString();
            avgKillsProfile2Map2.Text = profile2.statistics.segments[index].average_Kills.ToString();
            avgDeathProfile2Map2.Text = profile2.statistics.segments[index].average_Deaths.ToString();
            avgMVPsProfile2Map2.Text = profile2.statistics.segments[index].average_MVPs.ToString();
            avgAssistsProfile2Map2.Text = profile2.statistics.segments[index].average_Assists.ToString();
            avgHSsProfile2Map2.Text = profile2.statistics.segments[index].average_Headshot_Percentage.ToString();
        }

        // Показать информацию о map1

        private void map1BoxProfile2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            UpdateMapBoxProfile2();
            loginMapBox1Profile2_defaultStyle();
        }

        // Показать информацию о map2
        private void map2BoxProfile2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            UpdateMap2BoxProfile2();
            loginMapBox2Profile2_defaultStyle();
        }

        // Очистить табло логин2
        public void ClearProfile2Tab()
        {
            friendsBoxProfile2.Items.Clear();
            map1BoxProfile2.Items.Clear();
            map2BoxProfile2.Items.Clear();
        }

        // 
        private void compareMapsProfile2_Click(object sender, EventArgs e)
        {
            DisableStatsCompareProfile();
            loginProfileProfile2_defaultStyle();
            loginMapBox1Profile2_defaultStyle();
            loginMapBox2Profile2_defaultStyle();
            CompareMap1andMap2Profile2_Style();
            SelectBestMapProfile2();
        }

        // Загрузка login2
        private void metroButton4_Click(object sender, EventArgs e)
        {
            if (login2.Text == null) return;
            loadingLogin = loadedLogins.login2;
        }

        //
        private void getFriendInfoProfile2_Click_1(object sender, EventArgs e)
        {
            login1.Text = friendsBoxProfile2.Items[friendsBoxProfile2.SelectedIndex].ToString();
            getProfile1.PerformClick();
        }

        public void Load20MatchesProfile2History()
        {
            List<CSGOMatchStatsTeamPlayerStats> statLast20 = new List<CSGOMatchStatsTeamPlayerStats>();
            MatchStats match_Stats = new MatchStats();
            match_Stats.faceit = faceit;
            CSGOPlayerStatisticsProfileSegment last20Profile2 = new CSGOPlayerStatisticsProfileSegment();

            for (int i = 0; i < profile2.history.matches.Count; i++)
            {
                if (profile2.history.matches.Count == 19)
                {
                    CSGOMatchStatsTeamPlayerStats stats = new CSGOMatchStatsTeamPlayerStats();
                    if (match_Stats.GetMatchStatsCSGO(profile2.history.matches[i].match_Id, out CSGOMatchStats value))
                    {
                        stats = value.GetStatsPlayer(profile2.detail.player_Id);
                        last20Profile2.rounds += value.rounds;
                        statLast20.Add(stats);
                    }
                    else
                    {
                        Form2 form = new Form2();
                        form.errorMsg.Text = "Error: " + match_Stats.GetLastError;
                        form.Show();
                    }
                    break;
                }
                else
                {
                    CSGOMatchStatsTeamPlayerStats stats = new CSGOMatchStatsTeamPlayerStats();
                    if (match_Stats.GetMatchStatsCSGO(profile2.history.matches[i].match_Id, out CSGOMatchStats value))
                    {
                        stats = value.GetStatsPlayer(profile2.detail.player_Id);
                        last20Profile2.rounds += value.rounds;
                        statLast20.Add(stats);
                    }
                    else
                    {
                        Form2 form = new Form2();
                        form.errorMsg.Text = "Error: " + match_Stats.GetLastError;
                        form.Show();
                    }
                }
            }

            for (int i = 0; i < statLast20.Count; i++)
            {
                last20Profile2.assists += statLast20[i].assists;
                last20Profile2.deaths += statLast20[i].deaths;
                last20Profile2.headshots += statLast20[i].headshot;
                last20Profile2.total_Headshot_Percentage += statLast20[i].headshot_Percentage;
                last20Profile2.kd_Ratio += statLast20[i].kd_Ratio;
                last20Profile2.kills += statLast20[i].kills;
                last20Profile2.kr_Ratio += statLast20[i].kr_Ratio;
                last20Profile2.mvps += statLast20[i].mvps;
                last20Profile2.penta_Kills += statLast20[i].penta_Kills;
                last20Profile2.quadro_Kills += statLast20[i].quadro_Kills;
                last20Profile2.triple_Kills += statLast20[i].triple_Kills;
                last20Profile2.wins += statLast20[i].result;

            }
            last20Profile2.matches = statLast20.Count + 1;
            last20Profile2.average_Assists = last20Profile2.assists / last20Profile2.matches;
            last20Profile2.average_Deaths = last20Profile2.deaths / last20Profile2.matches;
            last20Profile2.average_Headshot_Percentage = last20Profile2.headshots * 100 / last20Profile2.matches;
            last20Profile2.average_KD_Ratio = last20Profile2.kd_Ratio / last20Profile2.matches;
            last20Profile2.average_KR_Ratio = last20Profile2.kr_Ratio / last20Profile2.matches;
            last20Profile2.average_Kills = last20Profile2.kills / last20Profile2.matches;
            last20Profile2.average_MVPs = last20Profile2.mvps / last20Profile2.matches;
            last20Profile2.average_Penta_Kills = last20Profile2.penta_Kills / last20Profile2.matches;
            last20Profile2.average_Quadro_Kills = last20Profile2.quadro_Kills / last20Profile2.matches;
            last20Profile2.average_Triple_Kills = last20Profile2.triple_Kills / last20Profile2.matches;
            last20Profile2.headshots_Per_match = last20Profile2.headshots / last20Profile2.matches;
            last20Profile2.win_Rate_Percantage += last20Profile2.wins * 100 / last20Profile2.matches;
            Combine20MatchesProfile2(last20Profile2);
        }
        public void Combine20MatchesProfile2(CSGOPlayerStatisticsProfileSegment last20)
        {
            /*profileLast20mode.Text = "";
            for (int i = 0; i < stats.Count; i++)
            {
                if (!profileLast20mode.Text.Contains(stats[i].mode))
                {
                    profileLast20mode.Text += '"' + stats[i].mode + '"' + " ";
                }
            }*/

            double average_Rounds = last20.rounds / last20.matches;
            double average_Headshot = last20.headshots / last20.matches;
            profile2Last20Death.Text = last20.deaths.ToString();
            profile2Last20Assists.Text = last20.assists.ToString();
            profile2Last20Kills.Text = last20.kills.ToString();
            profile2Last20KDr.Text = last20.kd_Ratio.ToString();
            profile2Last20KRr.Text = last20.kr_Ratio.ToString();
            profile2Last20Matches.Text = last20.matches.ToString();
            profile2Last20MVPs.Text = last20.mvps.ToString();
            profile2Last20Penta.Text = last20.penta_Kills.ToString();
            profile2Last20Quadro.Text = last20.quadro_Kills.ToString();
            profile2Last20Triple.Text = last20.triple_Kills.ToString();
            profile2Last20WinRate.Text = last20.win_Rate_Percantage.ToString() + "%";
            profile2Last20Wins.Text = last20.wins.ToString();
            profile2Last20HS.Text = last20.headshots.ToString();
            profile2Last20Rounds.Text = last20.rounds.ToString();
            profile2Last20HSperMatch.Text = last20.headshots_Per_match.ToString();
            profile2Last20avgRounds.Text = Math.Round(average_Rounds, 2).ToString();
            profile2Last20avgKills.Text = Math.Round(last20.average_Kills, 2).ToString();
            profile2Last20avgDeath.Text = Math.Round(last20.average_Deaths, 2).ToString();
            profile2Last20avgAssists.Text = Math.Round(last20.average_Assists, 2).ToString();
            profile2Last20avgHSs.Text = Math.Round(average_Headshot, 2).ToString();
            profile2Last20avgHSp.Text = last20.average_Headshot_Percentage.ToString() + "%";
            profile2Last20avgKDR.Text = Math.Round(last20.average_KD_Ratio, 2).ToString();
            profile2Last20avgKRR.Text = Math.Round(last20.average_KR_Ratio, 2).ToString();
            profile2Last20avgMVPs.Text = Math.Round(last20.average_MVPs, 2).ToString();
            profile2Last20avgPenta.Text = Math.Round(last20.average_Penta_Kills, 2).ToString();
            profile2Last20avgQuadro.Text = Math.Round(last20.average_Quadro_Kills, 2).ToString();
            profile2Last20avgTriple.Text = Math.Round(last20.average_Triple_Kills, 2).ToString();
        }

        #endregion

        #region Match

        public int GetAvgElo(List<CSGOMatchDetailsTeamPlayer> play_det, out int elo)
        {
            elo = 0;
            int mem = 0;
            for (int i = 0; i < play_det.Count; i++)
            {
                for (int k = 0; k < players.Count; k++)
                {
                    if (players[k].detail.player_Id == play_det[i].player_Id)
                    {
                        if(players[k].detail.faceit_Elo != 0)
                        {
                            elo += players[k].detail.faceit_Elo;
                            mem += 1;
                        }
                    }
                }
            }
            return elo / mem;
        }
        public int GetAvgRanking(List<CSGOMatchDetailsTeamPlayer> play_det, out int rank)
        {
            rank = 0;
            int mem = 0;
            for (int i = 0; i < play_det.Count; i++)
            {
                for (int k = 0; k < players.Count; k++)
                {
                    if (players[k].detail.player_Id == play_det[i].player_Id)
                    {
                        rank += players[k].ranking.position;
                        mem += 1;
                    }
                }
            }
            return rank / mem;
        }
        public double GetAvgHS(List<CSGOMatchDetailsTeamPlayer> play_det, out double avghs)
        {
            avghs = 0;
            double mem = 0;
            for (int i = 0; i < play_det.Count; i++)
            {
                for (int k = 0; k < players.Count; k++)
                {
                    if (players[k].detail.player_Id == play_det[i].player_Id)
                    {
                        avghs += players[k].statistics.average_Headshots_Percentage;
                        mem += 1;
                    }
                }
            }
            return avghs / mem;
        }
        public double GetAvgKD(List<CSGOMatchDetailsTeamPlayer> play_det, out double avgkd)
        {
            avgkd = 0;
            double mem = 0;
            for (int i = 0; i < play_det.Count; i++)
            {
                for (int k = 0; k < players.Count; k++)
                {
                    if (players[k].detail.player_Id == play_det[i].player_Id)
                    {
                        avgkd += players[k].statistics.average_KD_Ratio;
                        mem += 1;
                    }
                }
            }
            return avgkd / mem;
        }
        public double GetAvgMatches(List<CSGOMatchDetailsTeamPlayer> play_det, out double matches)
        {
            matches = 0;
            double mem = 0;
            for (int i = 0; i < play_det.Count; i++)
            {
                for (int k = 0; k < players.Count; k++)
                {
                    if (players[k].detail.player_Id == play_det[i].player_Id)
                    {
                        matches += players[k].statistics.matches;
                        mem += 1;
                    }
                }
            }
            return matches / mem;
        }
        public double GetAvgWinRate(List<CSGOMatchDetailsTeamPlayer> play_det, out double winrate)
        {
            winrate = 0;
            double mem = 0;
            for (int i = 0; i < play_det.Count; i++)
            {
                for (int k = 0; k < players.Count; k++)
                {
                    if (players[k].detail.player_Id == play_det[i].player_Id)
                    {
                        winrate += players[k].statistics.win_Rate;
                        mem += 1;
                    }
                }
            }
            return winrate / mem;
        }

        // Открыть в браузере и скачать демо файл
        private void downloadDemo1_Click(object sender, EventArgs e)
        {
            Process.Start(matchDetails.demo_Url[0].ToString());
        }
        // Открыть в браузере комнату матча
        private void OpenMatchInWeb1_Click(object sender, EventArgs e)
        {
            Process.Start(matchDetails.faceit_Url.ToString());
        }
        private string FromDateTimeToString(DateTime dt)
        {
            return dt.Hour + ":" + dt.Minute + ":" + dt.Second + " " + dt.Day + "." + dt.Month + "." + dt.Year;
        }
        // Показать информацию о матче
        private void ShowInfoMatch()
        {
            //LoadMembers();
            
            //настройка матча
            teamnameA.Text = matchDetails.faction1.name;
            teamnameB.Text = matchDetails.faction2.name;
            scoreTeamA.Text = matchDetails.score_Faction1.ToString();
            scoreTeamB.Text = matchDetails.score_Faction2.ToString();
            statusMatch1.Text = matchDetails.status;

            mapName1.Text = string.Empty;
            for (int i = 0; i < matchDetails.map_Pick.Count; i++)
            {
                mapName1.Text += matchDetails.map_Pick[i];
            }

            gameType1.Text = matchDetails.competition_Type;
            matchBestOf1.Text = matchDetails.best_Of.ToString();
            countryServer1.Text = "";
            for (int i = 0; i < matchDetails.location_Entities.Count; i++)
            {
                if(matchDetails.location_Entities[i].name == matchDetails.location_Pick[0])
                {
                    countryServerImg1.Image = GetImageServer((matchDetails.location_Entities[i].image_Lg.ToString()));
                }
            }
            for (int i = 0; i < matchDetails.location_Pick.Count; i++)
            {
                countryServer1.Text += matchDetails.location_Pick[i];
            }
            
            scoreTeamA.Text = matchStatistics.team_A.final_score.ToString();
            scoreTeamB.Text = matchStatistics.team_B.final_score.ToString();
            configuredDate1.Text = FromDateTimeToString(matchDetails.configured_At);
            startedDate1.Text = FromDateTimeToString(matchDetails.started_At);
            finishedDate1.Text = FromDateTimeToString(matchDetails.finished_At);
            
            
            int memberOldA = 0;
            List<CSGOMatchDetailsTeamPlayer> membOldA = new List<CSGOMatchDetailsTeamPlayer>();

            for (int i = 0; i < matchDetails.faction1.roster.Count; i++)
            {
                membOldA.Add(matchDetails.faction1.roster[i]);
            }
            for (int i = 0; i < membOldA.Count; i++)
            {
                if (membOldA[i].game_Skill_Level != 0)
                {
                    memberOldA += 1;
                }
            }

            int memberOldB = 0;
            List<CSGOMatchDetailsTeamPlayer> membOldB = new List<CSGOMatchDetailsTeamPlayer>();

            for (int i = 0; i < matchDetails.faction2.roster.Count; i++)
            {
                membOldB.Add(matchDetails.faction2.roster[i]);
            }
            for (int i = 0; i < membOldB.Count; i++)
            {
                if (membOldB[i].game_Skill_Level != 0)
                {
                    memberOldB += 1;
                }
            }
            
            avgEloTeamA.Text = GetAvgElo(membOldA, out int eloA).ToString();
            avgHSTeamA.Text = GetAvgHS(membOldA, out double avghsA).ToString() + "%";
            avgKDTeamA.Text = GetAvgKD(membOldA, out double avgkdA).ToString();
            avgRankingTeamA.Text = GetAvgRanking(membOldA, out int rankA).ToString();
            avgMatchesTeamA.Text = GetAvgMatches(membOldA, out double matchesA).ToString();
            avgWinRateTeamA.Text = GetAvgWinRate(membOldA, out double winrateA).ToString() + "%";

            avgEloTeamB.Text = GetAvgElo(membOldB, out int eloB).ToString();
            avgHSTeamB.Text = GetAvgHS(membOldB, out double avghsB).ToString() + "%";
            avgKDTeamB.Text = GetAvgKD(membOldB, out double avgkdB).ToString();
            avgRankingTeamB.Text = GetAvgRanking(membOldB, out int rankB).ToString();
            avgMatchesTeamB.Text = GetAvgMatches(membOldB, out double matchesB).ToString();
            avgWinRateTeamB.Text = GetAvgWinRate(membOldB, out double winrateB).ToString() + "%";
            
            if (eloA > eloB)
            {
                avgEloTeamB.Style = MetroFramework.MetroColorStyle.Red;
                avgEloTeamA.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (eloA < eloB)
            {
                avgEloTeamB.Style = MetroFramework.MetroColorStyle.Green;
                avgEloTeamA.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgEloTeamB.Style = MetroFramework.MetroColorStyle.Blue;
                avgEloTeamA.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (avghsA > avghsB)
            {
                avgHSTeamB.Style = MetroFramework.MetroColorStyle.Red;
                avgHSTeamA.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (avghsA < avghsB)
            {
                avgHSTeamB.Style = MetroFramework.MetroColorStyle.Green;
                avgHSTeamA.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgHSTeamB.Style = MetroFramework.MetroColorStyle.Blue;
                avgHSTeamA.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (avgkdA > avgkdB)
            {
                avgKDTeamB.Style = MetroFramework.MetroColorStyle.Red;
                avgKDTeamA.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (avgkdA < avgkdB)
            {
                avgKDTeamB.Style = MetroFramework.MetroColorStyle.Green;
                avgKDTeamA.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKDTeamB.Style = MetroFramework.MetroColorStyle.Blue;
                avgKDTeamA.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (rankA < rankB)
            {
                avgRankingTeamB.Style = MetroFramework.MetroColorStyle.Red;
                avgRankingTeamA.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (rankA > rankB)
            {
                avgRankingTeamB.Style = MetroFramework.MetroColorStyle.Green;
                avgRankingTeamA.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgRankingTeamB.Style = MetroFramework.MetroColorStyle.Blue;
                avgRankingTeamA.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (matchesA > matchesB)
            {
                avgMatchesTeamB.Style = MetroFramework.MetroColorStyle.Red;
                avgMatchesTeamA.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (matchesA < matchesB)
            {
                avgMatchesTeamB.Style = MetroFramework.MetroColorStyle.Green;
                avgMatchesTeamA.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgMatchesTeamB.Style = MetroFramework.MetroColorStyle.Blue;
                avgMatchesTeamA.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (winrateA > winrateB)
            {
                avgWinRateTeamB.Style = MetroFramework.MetroColorStyle.Red;
                avgWinRateTeamA.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (winrateA < winrateB)
            {
                avgWinRateTeamB.Style = MetroFramework.MetroColorStyle.Green;
                avgWinRateTeamA.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgWinRateTeamB.Style = MetroFramework.MetroColorStyle.Blue;
                avgWinRateTeamA.Style = MetroFramework.MetroColorStyle.Blue;
            }
            
        }

        private void ShowStatMatch()
        {
            teamAFinalScore.Text = matchStatistics.team_A.final_score.ToString();
            teamBFinalScore.Text = matchStatistics.team_B.final_score.ToString();
            if (matchStatistics.team_A.final_score > matchStatistics.team_B.final_score)
            {
                teamAFinalScore.Style = MetroFramework.MetroColorStyle.Green;
                teamBFinalScore.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                teamAFinalScore.Style = MetroFramework.MetroColorStyle.Red;
                teamBFinalScore.Style = MetroFramework.MetroColorStyle.Green;
            }

            teamAFirstHalfScore.Text = matchStatistics.team_A.first_Half_Score.ToString();
            teamBFirstHalfScore.Text = matchStatistics.team_B.first_Half_Score.ToString();
            if (matchStatistics.team_A.first_Half_Score > matchStatistics.team_B.first_Half_Score)
            {
                teamAFirstHalfScore.Style = MetroFramework.MetroColorStyle.Green;
                teamBFirstHalfScore.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                teamAFirstHalfScore.Style = MetroFramework.MetroColorStyle.Red;
                teamBFirstHalfScore.Style = MetroFramework.MetroColorStyle.Green;
            }

            teamASecondHalfScore.Text = matchStatistics.team_A.second_Half_Score.ToString();
            teamBSecondHalfScore.Text = matchStatistics.team_B.second_Half_Score.ToString();
            if (matchStatistics.team_A.second_Half_Score > matchStatistics.team_B.second_Half_Score)
            {
                teamASecondHalfScore.Style = MetroFramework.MetroColorStyle.Green;
                teamBSecondHalfScore.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                teamASecondHalfScore.Style = MetroFramework.MetroColorStyle.Red;
                teamBSecondHalfScore.Style = MetroFramework.MetroColorStyle.Green;
            }

            teamAOTScore.Text = matchStatistics.team_A.overtime_Score.ToString();
            teamBOTScore.Text = matchStatistics.team_B.overtime_Score.ToString();
            if (matchStatistics.team_A.overtime_Score > matchStatistics.team_B.overtime_Score)
            {
                teamAOTScore.Style = MetroFramework.MetroColorStyle.Green;
                teamBOTScore.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (matchStatistics.team_A.overtime_Score < matchStatistics.team_B.overtime_Score)
            {
                teamAOTScore.Style = MetroFramework.MetroColorStyle.Red;
                teamBOTScore.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                teamAOTScore.Style = MetroFramework.MetroColorStyle.Blue;
                teamBOTScore.Style = MetroFramework.MetroColorStyle.Blue;
            }

            teamATeamHS.Text = matchStatistics.team_A.team_Headshot.ToString();
            teamBTeamHS.Text = matchStatistics.team_B.team_Headshot.ToString();
            if (matchStatistics.team_A.team_Headshot > matchStatistics.team_B.team_Headshot)
            {
                teamATeamHS.Style = MetroFramework.MetroColorStyle.Green;
                teamBTeamHS.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (matchStatistics.team_A.team_Headshot < matchStatistics.team_B.team_Headshot)
            {
                teamATeamHS.Style = MetroFramework.MetroColorStyle.Red;
                teamBTeamHS.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                teamATeamHS.Style = MetroFramework.MetroColorStyle.Blue;
                teamBTeamHS.Style = MetroFramework.MetroColorStyle.Blue;
            }

            teamAResult.Text = matchStatistics.team_A.team_Win.ToString();
            teamBResult.Text = matchStatistics.team_B.team_Win.ToString();
            if (matchStatistics.team_A.team_Win > matchStatistics.team_B.team_Win)
            {
                teamAResult.Style = MetroFramework.MetroColorStyle.Green;
                teamBResult.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                teamAResult.Style = MetroFramework.MetroColorStyle.Red;
                teamBResult.Style = MetroFramework.MetroColorStyle.Green;
            }

            teamAPremade.Text = matchStatistics.team_A.premade.ToString();
            teamBPremade.Text = matchStatistics.team_B.premade.ToString();
            if (matchStatistics.team_A.premade == matchStatistics.team_B.premade)
            {
                teamAPremade.Style = MetroFramework.MetroColorStyle.Blue;
                teamBPremade.Style = MetroFramework.MetroColorStyle.Blue;
            }
            else
            {
                teamAPremade.Style = MetroFramework.MetroColorStyle.Green;
                teamBPremade.Style = MetroFramework.MetroColorStyle.Green;
            }

            List<CSGOMatchStatsTeamPlayer> pl_stats = new List<CSGOMatchStatsTeamPlayer>();
            foreach(CSGOMatchStatsTeamPlayer pl in matchStatistics.team_A.players)
            {
                pl_stats.Add(pl);
            }
            foreach (CSGOMatchStatsTeamPlayer pl in matchStatistics.team_B.players)
            {
                pl_stats.Add(pl);
            }

            for (int i = 0; i < pl_stats.Count; i++)
            {
                sNickname[i].Text = pl_stats[i].nickname;
                sKills[i].Text = pl_stats[i].stats.kills.ToString();
                sAssists[i].Text = pl_stats[i].stats.assists.ToString();
                sDeaths[i].Text = pl_stats[i].stats.deaths.ToString();
                sPentaKills[i].Text = pl_stats[i].stats.penta_Kills.ToString();
                sQuadroKills[i].Text = pl_stats[i].stats.quadro_Kills.ToString();
                sTripleKills[i].Text = pl_stats[i].stats.triple_Kills.ToString();
                sAvgKR[i].Text = pl_stats[i].stats.kr_Ratio.ToString();
                sAvgKD[i].Text = pl_stats[i].stats.kd_Ratio.ToString();
                sMVP[i].Text = pl_stats[i].stats.mvps.ToString();
                sHS[i].Text = pl_stats[i].stats.headshot.ToString() + "(" + pl_stats[i].stats.headshot_Percentage + "%)";
                if (pl_stats[i].player_Id == matchDetails.faction1.leader)
                {
                    sNickname[i].Style = MetroFramework.MetroColorStyle.Blue;
                }
                else if (pl_stats[i].player_Id == matchDetails.faction2.leader)
                {
                    sNickname[i].Style = MetroFramework.MetroColorStyle.Blue;
                }
                else
                {
                    sNickname[i].Style = MetroFramework.MetroColorStyle.Black;
                }
            }
        }

        #endregion

        #region Style

        #region Profile1
        // Загрузка обычных стилей о map1Profile2
        public void loginMapBox1Profile1_defaultStyle()
        {
            winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            modeProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            pentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Black;

        }
        // Загрузка обычных стилей о map2Profile2
        public void loginMapBox2Profile1_defaultStyle()
        {
            modeProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Black;
        }
        // Загрузка обычных стилей о profile
        public void loginProfileProfile1_defaultStyle()
        {
            eloProfile1.Style = MetroFramework.MetroColorStyle.Black;
            levelProfile1.Style = MetroFramework.MetroColorStyle.Black;
            countryProfile1.Style = MetroFramework.MetroColorStyle.Black;
            membershipProfile1.Style = MetroFramework.MetroColorStyle.Black;
            avgHSProfile1.Style = MetroFramework.MetroColorStyle.Black;
            avgKDProfile1.Style = MetroFramework.MetroColorStyle.Black;
            curWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Black;
            longestWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Black;
            matchesProfile1.Style = MetroFramework.MetroColorStyle.Black;
            winRateProfile1.Style = MetroFramework.MetroColorStyle.Black;
            winMatchesProfile1.Style = MetroFramework.MetroColorStyle.Black;
        }
        // Загрузить стили сравнения карт
        public void CompareMap1andMap2Profile1_Style()
        {
            int index1 = map1BoxProfile1.SelectedIndex;
            int index2 = map2BoxProfile1.SelectedIndex;
            CSGOPlayerStatisticsProfileSegment map1 = profile.statistics.segments[index1];
            CSGOPlayerStatisticsProfileSegment map2 = profile.statistics.segments[index2];

            if (map1.wins > map2.wins)
            {
                winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.wins < map2.wins)
            {
                winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.matches > map2.matches)
            {
                matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.matches < map2.matches)
            {
                matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.win_Rate_Percantage > map2.win_Rate_Percantage)
            {
                winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.win_Rate_Percantage < map2.win_Rate_Percantage)
            {
                winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kr_Ratio > map2.kr_Ratio)
            {
                kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.kr_Ratio < map2.kr_Ratio)
            {
                kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Headshot_Percentage > map2.average_Headshot_Percentage)
            {
                avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Headshot_Percentage < map2.average_Headshot_Percentage)
            {
                avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mode == map2.mode)
            {
                modeProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                modeProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                modeProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                modeProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }

            if (map1.rounds > map2.rounds)
            {
                roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.rounds < map2.rounds)
            {
                roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kills > map2.kills)
            {
                killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.kills < map2.kills)
            {
                killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.deaths < map2.deaths)
            {
                deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.deaths > map2.deaths)
            {
                deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kr_Ratio > map2.kr_Ratio)
            {
                krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.kr_Ratio < map2.kr_Ratio)
            {
                krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mvps > map2.mvps)
            {
                mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.mvps < map2.mvps)
            {
                mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.assists > map2.assists)
            {
                assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.assists < map2.assists)
            {
                assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.penta_Kills > map2.penta_Kills)
            {
                pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                pentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.penta_Kills < map2.penta_Kills)
            {
                pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                pentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                pentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.quadro_Kills > map2.quadro_Kills)
            {
                quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.quadro_Kills < map2.quadro_Kills)
            {
                quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.triple_Kills > map2.triple_Kills)
            {
                tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.triple_Kills < map2.triple_Kills)
            {
                tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.headshots_Per_match > map2.headshots_Per_match)
            {
                hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.headshots_Per_match < map2.headshots_Per_match)
            {
                hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Kills > map2.average_Kills)
            {
                avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Kills < map2.average_Kills)
            {
                avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Deaths < map2.average_Deaths)
            {
                avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Deaths > map2.average_Deaths)
            {
                avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.total_Headshot_Percentage > map2.total_Headshot_Percentage)
            {
                totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.total_Headshot_Percentage < map2.total_Headshot_Percentage)
            {
                totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Assists > map2.average_Assists)
            {
                avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Assists < map2.average_Assists)
            {
                avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_MVPs > map2.average_MVPs)
            {
                avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_MVPs < map2.average_MVPs)
            {
                avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Penta_Kills > map2.average_Penta_Kills)
            {
                avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Penta_Kills < map2.average_Penta_Kills)
            {
                avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Quadro_Kills > map2.average_Quadro_Kills)
            {
                avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Quadro_Kills < map2.average_Quadro_Kills)
            {
                avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Triple_Kills > map2.average_Triple_Kills)
            {
                avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Triple_Kills < map2.average_Triple_Kills)
            {
                avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_KR_Ratio > map2.average_KR_Ratio)
            {
                avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_KR_Ratio < map2.average_KR_Ratio)
            {
                avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_KD_Ratio > map2.average_KD_Ratio)
            {
                avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_KD_Ratio < map2.average_KD_Ratio)
            {
                avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

        }
        // Сравнение карт
        public void SelectBestMapProfile1()
        {
            CSGOPlayerStatisticsProfileSegment map1 = profile.statistics.segments[map1BoxProfile1.SelectedIndex];
            CSGOPlayerStatisticsProfileSegment map2 = profile.statistics.segments[map2BoxProfile1.SelectedIndex];

            int indexMap1 = 0;
            int indexMap2 = 0;

            if (map1.matches < map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.matches > map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.wins < map2.wins)
            {
                indexMap2 += 1;
            }
            else if (map1.wins > map2.wins)
            {
                indexMap1 += 1;
            }

            if (map1.win_Rate_Percantage < map2.win_Rate_Percantage)
            {
                indexMap2 += 1;
            }
            else if (map1.win_Rate_Percantage > map2.win_Rate_Percantage)
            {
                indexMap1 += 1;
            }

            if (map1.kd_Ratio < map2.kd_Ratio)
            {
                indexMap2 += 1;
            }
            else if (map1.kd_Ratio > map2.kd_Ratio)
            {
                indexMap1 += 1;
            }

            if (map1.average_Headshot_Percentage < map2.average_Headshot_Percentage)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Headshot_Percentage > map2.average_Headshot_Percentage)
            {
                indexMap1 += 1;
            }

            if (map1.rounds < map2.rounds)
            {
                indexMap2 += 1;
            }
            else if (map1.rounds > map2.rounds)
            {
                indexMap1 += 1;
            }

            if (map1.kills < map2.kills)
            {
                indexMap2 += 1;
            }
            else if (map1.kills > map2.kills)
            {
                indexMap1 += 1;
            }

            if (map1.deaths > map2.deaths)
            {
                indexMap2 += 1;
            }
            else if (map1.deaths < map2.deaths)
            {
                indexMap1 += 1;
            }

            if (map1.kr_Ratio < map2.kr_Ratio)
            {
                indexMap2 += 1;
            }
            else if (map1.kr_Ratio > map2.kr_Ratio)
            {
                indexMap1 += 1;
            }

            if (map1.mvps < map2.mvps)
            {
                indexMap2 += 1;
            }
            else if (map1.mvps > map2.mvps)
            {
                indexMap1 += 1;
            }

            if (map1.assists < map2.assists)
            {
                indexMap2 += 1;
            }
            else if (map1.assists > map2.assists)
            {
                indexMap1 += 1;
            }

            if (map1.penta_Kills < map2.penta_Kills)
            {
                indexMap2 += 1;
            }
            else if (map1.penta_Kills > map2.penta_Kills)
            {
                indexMap1 += 1;
            }

            if (map1.quadro_Kills < map2.quadro_Kills)
            {
                indexMap2 += 1;
            }
            else if (map1.quadro_Kills > map2.quadro_Kills)
            {
                indexMap1 += 1;
            }

            if (map1.triple_Kills < map2.triple_Kills)
            {
                indexMap2 += 1;
            }
            else if (map1.triple_Kills > map2.triple_Kills)
            {
                indexMap1 += 1;
            }

            if (map1.headshots_Per_match < map2.headshots_Per_match)
            {
                indexMap2 += 1;
            }
            else if (map1.headshots_Per_match > map2.headshots_Per_match)
            {
                indexMap1 += 1;
            }

            if (map1.average_Kills < map2.average_Kills)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Kills > map2.average_Kills)
            {
                indexMap1 += 1;
            }

            if (map1.average_Deaths > map2.average_Deaths)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Deaths < map2.average_Deaths)
            {
                indexMap1 += 1;
            }

            if (map1.average_Assists < map2.average_Assists)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Assists > map2.average_Assists)
            {
                indexMap1 += 1;
            }

            if (map1.average_MVPs < map2.average_MVPs)
            {
                indexMap2 += 1;
            }
            else if (map1.average_MVPs > map2.average_MVPs)
            {
                indexMap1 += 1;
            }

            if (map1.average_Penta_Kills < map2.average_Penta_Kills)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Penta_Kills > map2.average_Penta_Kills)
            {
                indexMap1 += 1;
            }

            if (map1.average_Quadro_Kills < map2.average_Quadro_Kills)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Quadro_Kills > map2.average_Quadro_Kills)
            {
                indexMap1 += 1;
            }

            if (map1.average_Triple_Kills < map2.average_Triple_Kills)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Triple_Kills > map2.average_Triple_Kills)
            {
                indexMap1 += 1;
            }

            if (map1.average_KD_Ratio < map2.average_KD_Ratio)
            {
                indexMap2 += 1;
            }
            else if (map1.average_KD_Ratio > map2.average_KD_Ratio)
            {
                indexMap1 += 1;
            }

            if (map1.average_KR_Ratio < map2.average_KR_Ratio)
            {
                indexMap2 += 1;
            }
            else if (map1.average_KR_Ratio > map2.average_KR_Ratio)
            {
                indexMap1 += 1;
            }

            if (indexMap1 > indexMap2)
            {
                bestMapProfile1Map1.Text = "Best";
                bestMapProfile1Map2.Text = "Worst";
                bestMapProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                bestMapProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (indexMap1 < indexMap2)
            {
                bestMapProfile1Map1.Text = "Worst";
                bestMapProfile1Map2.Text = "Best";
                bestMapProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                bestMapProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                bestMapProfile1Map1.Text = "Best";
                bestMapProfile1Map2.Text = "Best";
                bestMapProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
                bestMapProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }
            metro.Visible = true;
            bestMapProfile1Map1.Visible = true;
            bestMapProfile1Map2.Visible = true;
        }
        #endregion

        #region Profile2

        // Загрузка обычных стилей о map1Profile2
        public void loginMapBox1Profile2_defaultStyle()
        {
            winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            modeProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;
            avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Black;

        }
        // Загрузка обычных стилей о map2Profile2
        public void loginMapBox2Profile2_defaultStyle()
        {
            modeProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
            avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Black;
        }
        // Загрузка обычных стилей о profile
        public void loginProfileProfile2_defaultStyle()
        {
            eloProfile2.Style = MetroFramework.MetroColorStyle.Black;
            levelProfile2.Style = MetroFramework.MetroColorStyle.Black;
            countryProfile2.Style = MetroFramework.MetroColorStyle.Black;
            membershipProfile2.Style = MetroFramework.MetroColorStyle.Black;
            avgHSProfile2.Style = MetroFramework.MetroColorStyle.Black;
            avgKDProfile2.Style = MetroFramework.MetroColorStyle.Black;
            curWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Black;
            longestWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Black;
            matchesProfile2.Style = MetroFramework.MetroColorStyle.Black;
            winRateProfile2.Style = MetroFramework.MetroColorStyle.Black;
            winMatchesProfile2.Style = MetroFramework.MetroColorStyle.Black;
        }
        // Загрузить стили сравнения карт
        public void CompareMap1andMap2Profile2_Style()
        {
            int index1 = map1BoxProfile1.SelectedIndex;
            int index2 = map2BoxProfile1.SelectedIndex;
            CSGOPlayerStatisticsProfileSegment map1 = profile.statistics.segments[index1];
            CSGOPlayerStatisticsProfileSegment map2 = profile.statistics.segments[index2];

            if (map1.wins > map2.wins)
            {
                winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.wins < map2.wins)
            {
                winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.matches > map2.matches)
            {
                matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.matches < map2.matches)
            {
                matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.win_Rate_Percantage > map2.win_Rate_Percantage)
            {
                winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.win_Rate_Percantage < map2.win_Rate_Percantage)
            {
                winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kd_Ratio > map2.kd_Ratio)
            {
                kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.kd_Ratio < map2.kd_Ratio)
            {
                kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Headshot_Percentage > map2.average_Headshot_Percentage)
            {
                avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Headshot_Percentage < map2.average_Headshot_Percentage)
            {
                avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mode == map2.mode)
            {
                modeProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                modeProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                modeProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                modeProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }

            if (map1.rounds > map2.rounds)
            {
                roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.rounds < map2.rounds)
            {
                roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kills > map2.kills)
            {
                killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.kills < map2.kills)
            {
                killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.deaths < map2.deaths)
            {
                deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.deaths > map2.deaths)
            {
                deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kr_Ratio > map2.kr_Ratio)
            {
                krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.kr_Ratio < map2.kr_Ratio)
            {
                krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mvps > map2.mvps)
            {
                mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.mvps < map2.mvps)
            {
                mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.assists > map2.assists)
            {
                assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.assists < map2.assists)
            {
                assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.penta_Kills > map2.penta_Kills)
            {
                pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.penta_Kills < map2.penta_Kills)
            {
                pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.quadro_Kills > map2.quadro_Kills)
            {
                quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.quadro_Kills < map2.quadro_Kills)
            {
                quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.triple_Kills > map2.triple_Kills)
            {
                tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.triple_Kills < map2.triple_Kills)
            {
                tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.headshots_Per_match > map2.headshots_Per_match)
            {
                hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.headshots_Per_match < map2.headshots_Per_match)
            {
                hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Kills > map2.average_Kills)
            {
                avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Kills < map2.average_Kills)
            {
                avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Deaths < map2.average_Deaths)
            {
                avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Deaths > map2.average_Deaths)
            {
                avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.total_Headshot_Percentage > map2.total_Headshot_Percentage)
            {
                totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.total_Headshot_Percentage < map2.total_Headshot_Percentage)
            {
                totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Assists > map2.average_Assists)
            {
                avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Assists < map2.average_Assists)
            {
                avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_MVPs > map2.average_MVPs)
            {
                avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_MVPs < map2.average_MVPs)
            {
                avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Penta_Kills > map2.average_Penta_Kills)
            {
                avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Penta_Kills < map2.average_Penta_Kills)
            {
                avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Quadro_Kills > map2.average_Quadro_Kills)
            {
                avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Quadro_Kills < map2.average_Quadro_Kills)
            {
                avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Triple_Kills > map2.average_Triple_Kills)
            {
                avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_Triple_Kills < map2.average_Triple_Kills)
            {
                avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_KR_Ratio > map2.average_KR_Ratio)
            {
                avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_KR_Ratio < map2.average_KR_Ratio)
            {
                avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_KD_Ratio > map2.average_KD_Ratio)
            {
                avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (map1.average_KD_Ratio < map2.average_KD_Ratio)
            {
                avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
            }
            else
            {
                avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

        }
        // Сравнение карт
        public void SelectBestMapProfile2()
        {
            CSGOPlayerStatisticsProfileSegment map1 = profile.statistics.segments[map1BoxProfile1.SelectedIndex];
            CSGOPlayerStatisticsProfileSegment map2 = profile.statistics.segments[map2BoxProfile1.SelectedIndex];

            int indexMap1 = 0;
            int indexMap2 = 0;

            if (map1.matches < map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.matches > map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.wins/map1.matches < map2.wins / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.wins / map1.matches > map2.wins / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.win_Rate_Percantage / map1.matches < map2.win_Rate_Percantage / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.win_Rate_Percantage / map1.matches > map2.win_Rate_Percantage / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.kd_Ratio / map1.matches / map1.matches < map2.kd_Ratio / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.kd_Ratio / map1.matches / map1.matches > map2.kd_Ratio / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.average_Headshot_Percentage / map1.matches < map2.average_Headshot_Percentage / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Headshot_Percentage / map1.matches > map2.average_Headshot_Percentage / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.rounds / map1.matches / map1.matches < map2.rounds / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.rounds / map1.matches / map1.matches > map2.rounds / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.kills / map1.matches < map2.kills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.kills / map1.matches > map2.kills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.deaths / map1.matches > map2.deaths / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.deaths / map1.matches < map2.deaths / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.kr_Ratio / map1.matches < map2.kr_Ratio / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.kr_Ratio / map1.matches > map2.kr_Ratio / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.mvps / map1.matches < map2.mvps / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.mvps / map1.matches > map2.mvps / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.assists / map1.matches < map2.assists / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.assists / map1.matches > map2.assists / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.penta_Kills / map1.matches < map2.penta_Kills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.penta_Kills / map1.matches > map2.penta_Kills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.quadro_Kills / map1.matches < map2.quadro_Kills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.quadro_Kills / map1.matches > map2.quadro_Kills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.triple_Kills / map1.matches < map2.triple_Kills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.triple_Kills / map1.matches > map2.triple_Kills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.headshots_Per_match / map1.matches < map2.headshots_Per_match / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.headshots_Per_match / map1.matches > map2.headshots_Per_match / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.average_Kills / map1.matches < map2.average_Kills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Kills / map1.matches > map2.average_Kills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.average_Deaths / map1.matches > map2.average_Deaths / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Deaths / map1.matches < map2.average_Deaths / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.average_Assists / map1.matches < map2.average_Assists / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Assists / map1.matches > map2.average_Assists / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.average_MVPs / map1.matches < map2.average_MVPs / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.average_MVPs / map1.matches > map2.average_MVPs / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.average_Penta_Kills / map1.matches < map2.average_Penta_Kills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Penta_Kills / map1.matches > map2.average_Penta_Kills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.average_Quadro_Kills / map1.matches < map2.average_Quadro_Kills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Quadro_Kills / map1.matches > map2.average_Quadro_Kills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.average_Triple_Kills / map1.matches < map2.average_Triple_Kills / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.average_Triple_Kills / map1.matches > map2.average_Triple_Kills / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.average_KD_Ratio / map1.matches < map2.average_KD_Ratio / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.average_KD_Ratio / map1.matches > map2.average_KD_Ratio / map2.matches)
            {
                indexMap1 += 1;
            }

            if (map1.average_KR_Ratio / map1.matches < map2.average_KR_Ratio / map2.matches)
            {
                indexMap2 += 1;
            }
            else if (map1.average_KR_Ratio / map1.matches > map2.average_KR_Ratio / map2.matches)
            {
                indexMap1 += 1;
            }

            if (indexMap1 > indexMap2)
            {
                bestMapProfile2Map1.Text = "Best";
                bestMapProfile2Map2.Text = "Worst";
                bestMapProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                bestMapProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (indexMap1 < indexMap2)
            {
                bestMapProfile2Map1.Text = "Worst";
                bestMapProfile2Map2.Text = "Best";
                bestMapProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                bestMapProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                bestMapProfile2Map1.Text = "Best";
                bestMapProfile2Map2.Text = "Best";
                bestMapProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                bestMapProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }
            metroLabel74.Visible = true;
            bestMapProfile2Map1.Visible = true;
            bestMapProfile2Map2.Visible = true;
        }

        #endregion

        
        public void EnableStatsCompareProfile()
        {
            metroLabel74.Visible = true;
            metro.Visible = true;
            bestMapProfile1Map1.Visible = true;
            bestMapProfile1Map2.Visible = true;
            bestMapProfile2Map1.Visible = true;
            bestMapProfile2Map2.Visible = true;

            profileProfile1Map1.Visible = true;
            profileProfile1Map2.Visible = true;
            profileProfile2Map1.Visible = true;
            profileProfile2Map2.Visible = true;

            profileAllProfile1Map1.Visible = true;
            profileAllProfile1Map2.Visible = true;
            profileAllProfile2Map1.Visible = true;
            profileAllProfile2Map2.Visible = true;

            profileMap1Profile1Map1.Visible = true;
            profileMap1Profile1Map2.Visible = true;
            profileMap1Profile2Map1.Visible = true;
            profileMap1Profile2Map2.Visible = true;

            profileMap2Profile1Map1.Visible = true;
            profileMap2Profile1Map2.Visible = true;
            profileMap2Profile2Map1.Visible = true;
            profileMap2Profile2Map2.Visible = true;

            profileLast20Profile1Map1.Visible = true;
            profileLast20Profile1Map2.Visible = true;
            profileLast20Profile2Map1.Visible = true;
            profileLast20Profile2Map2.Visible = true;

            //Для профиля #1
            metroLabel85.Visible = true;
            metroLabel86.Visible = true;
            metroLabel101.Visible = true;
            metroLabel78.Visible = true;
            metroLabel90.Visible = true;
            metroLabel70.Visible = true;
            metroLabel2.Visible = true;

            //Для профиля #2
            metroLabel55.Visible = true;
            metroLabel57.Visible = true;
            metroLabel62.Visible = true;
            metroLabel49.Visible = true;
            metroLabel65.Visible = true;
            metroLabel42.Visible = true;
            metroLabel33.Visible = true;
        }

        public void DisableStatsCompareProfile()
        {
            metroLabel74.Visible = false;
            metro.Visible = false;
            bestMapProfile1Map1.Visible = false;
            bestMapProfile1Map2.Visible = false;
            bestMapProfile2Map1.Visible = false;
            bestMapProfile2Map2.Visible = false;

            profileProfile1Map1.Visible = false;
            profileProfile1Map2.Visible = false;
            profileProfile2Map1.Visible = false;
            profileProfile2Map2.Visible = false;

            profileAllProfile1Map1.Visible = false;
            profileAllProfile1Map2.Visible = false;
            profileAllProfile2Map1.Visible = false;
            profileAllProfile2Map2.Visible = false;

            profileMap1Profile1Map1.Visible = false;
            profileMap1Profile1Map2.Visible = false;
            profileMap1Profile2Map1.Visible = false;
            profileMap1Profile2Map2.Visible = false;

            profileMap2Profile1Map1.Visible = false;
            profileMap2Profile1Map2.Visible = false;
            profileMap2Profile2Map1.Visible = false;
            profileMap2Profile2Map2.Visible = false;

            profileLast20Profile1Map1.Visible = false;
            profileLast20Profile1Map2.Visible = false;
            profileLast20Profile2Map1.Visible = false;
            profileLast20Profile2Map2.Visible = false;

            //Для профиля #1
            metroLabel85.Visible = false;
            metroLabel86.Visible = false;
            metroLabel101.Visible = false;
            metroLabel78.Visible = false;
            metroLabel90.Visible = false;
            metroLabel70.Visible = false;
            metroLabel2.Visible = false;

            //Для профиля #2
            metroLabel55.Visible = false;
            metroLabel57.Visible = false;
            metroLabel62.Visible = false;
            metroLabel49.Visible = false;
            metroLabel65.Visible = false;
            metroLabel42.Visible = false;
            metroLabel33.Visible = false;
        }

        #endregion

        public string Get(string player_Id)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if(players[i].detail.player_Id == player_Id)
                {
                    return players[i].detail.faceit_Url.ToString();
                }
            }
            return "";
        }

        #region Main
        // Проверка все ли профили загружены
        public void CheckOnLoadedLogins()
        {
            if(isLogin1Loaded & isLogin2Loaded)
            {
                CompareProfiles1.Visible = true;
            }
            else
            {
                CompareProfiles1.Visible = false;
            }
        }

        public void GetInfoMatch1_Click(object sender, EventArgs e)
        {
            if (!isMatchLoading)
            {
                metroPanel6.Visible = true;
                matchIDbox1.Enabled = false;
                isMatchLoading = true;
                InitializeBackgroundWorker();
                metroLabel1.Text = "";
                metroPanel3.Visible = false; // изменить после тестов
                thread1.RunWorkerAsync(); //Start Process
                //ShowInfoMatch();
                GetInfoMatch1.Enabled = false;
            }
            else
            {
                thread1.CancelAsync();
                thread2.CancelAsync();
                GetInfoMatch1.Enabled = true;
            }
        }
        private void getProfile1_Click_1(object sender, EventArgs e)
        {
            if (login1.Text == null) return;
            loadingLogin = loadedLogins.login1;
            
            //thread3.RunWorkerAsync();
        }
        private void Quit1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void goToFaceitProfile2_Click(object sender, EventArgs e)
        {
            Process.Start(profile2.detail.faceit_Url.ToString());
        }
        private void goToSteamProfile2_Click(object sender, EventArgs e)
        {
            Process.Start("https://steamcommunity.com/profiles/" + profile2.detail.steam_Id_64.ToString());
        }

        // Открытие профиля в браузере
        private void mNickname1_Click(object sender, EventArgs e)
        {
            Process.Start(Get(matchDetails.faction1.roster[0].player_Id));
        }
        private void mNickname2_Click(object sender, EventArgs e)
        {
            Process.Start(Get(matchDetails.faction1.roster[1].player_Id));
        }
        private void mNickname3_Click(object sender, EventArgs e)
        {
            Process.Start(Get(matchDetails.faction1.roster[2].player_Id));
        }
        private void mNickname4_Click(object sender, EventArgs e)
        {
            Process.Start(Get(matchDetails.faction1.roster[3].player_Id));
        }
        private void mNickname5_Click(object sender, EventArgs e)
        {
            Process.Start(Get(matchDetails.faction1.roster[4].player_Id));
        }
        private void mNickname6_Click(object sender, EventArgs e)
        {
            Process.Start(Get(matchDetails.faction2.roster[0].player_Id));
        }
        private void mNickname7_Click(object sender, EventArgs e)
        {
            Process.Start(Get(matchDetails.faction2.roster[1].player_Id));
        }
        private void mNickname8_Click(object sender, EventArgs e)
        {
            Process.Start(Get(matchDetails.faction2.roster[2].player_Id));
        }
        private void mNickname9_Click(object sender, EventArgs e)
        {
            Process.Start(Get(matchDetails.faction2.roster[3].player_Id));
        }
        private void mNickname10_Click(object sender, EventArgs e)
        {
            Process.Start(Get(matchDetails.faction2.roster[4].player_Id));
        }

        #endregion

        private void CompareProfiles1_Click(object sender, EventArgs e)
        {
            SelectBestMapProfile1();
            SelectBestMapProfile2();
            int index1 = map1BoxProfile1.SelectedIndex;
            int index2 = map1BoxProfile2.SelectedIndex;
            CSGOPlayerStatisticsProfileSegment map1 = profile.statistics.segments[index1];
            CSGOPlayerStatisticsProfileSegment map2 = profile2.statistics.segments[index2];

            int indexMap1Profile1 = 0;
            int indexMap1Profile2 = 0;

            int indexMap2Profile1 = 0;
            int indexMap2Profile2 = 0;

            int indexProfile1 = 0;
            int indexProfile2 = 0;

            int indexAllProfile1 = 0;
            int indexAllProfile2 = 0;

            int indexLast20Profile1 = 0;
            int indexLast20Profile2 = 0;


            #region Map1
            if (map1.wins > map2.wins)
            {
                winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.wins < map2.wins)
            {
                winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                winsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                winsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.matches > map2.matches)
            {
                matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.matches < map2.matches)
            {
                matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                matchesProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                matchesProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.win_Rate_Percantage > map2.win_Rate_Percantage)
            {
                winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.win_Rate_Percantage < map2.win_Rate_Percantage)
            {
                winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                winRateProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                winRateProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kd_Ratio > map2.kd_Ratio)
            {
                kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.kd_Ratio < map2.kd_Ratio)
            {
                kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                kdRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                kdRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Headshot_Percentage > map2.average_Headshot_Percentage)
            {
                avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.average_Headshot_Percentage < map2.average_Headshot_Percentage)
            {
                avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgHSsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgHSsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mode == map2.mode)
            {
                modeProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                modeProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                modeProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                modeProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
            }

            if (map1.rounds > map2.rounds)
            {
                roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.rounds < map2.rounds)
            {
                roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                roundsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                roundsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kills > map2.kills)
            {
                killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.kills < map2.kills)
            {
                killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                killsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                killsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.deaths < map2.deaths)
            {
                deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.deaths > map2.deaths)
            {
                deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                deathProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                deathProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kr_Ratio > map2.kr_Ratio)
            {
                krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.kr_Ratio < map2.kr_Ratio)
            {
                krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                krRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                krRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mvps > map2.mvps)
            {
                mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.mvps < map2.mvps)
            {
                mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                mvpsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                mvpsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.assists > map2.assists)
            {
                assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.assists < map2.assists)
            {
                assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                assistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                assistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.penta_Kills > map2.penta_Kills)
            {
                pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                pentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.penta_Kills < map2.penta_Kills)
            {
                pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                pentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                pentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                pentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.quadro_Kills > map2.quadro_Kills)
            {
                quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.quadro_Kills < map2.quadro_Kills)
            {
                quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                quadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                quadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.triple_Kills > map2.triple_Kills)
            {
                tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.triple_Kills < map2.triple_Kills)
            {
                tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                tripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                tripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.headshots_Per_match > map2.headshots_Per_match)
            {
                hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.headshots_Per_match < map2.headshots_Per_match)
            {
                hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                hsPerMatchProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                hsPerMatchProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Kills > map2.average_Kills)
            {
                avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.average_Kills < map2.average_Kills)
            {
                avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Deaths < map2.average_Deaths)
            {
                avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.average_Deaths > map2.average_Deaths)
            {
                avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgDeathProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgDeathProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.total_Headshot_Percentage > map2.total_Headshot_Percentage)
            {
                totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.total_Headshot_Percentage < map2.total_Headshot_Percentage)
            {
                totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                totalHSProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                totalHSProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Assists > map2.average_Assists)
            {
                avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.average_Assists < map2.average_Assists)
            {
                avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgAssistsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgAssistsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_MVPs > map2.average_MVPs)
            {
                avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.average_MVPs < map2.average_MVPs)
            {
                avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgMVPsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgMVPsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Penta_Kills > map2.average_Penta_Kills)
            {
                avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.average_Penta_Kills < map2.average_Penta_Kills)
            {
                avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgPentaKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgPentaKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Quadro_Kills > map2.average_Quadro_Kills)
            {
                avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.average_Quadro_Kills < map2.average_Quadro_Kills)
            {
                avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgQuadroKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgQuadroKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Triple_Kills > map2.average_Triple_Kills)
            {
                avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.average_Triple_Kills < map2.average_Triple_Kills)
            {
                avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgTripleKillsProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgTripleKillsProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_KR_Ratio > map2.average_KR_Ratio)
            {
                avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.average_KR_Ratio < map2.average_KR_Ratio)
            {
                avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgKRRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgKRRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_KD_Ratio > map2.average_KR_Ratio)
            {
                avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                indexMap1Profile1 += 1;
            }
            else if (map1.average_KR_Ratio < map2.average_KR_Ratio)
            {
                avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                indexMap1Profile2 += 1;
            }
            else
            {
                avgKDRatioProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                avgKDRatioProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
            }
            #endregion

            map1 = new CSGOPlayerStatisticsProfileSegment();
            map2 = new CSGOPlayerStatisticsProfileSegment();
            index1 = map2BoxProfile1.SelectedIndex;
            index2 = map2BoxProfile2.SelectedIndex;
            map1 = profile.statistics.segments[index1];
            map2 = profile2.statistics.segments[index2];

            #region Map2
            if (map1.wins > map2.wins)
            {
                winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.wins < map2.wins)
            {
                winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                winsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                winsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.matches > map2.matches)
            {
                matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.matches < map2.matches)
            {
                matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                matchesProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                matchesProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.win_Rate_Percantage > map2.win_Rate_Percantage)
            {
                winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.win_Rate_Percantage < map2.win_Rate_Percantage)
            {
                winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                winRateProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                winRateProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kd_Ratio > map2.kd_Ratio)
            {
                kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.kd_Ratio < map2.kd_Ratio)
            {
                kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                kdRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                kdRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Headshot_Percentage > map2.average_Headshot_Percentage)
            {
                avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.average_Headshot_Percentage < map2.average_Headshot_Percentage)
            {
                avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgHSsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgHSsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mode == map2.mode)
            {
                modeProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                modeProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                modeProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                modeProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
            }

            if (map1.rounds > map2.rounds)
            {
                roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.rounds < map2.rounds)
            {
                roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                roundsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                roundsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kills > map2.kills)
            {
                killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.kills < map2.kills)
            {
                killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                killsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                killsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.deaths < map2.deaths)
            {
                deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.deaths > map2.deaths)
            {
                deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                deathProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                deathProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.kr_Ratio > map2.kr_Ratio)
            {
                krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.kr_Ratio < map2.kr_Ratio)
            {
                krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                krRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                krRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.mvps > map2.mvps)
            {
                mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.mvps < map2.mvps)
            {
                mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                mvpsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                mvpsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.assists > map2.assists)
            {
                assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.assists < map2.assists)
            {
                assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                assistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                assistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.penta_Kills > map2.penta_Kills)
            {
                pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.penta_Kills < map2.penta_Kills)
            {
                pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                pentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                pentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.quadro_Kills > map2.quadro_Kills)
            {
                quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.quadro_Kills < map2.quadro_Kills)
            {
                quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                quadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                quadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.triple_Kills > map2.triple_Kills)
            {
                tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.triple_Kills < map2.triple_Kills)
            {
                tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                tripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                tripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.headshots_Per_match > map2.headshots_Per_match)
            {
                hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.headshots_Per_match < map2.headshots_Per_match)
            {
                hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                hsPerMatchProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                hsPerMatchProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Kills > map2.average_Kills)
            {
                avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.average_Kills < map2.average_Kills)
            {
                avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Deaths < map2.average_Deaths)
            {
                avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.average_Deaths > map2.average_Deaths)
            {
                avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgDeathProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgDeathProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.total_Headshot_Percentage > map2.total_Headshot_Percentage)
            {
                totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.total_Headshot_Percentage < map2.total_Headshot_Percentage)
            {
                totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                totalHSProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                totalHSProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Assists > map2.average_Assists)
            {
                avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.average_Assists < map2.average_Assists)
            {
                avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgAssistsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgAssistsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_MVPs > map2.average_MVPs)
            {
                avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.average_MVPs < map2.average_MVPs)
            {
                avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgMVPsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgMVPsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Penta_Kills > map2.average_Penta_Kills)
            {
                avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.average_Penta_Kills < map2.average_Penta_Kills)
            {
                avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgPentaKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgPentaKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Quadro_Kills > map2.average_Quadro_Kills)
            {
                avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.average_Quadro_Kills < map2.average_Quadro_Kills)
            {
                avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgQuadroKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgQuadroKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_Triple_Kills > map2.average_Triple_Kills)
            {
                avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.average_Triple_Kills < map2.average_Triple_Kills)
            {
                avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgTripleKillsProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgTripleKillsProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_KR_Ratio > map2.average_KR_Ratio)
            {
                avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.average_KR_Ratio < map2.average_KR_Ratio)
            {
                avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgKRRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKRRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (map1.average_KR_Ratio > map2.average_KR_Ratio)
            {
                avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
                avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                indexMap2Profile1 += 1;
            }
            else if (map1.average_KR_Ratio < map2.average_KR_Ratio)
            {
                avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
                avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                indexMap2Profile2 += 1;
            }
            else
            {
                avgKDRatioProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
                avgKDRatioProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }
            #endregion

            #region Profile
            if (profile.detail.faceit_Elo < profile2.detail.faceit_Elo)
            {
                eloProfile1.Style = MetroFramework.MetroColorStyle.Red;
                eloProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.detail.faceit_Elo > profile2.detail.faceit_Elo)
            {
                eloProfile1.Style = MetroFramework.MetroColorStyle.Green;
                eloProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                eloProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                eloProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.detail.skill_Level < profile2.detail.skill_Level)
            {
                levelProfile1.Style = MetroFramework.MetroColorStyle.Red;
                levelProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.detail.skill_Level > profile2.detail.skill_Level)
            {
                levelProfile1.Style = MetroFramework.MetroColorStyle.Green;
                levelProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                levelProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                levelProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.detail.membership_Type == "premium")
            {
                membershipProfile1.Style = MetroFramework.MetroColorStyle.Blue;
            }
            else if (profile.detail.membership_Type == "supporter")
            {
                membershipProfile1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                membershipProfile1.Style = MetroFramework.MetroColorStyle.Red;
            }

            if (profile2.detail.membership_Type == "premium")
            {
                membershipProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }
            else if (profile2.detail.membership_Type == "supporter")
            {
                membershipProfile2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                membershipProfile2.Style = MetroFramework.MetroColorStyle.Red;
            }

            if (profile.statistics.average_Headshots_Percentage < profile2.statistics.average_Headshots_Percentage)
            {
                avgHSProfile1.Style = MetroFramework.MetroColorStyle.Red;
                avgHSProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.statistics.average_Headshots_Percentage > profile2.statistics.average_Headshots_Percentage)
            {
                avgHSProfile1.Style = MetroFramework.MetroColorStyle.Green;
                avgHSProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                avgHSProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                avgHSProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.statistics.average_KD_Ratio < profile2.statistics.average_KD_Ratio)
            {
                avgKDProfile1.Style = MetroFramework.MetroColorStyle.Red;
                avgKDProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.statistics.average_KD_Ratio > profile2.statistics.average_KD_Ratio)
            {
                avgKDProfile1.Style = MetroFramework.MetroColorStyle.Green;
                avgKDProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                avgKDProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                avgKDProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.statistics.current_Win_Streak < profile2.statistics.current_Win_Streak)
            {
                curWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Red;
                curWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.statistics.current_Win_Streak > profile2.statistics.current_Win_Streak)
            {
                curWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Green;
                curWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                curWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                curWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.statistics.longest_Win_Streak < profile2.statistics.longest_Win_Streak)
            {
                longestWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Red;
                longestWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.statistics.longest_Win_Streak > profile2.statistics.longest_Win_Streak)
            {
                longestWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Green;
                longestWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                longestWinStrikeProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                longestWinStrikeProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.statistics.matches < profile2.statistics.matches)
            {
                matchesProfile1.Style = MetroFramework.MetroColorStyle.Red;
                matchesProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.statistics.matches > profile2.statistics.matches)
            {
                matchesProfile1.Style = MetroFramework.MetroColorStyle.Green;
                matchesProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                matchesProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                matchesProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.statistics.wins < profile2.statistics.wins)
            {
                winMatchesProfile1.Style = MetroFramework.MetroColorStyle.Red;
                winMatchesProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.statistics.wins > profile2.statistics.wins)
            {
                winMatchesProfile1.Style = MetroFramework.MetroColorStyle.Green;
                winMatchesProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                winMatchesProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                winMatchesProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.statistics.win_Rate < profile2.statistics.win_Rate)
            {
                winRateProfile1.Style = MetroFramework.MetroColorStyle.Red;
                winRateProfile2.Style = MetroFramework.MetroColorStyle.Green;
                indexProfile2 += 1;
            }
            else if (profile.statistics.win_Rate > profile2.statistics.win_Rate)
            {
                winRateProfile1.Style = MetroFramework.MetroColorStyle.Green;
                winRateProfile2.Style = MetroFramework.MetroColorStyle.Red;
                indexProfile1 += 1;
            }
            else
            {
                winRateProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                winRateProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (profile.detail.country == profile2.detail.country)
            {
                countryProfile1.Style = MetroFramework.MetroColorStyle.Blue;
                countryProfile2.Style = MetroFramework.MetroColorStyle.Blue;
            }
            else
            {
                countryProfile1.Style = MetroFramework.MetroColorStyle.Green;
                countryProfile2.Style = MetroFramework.MetroColorStyle.Green;
            }
            #endregion

            #region Last20

            CSGOPlayerStatisticsProfileSegment last20profile1 = last20Profile1;
            CSGOPlayerStatisticsProfileSegment last20profile2 = last20Profile2;

            ExtensionPlayerStatistics play1 = new ExtensionPlayerStatistics();
            play1.SetStats(last20profile1);
            ExtensionPlayerStatistics play2 = new ExtensionPlayerStatistics();
            play2.SetStats(last20profile2);

            if (last20profile1.kills > last20profile2.kills)
            {
                profile1Last20Kills.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Kills.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if(last20profile1.kills < last20profile2.kills)
            {
                profile1Last20Kills.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Kills.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Kills.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Kills.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.deaths < last20profile2.deaths)
            {
                profile1Last20Death.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Death.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.deaths > last20profile2.deaths)
            {
                profile1Last20Death.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Death.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Death.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Death.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.assists > last20profile2.assists)
            {
                profile1Last20Assists.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Assists.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.assists < last20profile2.assists)
            {
                profile1Last20Assists.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Assists.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Assists.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Assists.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.mvps > last20profile2.mvps)
            {
                profile1Last20MVPs.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20MVPs.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.mvps < last20profile2.mvps)
            {
                profile1Last20MVPs.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20MVPs.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20MVPs.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20MVPs.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.total_Headshot_Percentage > last20profile2.total_Headshot_Percentage)
            {
                profileLast20HS.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20HS.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.total_Headshot_Percentage < last20profile2.total_Headshot_Percentage)
            {
                profileLast20HS.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20HS.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profileLast20HS.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20HS.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.kd_Ratio > last20profile2.kd_Ratio)
            {
                profile1Last20KDr.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20KDr.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.kd_Ratio < last20profile2.kd_Ratio)
            {
                profile1Last20KDr.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20KDr.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20KDr.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20KDr.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.kr_Ratio > last20profile2.kr_Ratio)
            {
                profile1Last20KRr.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20KRr.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.kr_Ratio < last20profile2.kr_Ratio)
            {
                profile1Last20KRr.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20KRr.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20KRr.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20KRr.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.triple_Kills > last20profile2.triple_Kills)
            {
                profile1Last20Triple.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Triple.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.triple_Kills < last20profile2.triple_Kills)
            {
                profile1Last20Triple.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Triple.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Triple.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Triple.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.quadro_Kills > last20profile2.quadro_Kills)
            {
                profile1Last20Quadro.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Quadro.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.quadro_Kills < last20profile2.quadro_Kills)
            {
                profile1Last20Quadro.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Quadro.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Quadro.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Quadro.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.penta_Kills > last20profile2.penta_Kills)
            {
                profile1Last20Penta.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Penta.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.penta_Kills < last20profile2.penta_Kills)
            {
                profile1Last20Penta.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Penta.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Penta.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Penta.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.headshots_Per_match > last20profile2.headshots_Per_match)
            {
                profileLast20HSperMatch.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20HSperMatch.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.headshots_Per_match < last20profile2.headshots_Per_match)
            {
                profileLast20HSperMatch.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20HSperMatch.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profileLast20HSperMatch.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20HSperMatch.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.matches > last20profile2.matches)
            {
                profile1Last20Matches.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Matches.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.matches < last20profile2.matches)
            {
                profile1Last20Matches.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Matches.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Matches.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Matches.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.wins > last20profile2.wins)
            {
                profile1Last20Wins.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Wins.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.wins < last20profile2.wins)
            {
                profile1Last20Wins.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Wins.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Wins.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Wins.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.win_Rate_Percantage > last20profile2.win_Rate_Percantage)
            {
                profile1Last20WinRate.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20WinRate.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.win_Rate_Percantage < last20profile2.win_Rate_Percantage)
            {
                profile1Last20WinRate.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20WinRate.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20WinRate.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20WinRate.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.rounds > last20profile2.rounds)
            {
                profile1Last20Rounds.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Rounds.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.rounds < last20profile2.rounds)
            {
                profile1Last20Rounds.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Rounds.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Rounds.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Rounds.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.mode == last20profile2.mode)
            {
                profileLast20mode.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20mode.Style = MetroFramework.MetroColorStyle.Blue;
            }
            else
            {
                profileLast20mode.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20mode.Style = MetroFramework.MetroColorStyle.Green;
            }

            if (last20profile1.average_Kills > last20profile2.average_Kills)
            {
                profile1Last20avgKills.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgKills.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Kills < last20profile2.average_Kills)
            {
                profile1Last20avgKills.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgKills.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgKills.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgKills.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Deaths < last20profile2.average_Deaths)
            {
                profile1Last20avgDeath.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgDeath.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Deaths > last20profile2.average_Deaths)
            {
                profile1Last20avgDeath.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgDeath.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgDeath.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgDeath.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Assists > last20profile2.average_Assists)
            {
                profile1Last20avgAssists.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgAssists.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Assists < last20profile2.average_Assists)
            {
                profile1Last20avgAssists.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgAssists.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgAssists.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgAssists.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Headshot_Percentage > last20profile2.average_Headshot_Percentage)
            {
                profile1Last20avgHSs.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgHSs.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Headshot_Percentage < last20profile2.average_Headshot_Percentage)
            {
                profile1Last20avgHSs.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgHSs.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgHSs.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgHSs.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_KD_Ratio > last20profile2.average_KD_Ratio)
            {
                profile1Last20avgKDR.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgKDR.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_KD_Ratio < last20profile2.average_KD_Ratio)
            {
                profile1Last20avgKDR.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgKDR.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgKDR.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgKDR.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_KR_Ratio > last20profile2.average_KR_Ratio)
            {
                profile1Last20avgKRR.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgKRR.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_KR_Ratio < last20profile2.average_KR_Ratio)
            {
                profile1Last20avgKRR.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgKRR.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgKRR.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgKRR.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_MVPs > last20profile2.average_MVPs)
            {
                profile1Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_MVPs < last20profile2.average_MVPs)
            {
                profile1Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Triple_Kills > last20profile2.average_Triple_Kills)
            {
                profile1Last20avgTriple.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgTriple.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Triple_Kills < last20profile2.average_Triple_Kills)
            {
                profile1Last20avgTriple.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgTriple.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgTriple.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgTriple.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Quadro_Kills > last20profile2.average_Quadro_Kills)
            {
                profile1Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Quadro_Kills < last20profile2.average_Quadro_Kills)
            {
                profile1Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Penta_Kills > last20profile2.average_Penta_Kills)
            {
                profile1Last20avgPenta.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgPenta.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Penta_Kills < last20profile2.average_Penta_Kills)
            {
                profile1Last20avgPenta.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgPenta.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgPenta.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgPenta.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (play1.average_Rounds > play2.average_Rounds)
            {
                profile1Last20avgRounds.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgRounds.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (play1.average_Rounds < play2.average_Rounds)
            {
                profile1Last20avgRounds.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgRounds.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgRounds.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgRounds.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (play1.average_Headshot > play2.average_Headshot)
            {
                profile1Last20avgHSp.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgHSp.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (play1.average_Headshot < play2.average_Headshot)
            {
                profile1Last20avgHSp.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgHSp.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgHSp.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgHSp.Style = MetroFramework.MetroColorStyle.Blue;
            }

            
            #endregion

            //Для первой карты двух профилей
            if (indexMap1Profile1 > indexMap1Profile2)
            {
                profileMap1Profile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileMap1Profile1Map2.Text = "Worst"; //Отображение в Profile #1 графе Map2
                profileMap1Profile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileMap1Profile2Map2.Text = "Worst"; //Отображение в Profile #2 графе Map2
                profileMap1Profile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileMap1Profile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                profileMap1Profile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileMap1Profile2Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if(indexMap1Profile1 < indexMap1Profile2)
            {
                profileMap1Profile1Map1.Text = "Worst"; //Отображение в Profile #1 графе Map1
                profileMap1Profile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileMap1Profile2Map1.Text = "Worst"; //Отображение в Profile #2 графе Map1
                profileMap1Profile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileMap1Profile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileMap1Profile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                profileMap1Profile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileMap1Profile2Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                profileMap1Profile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileMap1Profile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileMap1Profile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileMap1Profile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileMap1Profile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileMap1Profile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                profileMap1Profile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileMap1Profile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            //Для второй карты двух профилей
            if (indexMap2Profile1 > indexMap2Profile2)
            {
                profileMap2Profile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileMap2Profile1Map2.Text = "Worst"; //Отображение в Profile #1 графе Map2
                profileMap2Profile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileMap2Profile2Map2.Text = "Worst"; //Отображение в Profile #2 графе Map2
                profileMap2Profile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileMap2Profile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                profileMap2Profile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileMap2Profile2Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (indexMap2Profile1 < indexMap2Profile2)
            {
                profileMap2Profile1Map1.Text = "Worst"; //Отображение в Profile #1 графе Map1
                profileMap2Profile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileMap2Profile2Map1.Text = "Worst"; //Отображение в Profile #2 графе Map1
                profileMap2Profile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileMap2Profile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileMap2Profile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                profileMap2Profile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileMap2Profile2Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                profileMap2Profile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileMap2Profile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileMap2Profile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileMap2Profile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileMap2Profile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileMap2Profile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                profileMap2Profile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileMap2Profile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            //Для 20-и последних матчей двух профилей
            if (indexLast20Profile1 > indexLast20Profile2)
            {
                profileLast20Profile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileLast20Profile1Map2.Text = "Worst"; //Отображение в Profile #1 графе Map2
                profileLast20Profile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileLast20Profile2Map2.Text = "Worst"; //Отображение в Profile #2 графе Map2
                profileLast20Profile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileLast20Profile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                profileLast20Profile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileLast20Profile2Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (indexLast20Profile1 < indexLast20Profile2)
            {
                profileLast20Profile1Map1.Text = "Worst"; //Отображение в Profile #1 графе Map1
                profileLast20Profile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileLast20Profile2Map1.Text = "Worst"; //Отображение в Profile #2 графе Map1
                profileLast20Profile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileLast20Profile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileLast20Profile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                profileLast20Profile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileLast20Profile2Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                profileLast20Profile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileLast20Profile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileLast20Profile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileLast20Profile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileLast20Profile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileLast20Profile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                profileLast20Profile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileLast20Profile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }


            indexAllProfile1 = indexMap1Profile1 + indexMap2Profile1 + indexAllProfile1 + indexLast20Profile1;
            indexAllProfile2 = indexMap1Profile2 + indexMap2Profile2 + indexAllProfile2 + indexLast20Profile2;

            //Для всего профиля двух профилей
            if (indexAllProfile1 > indexAllProfile2)
            {
                profileAllProfile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileAllProfile1Map2.Text = "Worst"; //Отображение в Profile #1 графе Map2
                profileAllProfile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileAllProfile2Map2.Text = "Worst"; //Отображение в Profile #2 графе Map2
                profileAllProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileAllProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                profileAllProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileAllProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (indexAllProfile1 < indexAllProfile2)
            {
                profileAllProfile1Map1.Text = "Worst"; //Отображение в Profile #1 графе Map1
                profileAllProfile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileAllProfile2Map1.Text = "Worst"; //Отображение в Profile #2 графе Map1
                profileAllProfile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileAllProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileAllProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                profileAllProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileAllProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                profileAllProfile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileAllProfile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileAllProfile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileAllProfile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileAllProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileAllProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                profileAllProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileAllProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            //Для профиля двух профилей
            if (indexProfile1 > indexProfile2)
            {
                profileProfile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileProfile1Map2.Text = "Worst"; //Отображение в Profile #1 графе Map2
                profileProfile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileProfile2Map2.Text = "Worst"; //Отображение в Profile #2 графе Map2
                profileProfile1Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileProfile1Map2.Style = MetroFramework.MetroColorStyle.Red;
                profileProfile2Map1.Style = MetroFramework.MetroColorStyle.Green;
                profileProfile2Map2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (indexProfile1 < indexProfile2)
            {
                profileProfile1Map1.Text = "Worst"; //Отображение в Profile #1 графе Map1
                profileProfile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileProfile2Map1.Text = "Worst"; //Отображение в Profile #2 графе Map1
                profileProfile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileProfile1Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileProfile1Map2.Style = MetroFramework.MetroColorStyle.Green;
                profileProfile2Map1.Style = MetroFramework.MetroColorStyle.Red;
                profileProfile2Map2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                profileProfile1Map1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profileProfile1Map2.Text = "Best"; //Отображение в Profile #1 графе Map2
                profileProfile2Map1.Text = "Best"; //Отображение в Profile #2 графе Map1
                profileProfile2Map2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profileProfile1Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileProfile1Map2.Style = MetroFramework.MetroColorStyle.Blue;
                profileProfile2Map1.Style = MetroFramework.MetroColorStyle.Blue;
                profileProfile2Map2.Style = MetroFramework.MetroColorStyle.Blue;
            }

            EnableStatsCompareProfile();
        }
        
        private void matchIDbox1_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = GetInfoMatch1;
        }

        private void login1_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = getProfile1;
        }

        private void login2_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = getProfile2;
        }

        private void settings1_Click(object sender, EventArgs e)
        {
            if (metroPanel4.Visible)
            {
                metroPanel4.Visible = false;
            }
            else
            {
                metroPanel4.Visible = true;
            }
            
        }

        private void metroCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (isLogin1Loaded)
            {
                map1BoxProfile1.Items.Clear();
                LoadMapInComboBox(map1BoxProfile1, profile.statistics.segments);
                map2BoxProfile1.Items.Clear();
                LoadMapInComboBox(map2BoxProfile1, profile.statistics.segments);
                loginMapBox1Profile1_defaultStyle();
                loginMapBox2Profile1_defaultStyle();
            }
            if (isLogin2Loaded)
            {
                map1BoxProfile2.Items.Clear();
                LoadMapInComboBox(map1BoxProfile2, profile2.statistics.segments);
                map2BoxProfile2.Items.Clear();
                LoadMapInComboBox(map2BoxProfile2, profile2.statistics.segments);
                loginMapBox1Profile2_defaultStyle();
                loginMapBox2Profile2_defaultStyle();
            }
        }

        private void metroCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (isLogin1Loaded)
            {
                map1BoxProfile1.Items.Clear();
                LoadMapInComboBox(map1BoxProfile1, profile.statistics.segments);
                map2BoxProfile1.Items.Clear();
                LoadMapInComboBox(map2BoxProfile1, profile.statistics.segments);
                loginMapBox1Profile1_defaultStyle();
                loginMapBox2Profile1_defaultStyle();
            }
            if (isLogin2Loaded)
            {
                map1BoxProfile2.Items.Clear();
                LoadMapInComboBox(map1BoxProfile2, profile2.statistics.segments);
                map2BoxProfile2.Items.Clear();
                LoadMapInComboBox(map2BoxProfile2, profile2.statistics.segments);
                loginMapBox1Profile2_defaultStyle();
                loginMapBox2Profile2_defaultStyle();
            }
        }
        
        private void metroButton1_Click_1(object sender, EventArgs e)
        {
            DisableStatsCompareProfile();
            loginProfileProfile1_defaultStyle();
            loginMapBox1Profile1_defaultStyle();
            loginMapBox2Profile1_defaultStyle();
            CompareMap1andMap2Profile1_Style();
            SelectBestMapProfile1();
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            DisableStatsCompareProfile();
            loginProfileProfile2_defaultStyle();
            loginMapBox1Profile2_defaultStyle();
            loginMapBox2Profile2_defaultStyle();
            CompareMap1andMap2Profile2_Style();
            SelectBestMapProfile2();
        }

        private void metroButton7_Click(object sender, EventArgs e)
        {
            CompareLast20Profile1andProfile2();
            Last20EnableStyleProfile1();
        }

        public void CompareLast20Profile1andProfile2()
        {
            int indexLast20Profile1 = 0;
            int indexLast20Profile2 = 0;
            CSGOPlayerStatisticsProfileSegment last20profile1 = last20Profile1;
            CSGOPlayerStatisticsProfileSegment last20profile2 = last20Profile2;

            ExtensionPlayerStatistics play2 = new ExtensionPlayerStatistics();
            play2.SetStats(last20profile2);
            ExtensionPlayerStatistics play1 = new ExtensionPlayerStatistics();
            play1.SetStats(last20profile2);

            if (last20profile1.kills > last20profile2.kills)
            {
                profile1Last20Kills.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Kills.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.kills < last20profile2.kills)
            {
                profile1Last20Kills.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Kills.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Kills.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Kills.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.deaths < last20profile2.deaths)
            {
                profile1Last20Death.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Death.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.deaths > last20profile2.deaths)
            {
                profile1Last20Death.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Death.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Death.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Death.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.assists > last20profile2.assists)
            {
                profile1Last20Assists.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Assists.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.assists < last20profile2.assists)
            {
                profile1Last20Assists.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Assists.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Assists.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Assists.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.mvps > last20profile2.mvps)
            {
                profile1Last20MVPs.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20MVPs.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.mvps < last20profile2.mvps)
            {
                profile1Last20MVPs.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20MVPs.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20MVPs.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20MVPs.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.total_Headshot_Percentage > last20profile2.total_Headshot_Percentage)
            {
                profileLast20HS.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20HS.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.total_Headshot_Percentage < last20profile2.total_Headshot_Percentage)
            {
                profileLast20HS.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20HS.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profileLast20HS.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20HS.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.kd_Ratio > last20profile2.kd_Ratio)
            {
                profile1Last20KDr.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20KDr.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.kd_Ratio < last20profile2.kd_Ratio)
            {
                profile1Last20KDr.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20KDr.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20KDr.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20KDr.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.kr_Ratio > last20profile2.kr_Ratio)
            {
                profile1Last20KRr.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20KRr.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.kr_Ratio < last20profile2.kr_Ratio)
            {
                profile1Last20KRr.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20KRr.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20KRr.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20KRr.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.triple_Kills > last20profile2.triple_Kills)
            {
                profile1Last20Triple.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Triple.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.triple_Kills < last20profile2.triple_Kills)
            {
                profile1Last20Triple.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Triple.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Triple.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Triple.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.quadro_Kills > last20profile2.quadro_Kills)
            {
                profile1Last20Quadro.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Quadro.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.quadro_Kills < last20profile2.quadro_Kills)
            {
                profile1Last20Quadro.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Quadro.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Quadro.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Quadro.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.penta_Kills > last20profile2.penta_Kills)
            {
                profile1Last20Penta.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Penta.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.penta_Kills < last20profile2.penta_Kills)
            {
                profile1Last20Penta.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Penta.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Penta.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Penta.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.headshots_Per_match > last20profile2.headshots_Per_match)
            {
                profileLast20HSperMatch.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20HSperMatch.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.headshots_Per_match < last20profile2.headshots_Per_match)
            {
                profileLast20HSperMatch.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20HSperMatch.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profileLast20HSperMatch.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20HSperMatch.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.matches > last20profile2.matches)
            {
                profile1Last20Matches.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Matches.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.matches < last20profile2.matches)
            {
                profile1Last20Matches.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Matches.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Matches.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Matches.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.wins > last20profile2.wins)
            {
                profile1Last20Wins.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Wins.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.wins < last20profile2.wins)
            {
                profile1Last20Wins.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Wins.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Wins.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Wins.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.win_Rate_Percantage > last20profile2.win_Rate_Percantage)
            {
                profile1Last20WinRate.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20WinRate.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.win_Rate_Percantage < last20profile2.win_Rate_Percantage)
            {
                profile1Last20WinRate.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20WinRate.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20WinRate.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20WinRate.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.rounds > last20profile2.rounds)
            {
                profile1Last20Rounds.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Rounds.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.rounds < last20profile2.rounds)
            {
                profile1Last20Rounds.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Rounds.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Rounds.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Rounds.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.mode == last20profile2.mode)
            {
                profileLast20mode.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20mode.Style = MetroFramework.MetroColorStyle.Blue;
            }
            else
            {
                profileLast20mode.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20mode.Style = MetroFramework.MetroColorStyle.Green;
            }

            if (last20profile1.average_Kills > last20profile2.average_Kills)
            {
                profile1Last20avgKills.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgKills.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Kills < last20profile2.average_Kills)
            {
                profile1Last20avgKills.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgKills.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgKills.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgKills.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Deaths < last20profile2.average_Deaths)
            {
                profile1Last20avgDeath.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgDeath.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Deaths > last20profile2.average_Deaths)
            {
                profile1Last20avgDeath.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgDeath.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgDeath.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgDeath.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Assists > last20profile2.average_Assists)
            {
                profile1Last20avgAssists.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgAssists.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Assists < last20profile2.average_Assists)
            {
                profile1Last20avgAssists.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgAssists.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgAssists.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgAssists.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Headshot_Percentage > last20profile2.average_Headshot_Percentage)
            {
                profile1Last20avgHSs.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgHSs.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Headshot_Percentage < last20profile2.average_Headshot_Percentage)
            {
                profile1Last20avgHSs.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgHSs.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgHSs.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgHSs.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_KD_Ratio > last20profile2.average_KD_Ratio)
            {
                profile1Last20avgKDR.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgKDR.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_KD_Ratio < last20profile2.average_KD_Ratio)
            {
                profile1Last20avgKDR.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgKDR.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgKDR.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgKDR.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_KR_Ratio > last20profile2.average_KR_Ratio)
            {
                profile1Last20avgKRR.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgKRR.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_KR_Ratio < last20profile2.average_KR_Ratio)
            {
                profile1Last20avgKRR.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgKRR.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgKRR.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgKRR.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_MVPs > last20profile2.average_MVPs)
            {
                profile1Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_MVPs < last20profile2.average_MVPs)
            {
                profile1Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Triple_Kills > last20profile2.average_Triple_Kills)
            {
                profile1Last20avgTriple.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgTriple.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Triple_Kills < last20profile2.average_Triple_Kills)
            {
                profile1Last20avgTriple.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgTriple.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgTriple.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgTriple.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Quadro_Kills > last20profile2.average_Quadro_Kills)
            {
                profile1Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Quadro_Kills < last20profile2.average_Quadro_Kills)
            {
                profile1Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Penta_Kills > last20profile2.average_Penta_Kills)
            {
                profile1Last20avgPenta.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgPenta.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Penta_Kills < last20profile2.average_Penta_Kills)
            {
                profile1Last20avgPenta.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgPenta.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgPenta.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgPenta.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (play1.average_Rounds > play2.average_Rounds)
            {
                profile1Last20avgRounds.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgRounds.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (play1.average_Rounds < play2.average_Rounds)
            {
                profile1Last20avgRounds.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgRounds.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgRounds.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgRounds.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (play1.average_Headshot > play2.average_Headshot)
            {
                profile1Last20avgHSp.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgHSp.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (play1.average_Headshot < play2.average_Headshot)
            {
                profile1Last20avgHSp.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgHSp.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgHSp.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgHSp.Style = MetroFramework.MetroColorStyle.Blue;
            }

            //Для профиля #1
            metroLabel28.Visible = true;
            metroLabel26.Visible = true;
            metroLabel16.Visible = true;
            profile1last20Profile1.Visible = true;
            profile2last20Profile1.Visible = true;
            //Для профиля #2
            metroLabel12.Visible = true;
            metroLabel18.Visible = true;
            metroLabel20.Visible = true;
            profile1last20profile2.Visible = true;
            profile2last20Profile2.Visible = true;

            if (indexLast20Profile1 > indexLast20Profile2)
            {
                profile1last20Profile1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profile2last20Profile1.Text = "Worst"; //Отображение в Profile #1 графе Map2
                profile1last20profile2.Text = "Best"; //Отображение в Profile #2 графе Map1
                profile2last20Profile2.Text = "Worst"; //Отображение в Profile #2 графе Map2
                profile1last20Profile1.Style = MetroFramework.MetroColorStyle.Green;
                profile2last20Profile1.Style = MetroFramework.MetroColorStyle.Red;
                profile1last20profile2.Style = MetroFramework.MetroColorStyle.Green;
                profile2last20Profile2.Style = MetroFramework.MetroColorStyle.Red;
            }
            else if (indexLast20Profile1 < indexLast20Profile2)
            {
                profile1last20Profile1.Text = "Worst"; //Отображение в Profile #1 графе Map1
                profile2last20Profile1.Text = "Best"; //Отображение в Profile #1 графе Map2
                profile1last20profile2.Text = "Worst"; //Отображение в Profile #2 графе Map1
                profile2last20Profile2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profile1last20Profile1.Style = MetroFramework.MetroColorStyle.Red;
                profile2last20Profile1.Style = MetroFramework.MetroColorStyle.Green;
                profile1last20profile2.Style = MetroFramework.MetroColorStyle.Red;
                profile2last20Profile2.Style = MetroFramework.MetroColorStyle.Green;
            }
            else
            {
                profile1last20Profile1.Text = "Best"; //Отображение в Profile #1 графе Map1
                profile2last20Profile1.Text = "Best"; //Отображение в Profile #1 графе Map2
                profile1last20profile2.Text = "Best"; //Отображение в Profile #2 графе Map1
                profile2last20Profile2.Text = "Best"; //Отображение в Profile #2 графе Map2
                profile1last20Profile1.Style = MetroFramework.MetroColorStyle.Blue;
                profile2last20Profile1.Style = MetroFramework.MetroColorStyle.Blue;
                profile1last20profile2.Style = MetroFramework.MetroColorStyle.Blue;
                profile2last20Profile2.Style = MetroFramework.MetroColorStyle.Blue;
            }
        }
        public void Last20EnableStyleProfile1()
        {
            int indexLast20Profile1 = 0;
            int indexLast20Profile2 = 0;
            CSGOPlayerStatisticsProfileSegment last20profile1 = last20Profile1;
            CSGOPlayerStatisticsProfileSegment last20profile2 = last20Profile2;

            ExtensionPlayerStatistics play2 = new ExtensionPlayerStatistics();
            play2.SetStats(last20profile2);
            ExtensionPlayerStatistics play1 = new ExtensionPlayerStatistics();
            play1.SetStats(last20profile2);

            if (last20profile1.kills > last20profile2.kills)
            {
                profile1Last20Kills.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Kills.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.kills < last20profile2.kills)
            {
                profile1Last20Kills.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Kills.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Kills.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Kills.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.deaths < last20profile2.deaths)
            {
                profile1Last20Death.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Death.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.deaths > last20profile2.deaths)
            {
                profile1Last20Death.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Death.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Death.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Death.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.assists > last20profile2.assists)
            {
                profile1Last20Assists.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Assists.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.assists < last20profile2.assists)
            {
                profile1Last20Assists.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Assists.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Assists.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Assists.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.mvps > last20profile2.mvps)
            {
                profile1Last20MVPs.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20MVPs.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.mvps < last20profile2.mvps)
            {
                profile1Last20MVPs.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20MVPs.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20MVPs.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20MVPs.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.total_Headshot_Percentage > last20profile2.total_Headshot_Percentage)
            {
                profileLast20HS.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20HS.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.total_Headshot_Percentage < last20profile2.total_Headshot_Percentage)
            {
                profileLast20HS.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20HS.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profileLast20HS.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20HS.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.kd_Ratio > last20profile2.kd_Ratio)
            {
                profile1Last20KDr.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20KDr.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.kd_Ratio < last20profile2.kd_Ratio)
            {
                profile1Last20KDr.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20KDr.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20KDr.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20KDr.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.kr_Ratio > last20profile2.kr_Ratio)
            {
                profile1Last20KRr.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20KRr.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.kr_Ratio < last20profile2.kr_Ratio)
            {
                profile1Last20KRr.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20KRr.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20KRr.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20KRr.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.triple_Kills > last20profile2.triple_Kills)
            {
                profile1Last20Triple.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Triple.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.triple_Kills < last20profile2.triple_Kills)
            {
                profile1Last20Triple.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Triple.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Triple.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Triple.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.quadro_Kills > last20profile2.quadro_Kills)
            {
                profile1Last20Quadro.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Quadro.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.quadro_Kills < last20profile2.quadro_Kills)
            {
                profile1Last20Quadro.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Quadro.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Quadro.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Quadro.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.penta_Kills > last20profile2.penta_Kills)
            {
                profile1Last20Penta.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Penta.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.penta_Kills < last20profile2.penta_Kills)
            {
                profile1Last20Penta.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Penta.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Penta.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Penta.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.headshots_Per_match > last20profile2.headshots_Per_match)
            {
                profileLast20HSperMatch.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20HSperMatch.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.headshots_Per_match < last20profile2.headshots_Per_match)
            {
                profileLast20HSperMatch.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20HSperMatch.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profileLast20HSperMatch.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20HSperMatch.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.matches > last20profile2.matches)
            {
                profile1Last20Matches.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Matches.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.matches < last20profile2.matches)
            {
                profile1Last20Matches.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Matches.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Matches.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Matches.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.wins > last20profile2.wins)
            {
                profile1Last20Wins.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Wins.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.wins < last20profile2.wins)
            {
                profile1Last20Wins.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Wins.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Wins.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Wins.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.win_Rate_Percantage > last20profile2.win_Rate_Percantage)
            {
                profile1Last20WinRate.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20WinRate.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.win_Rate_Percantage < last20profile2.win_Rate_Percantage)
            {
                profile1Last20WinRate.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20WinRate.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20WinRate.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20WinRate.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.rounds > last20profile2.rounds)
            {
                profile1Last20Rounds.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20Rounds.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.rounds < last20profile2.rounds)
            {
                profile1Last20Rounds.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20Rounds.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20Rounds.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20Rounds.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.mode == last20profile2.mode)
            {
                profileLast20mode.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20mode.Style = MetroFramework.MetroColorStyle.Blue;
            }
            else
            {
                profileLast20mode.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20mode.Style = MetroFramework.MetroColorStyle.Green;
            }

            if (last20profile1.average_Kills > last20profile2.average_Kills)
            {
                profile1Last20avgKills.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgKills.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Kills < last20profile2.average_Kills)
            {
                profile1Last20avgKills.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgKills.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgKills.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgKills.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Deaths < last20profile2.average_Deaths)
            {
                profile1Last20avgDeath.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgDeath.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Deaths > last20profile2.average_Deaths)
            {
                profile1Last20avgDeath.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgDeath.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgDeath.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgDeath.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Assists > last20profile2.average_Assists)
            {
                profile1Last20avgAssists.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgAssists.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Assists < last20profile2.average_Assists)
            {
                profile1Last20avgAssists.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgAssists.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgAssists.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgAssists.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Headshot_Percentage > last20profile2.average_Headshot_Percentage)
            {
                profile1Last20avgHSs.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgHSs.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Headshot_Percentage < last20profile2.average_Headshot_Percentage)
            {
                profile1Last20avgHSs.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgHSs.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgHSs.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgHSs.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_KD_Ratio > last20profile2.average_KD_Ratio)
            {
                profile1Last20avgKDR.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgKDR.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_KD_Ratio < last20profile2.average_KD_Ratio)
            {
                profile1Last20avgKDR.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgKDR.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgKDR.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgKDR.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_KR_Ratio > last20profile2.average_KR_Ratio)
            {
                profile1Last20avgKRR.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgKRR.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_KR_Ratio < last20profile2.average_KR_Ratio)
            {
                profile1Last20avgKRR.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgKRR.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgKRR.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgKRR.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_MVPs > last20profile2.average_MVPs)
            {
                profile1Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_MVPs < last20profile2.average_MVPs)
            {
                profile1Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Triple_Kills > last20profile2.average_Triple_Kills)
            {
                profile1Last20avgTriple.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgTriple.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Triple_Kills < last20profile2.average_Triple_Kills)
            {
                profile1Last20avgTriple.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgTriple.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgTriple.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgTriple.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Quadro_Kills > last20profile2.average_Quadro_Kills)
            {
                profile1Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Quadro_Kills < last20profile2.average_Quadro_Kills)
            {
                profile1Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (last20profile1.average_Penta_Kills > last20profile2.average_Penta_Kills)
            {
                profile1Last20avgPenta.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgPenta.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (last20profile1.average_Penta_Kills < last20profile2.average_Penta_Kills)
            {
                profile1Last20avgPenta.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgPenta.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgPenta.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgPenta.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (play1.average_Rounds > play2.average_Rounds)
            {
                profile1Last20avgRounds.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgRounds.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (play1.average_Rounds < play2.average_Rounds)
            {
                profile1Last20avgRounds.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgRounds.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgRounds.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgRounds.Style = MetroFramework.MetroColorStyle.Blue;
            }

            if (play1.average_Headshot > play2.average_Headshot)
            {
                profile1Last20avgHSp.Style = MetroFramework.MetroColorStyle.Green;
                profile2Last20avgHSp.Style = MetroFramework.MetroColorStyle.Red;
                indexLast20Profile1 += 1;
            }
            else if (play1.average_Headshot < play2.average_Headshot)
            {
                profile1Last20avgHSp.Style = MetroFramework.MetroColorStyle.Red;
                profile2Last20avgHSp.Style = MetroFramework.MetroColorStyle.Green;
                indexLast20Profile2 += 1;
            }
            else
            {
                profile1Last20avgHSp.Style = MetroFramework.MetroColorStyle.Blue;
                profile2Last20avgHSp.Style = MetroFramework.MetroColorStyle.Blue;
            }
        }

        public void Last20StyleProfile1_DefaultStyle()
        {
            profile1Last20Kills.Style = MetroFramework.MetroColorStyle.Red;
            profile1Last20avgHSp.Style = MetroFramework.MetroColorStyle.Red;
            profile1Last20avgRounds.Style = MetroFramework.MetroColorStyle.Red;
            profile1Last20avgPenta.Style = MetroFramework.MetroColorStyle.Green;
            profile1Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20avgTriple.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20avgKRR.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20avgKDR.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20avgHSs.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20avgAssists.Style = MetroFramework.MetroColorStyle.Green;
            profile1Last20avgDeath.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20avgKills.Style = MetroFramework.MetroColorStyle.Blue;
            profileLast20mode.Style = MetroFramework.MetroColorStyle.Green;
            profile1Last20Rounds.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20WinRate.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20Wins.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20Matches.Style = MetroFramework.MetroColorStyle.Blue;
            profileLast20HSperMatch.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20Penta.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20Quadro.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20Triple.Style = MetroFramework.MetroColorStyle.Red;
            profile1Last20KRr.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20KDr.Style = MetroFramework.MetroColorStyle.Blue;
            profileLast20HS.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20MVPs.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20Assists.Style = MetroFramework.MetroColorStyle.Blue;
            profile1Last20Death.Style = MetroFramework.MetroColorStyle.Blue;
            //Для профиля #1
            metroLabel28.Visible = false;
            metroLabel26.Visible = false;
            metroLabel16.Visible = false;
            //Для профиля #2
            metroLabel12.Visible = false;
            metroLabel18.Visible = false;
            metroLabel20.Visible = false;
        }
        public void Last20StyleProfile2_DefaultStyle()
        {
            profile2Last20Kills.Style = MetroFramework.MetroColorStyle.Red;
            profile2Last20avgHSp.Style = MetroFramework.MetroColorStyle.Red;
            profile2Last20avgRounds.Style = MetroFramework.MetroColorStyle.Red;
            profile2Last20avgPenta.Style = MetroFramework.MetroColorStyle.Green;
            profile2Last20avgQuadro.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20avgTriple.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20avgMVPs.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20avgKRR.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20avgKDR.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20avgHSs.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20avgAssists.Style = MetroFramework.MetroColorStyle.Green;
            profile2Last20avgDeath.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20avgKills.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20mode.Style = MetroFramework.MetroColorStyle.Green;
            profile2Last20Rounds.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20WinRate.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20Wins.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20Matches.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20HSperMatch.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20Penta.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20Quadro.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20Triple.Style = MetroFramework.MetroColorStyle.Red;
            profile2Last20KRr.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20KDr.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20HS.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20MVPs.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20Assists.Style = MetroFramework.MetroColorStyle.Blue;
            profile2Last20Death.Style = MetroFramework.MetroColorStyle.Blue;
            //Для профиля #1
            metroLabel28.Visible = false;
            metroLabel26.Visible = false;
            metroLabel16.Visible = false;
            //Для профиля #2
            metroLabel12.Visible = false;
            metroLabel18.Visible = false;
            metroLabel20.Visible = false;
        }

        private void metroButton6_Click(object sender, EventArgs e)
        {
            CompareLast20Profile1andProfile2();
            Last20EnableStyleProfile1();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void InitializeBackgroundWorker()
        {
            ThreadMatchDetails_Click();
            ThreadMatchStatistics_Click();
            ThreadProfile1_Click();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void SaveEloToFile()
        {
            if (!File.Exists(filePatchElo))
            {
                File.Create(filePatchElo);
            }
            File.WriteAllText(filePatchElo, JsonConvert.SerializeObject(sysElo));
        }
        private void ReadEloFromFile()
        {
            if (File.Exists(filePatchElo))
            {
                JToken token = new JObject(File.ReadAllText(filePatchElo));
                foreach (JToken json in token)
                {
                    sysElo.Add(json.ToObject<SysELO>());
                }
            }
        }

        private void metroLabel41_Click(object sender, EventArgs e)
        {

        }

        private void ThreadMatchDetails_Click()
        {
            thread1 = new BackgroundWorker();
            thread1.WorkerReportsProgress = true;
            thread1.WorkerSupportsCancellation = true;
            thread1.DoWork += ThreadMatchDetails_Work;
            thread1.ProgressChanged += ThreadMatchDetails_ProgressChanged;
            thread1.RunWorkerCompleted += ThreadMatchDetails_RunWorkerCompleted;
        }
        private void ThreadMatchDetails_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            metroProgressBar1.Value = e.ProgressPercentage;
        }
        private void ThreadMatchDetails_Work(object sender, DoWorkEventArgs e)
        {
            MatchDetails matchDet = new MatchDetails();
            matchDet.faceit = faceit;
            if(matchDet.GetMatchDetailsCSGO(matchIDbox1.Text, out CSGOMatchDetails value))
            {
                matchDetails = value;
                isMatchLoaded = true;
                List<CSGOMatchDetailsTeamPlayer> plays = new List<CSGOMatchDetailsTeamPlayer>();
                foreach (CSGOMatchDetailsTeamPlayer play in matchDetails.faction1.roster)
                {
                    plays.Add(play);
                }
                foreach (CSGOMatchDetailsTeamPlayer play in matchDetails.faction2.roster)
                {
                    plays.Add(play);
                }

                for (int i = 0; i < plays.Count; i++)
                {
                    PlayerCSGO player = new PlayerCSGO();
                    player.GetInfo(plays[i].player_Id, faceit);
                    players.Add(player);
                    thread1.ReportProgress(i);
                    Thread.Sleep(50);

                }
                e.Result = 200;    // будет передано в RunWorkerComрleted
            }
            else
            {
                e.Result = 201;    // будет передано в RunWorkerComрleted
            }
            
        }
        private void ThreadMatchDetails_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                metroLabel1.Text = "Process was cancelled";
                isMatchLoading = false;
                matchIDbox1.Enabled = true;
            }
            else if (e.Error != null)
            {
                metroLabel1.Text = e.Error.Message;
                Form2 form = new Form2();
                form.errorMsg.Text = "Msg: " + e.Error.Message;
                form.Show();
                isMatchLoading = false;
                matchIDbox1.Enabled = true;
            }
            else
            {
                metroLabel1.Text = "Process was completed";
                if(matchDetails.status.ToUpper() == "FINISHED")
                {
                    thread2.RunWorkerAsync();
                }
                else
                {
                    LoadMembers();
                    ShowInfoMatch();
                    metroPanel3.Visible = true;
                    metroTabControl1.SelectTab(tabMatchint);
                    isMatchLoading = false;
                    GetInfoMatch1.Enabled = true;
                    matchIDbox1.Enabled = true;
                }
            }
        }

        private void ThreadMatchStatistics_Click()
        {
            thread2 = new BackgroundWorker();
            thread2.WorkerReportsProgress = true;
            thread2.WorkerSupportsCancellation = true;
            thread2.DoWork += ThreadMatchStatistics_Work;
            thread2.ProgressChanged += ThreadMatchStatistics_ProgressChanged;
            thread2.RunWorkerCompleted += ThreadMatchStatistics_RunWorkerCompleted;
        }
        private void ThreadMatchStatistics_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            metroProgressBar3.Value = e.ProgressPercentage;
        }
        private void ThreadMatchStatistics_Work(object sender, DoWorkEventArgs e)
        {
            metroProgressBar3.Value = 0;
            MatchStats matchStats = new MatchStats();
            thread2.ReportProgress(5);
            matchStats.faceit = faceit;
            thread2.ReportProgress(5);
            if (matchStats.GetMatchStatsCSGO(matchIDbox1.Text, out CSGOMatchStats value))
            {
                matchStatistics = value;
                thread2.ReportProgress(30);
                playerS = new List<CSGOMatchStatsTeamPlayer>();
                thread2.ReportProgress(10);
                foreach (CSGOMatchStatsTeamPlayer play in matchStatistics.team_A.players)
                {
                    playerS.Add(play);
                }
                thread2.ReportProgress(25);
                foreach (CSGOMatchStatsTeamPlayer play in matchStatistics.team_B.players)
                {
                    playerS.Add(play);
                }
                thread2.ReportProgress(25);
            }
            else
            {
                Form2 form = new Form2();
                form.errorMsg.Text = "Error: " + matchStats.GetLastError;
                form.Show();
            }
        }
        private void ThreadMatchStatistics_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                metroLabel1.Text = "Process was cancelled. Stats.";

            }
            else if (e.Error != null)
            {
                metroLabel1.Text = e.Error.Message;
                Form2 form = new Form2();
                form.errorMsg.Text = "Msg(S): " + e.Error.Message;
                form.Show();
            }
            else
            {
                metroLabel1.Text = "Process was completed. Stats.";
                LoadMembers();
                ShowInfoMatch();
                ShowStatMatch();
                metroPanel3.Visible = true;
                metroTabControl1.SelectTab(tabMatchint);
                metroPanel6.Visible = false;
            }
            thread2.Dispose();
            isMatchLoading = false;
            GetInfoMatch1.Enabled = true;
            matchIDbox1.Enabled = true;
        }
        
        private void ThreadProfile1_Click()
        {
            thread3 = new BackgroundWorker();
            thread3.WorkerReportsProgress = true;
            thread3.WorkerSupportsCancellation = true;
            thread3.DoWork += ThreadProfile1_Work;
            thread3.ProgressChanged += ThreadProfile1_ProgressChanged;
            thread3.RunWorkerCompleted += ThreadProfile1_RunWorkerCompleted;
        }
        private void ThreadProfile1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            metroProgressBar3.Value = e.ProgressPercentage;
        }
        private void ThreadProfile1_Work(object sender, DoWorkEventArgs e)
        {
            CheckOnLoadedLogins();
            if(loadingLogin == loadedLogins.login1)
            {
                profile = null;
                if (profile == null)
                {
                    profile = new PlayerCSGO();
                    profile.GetInfo(faceit, login1.Text);
                }
                isLogin1Loaded = true;
                e.Result = 200;    // будет передано в RunWorkerComрleted
                return;
            }
            else if(loadingLogin == loadedLogins.login2)
            {
                profile2 = null;
                if (profile2 == null)
                {
                    profile2 = new PlayerCSGO();
                    profile2.GetInfo(faceit, login2.Text);
                }
                isLogin2Loaded = true;
                e.Result = 200;    // будет передано в RunWorkerComрleted
                return;
            }
            e.Result = -1;
        }
        private void ThreadProfile1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                metroLabel1.Text = "Process was cancelled. Stats.";
                statusLoadedLogin = LOADING_STATUS.Bad;
            }
            else if (e.Error != null)
            {
                metroLabel1.Text = e.Error.Message;
                Form2 form = new Form2();
                form.errorMsg.Text = e.Error.Message;
                form.Show();
                statusLoadedLogin = LOADING_STATUS.Bad;
            }
            else
            {
                if((int)e.Result == 200)
                {
                    statusLogin = LOADING_STATUS.Good;
                }
                else
                {
                    statusLogin = LOADING_STATUS.Bad;
                }
            }
        }

        private void LoadProfileFromMatchStats(int i)
        {
            List<CSGOMatchStatsTeamPlayer> pl_stats = new List<CSGOMatchStatsTeamPlayer>();
            foreach (CSGOMatchStatsTeamPlayer pl in matchStatistics.team_A.players)
            {
                pl_stats.Add(pl);
            }
            foreach (CSGOMatchStatsTeamPlayer pl in matchStatistics.team_B.players)
            {
                pl_stats.Add(pl);
            }
            
            if (isLogin1Loaded == false)
            {
                login1.Text = pl_stats[i].nickname;
                getProfile1.PerformClick();
            }
            else if (isLogin2Loaded == false)
            {
                login2.Text = pl_stats[i].nickname;
                getProfile2.PerformClick();
            }
            else if (lastLoadedLogin == loadedLogins.login1)
            {
                login2.Text = pl_stats[i].nickname;
                getProfile2.PerformClick();

            }
            else if (lastLoadedLogin == loadedLogins.login2)
            {
                login1.Text = pl_stats[i].nickname;
                getProfile1.PerformClick();
            }
            else
            {
                login1.Text = pl_stats[i].nickname;
                getProfile1.PerformClick();
            }
        }
        private void metroLabel289_Click(object sender, EventArgs e)
        {

        }

        private void metroLabel389_Click(object sender, EventArgs e)
        {

        }

        private void teamBResult_Click(object sender, EventArgs e)
        {

        }

        private void metroButton36_Click(object sender, EventArgs e)
        {
            LoadProfileFromMatchStats(0);
        }

        private void statsNickname2_Click(object sender, EventArgs e)
        {
            LoadProfileFromMatchStats(1);
        }
    }
    public class Remote
    {
        public Engine faceit = new Engine("c275df18-34f8-467f-a03b-e44bb7fcf581");
        public FaceitExeption exeption;
    }
    public class SysELO
    {
        public long tact;
        public int elo;
        public string player_id;
        public string nickname;
    }
    public class ParserInteger
    {
        public static int ParsInt(string index)
        {
            return int.Parse(index.Replace(",", ""));
        }
        public static double ParsDouble(string index)
        {
            string temp = index.Replace(",", "");
            return double.Parse(temp.Replace(".",","));
        }
    }
    public static class JsonExtensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) || 
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
    }
    public static class ConnectivityChecker
    {
        public enum ConnectionStatus
        {
            NotConnected,
            LimitedAccess,
            Connected
        }

        public static ConnectionStatus CheckInternet()
        {
            // Проверить подключение к dns.msftncsi.com
            try
            {
                IPHostEntry entry = Dns.GetHostEntry("dns.msftncsi.com");
                if (entry.AddressList.Length == 0)
                {
                    return ConnectionStatus.NotConnected;
                }
                else
                {
                    if (!entry.AddressList[0].ToString().Equals("131.107.255.255"))
                    {
                        return ConnectionStatus.LimitedAccess;
                    }
                }
            }
            catch
            {
                return ConnectionStatus.NotConnected;
            }

            // Проверить загрузку документа ncsi.txt
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.msftncsi.com/ncsi.txt");
            try
            {
                HttpWebResponse responce = (HttpWebResponse)request.GetResponse();

                if (responce.StatusCode != HttpStatusCode.OK)
                {
                    return ConnectionStatus.LimitedAccess;
                }
                using (StreamReader sr = new StreamReader(responce.GetResponseStream()))
                {
                    if (sr.ReadToEnd().Equals("Microsoft NCSI"))
                    {
                        return ConnectionStatus.Connected;
                    }
                    else
                    {
                        return ConnectionStatus.LimitedAccess;
                    }
                }
            }
            catch
            {
                return ConnectionStatus.NotConnected;
            }

        }
    }
}

